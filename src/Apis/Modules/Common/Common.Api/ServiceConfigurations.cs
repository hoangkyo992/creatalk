using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Common.Api.Binders;
using Common.Api.Configurations;
using Common.Api.Middleware;
using Common.Api.Middleware.AuthenticationSchemes;
using Common.Api.Services;
using Common.Infrastructure;
using Common.Infrastructure.Identity;
using HungHd.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Swagger;

namespace Common.Api;

public static partial class ServiceConfigurations
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            var policy = configuration["Cors:PolicyName"];
            var origins = configuration.GetValue("Cors:Origins", string.Empty);
            if (!string.IsNullOrWhiteSpace(policy) && !string.IsNullOrWhiteSpace(origins))
            {
                options.AddPolicy(policy, builder => builder
                      .WithOrigins(origins.Split(",", StringSplitOptions.RemoveEmptyEntries))
                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                      .WithHeaders(HeaderNames.ContentType, "x-custom-header")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials());
            }

            options.AddPolicy("Default",
                builder => builder.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .Build());
        });

        services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

        return services;
    }

    public static IServiceCollection ConfigureApi(this IServiceCollection services,
        IConfiguration configuration,
        Action<MvcOptions>? configureMvcOptions = null,
        Action<JsonOptions>? configureJsonOptions = null)
    {
        services.Configure<ApplicationOptions>(configuration);
        services.Configure<LanguageConfigurations>(configuration.GetSection("Language"));
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

        services.AddControllers(opt =>
        {
            opt.EnableEndpointRouting = false;
            opt.Filters.Add<ValidateParametersAttribute>();
            opt.Filters.Add<ValidateModelAttribute>();
            opt.Filters.Add(new ProducesAttribute("application/json"));
            opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            opt.ModelBinderProviders.Insert(0, new ApplicationBinderProvider());
            configureMvcOptions?.Invoke(opt);

            var cacheProfiles = configuration
                .GetSection("CacheProfiles")
                .GetChildren()
                .ToArray();
            if (cacheProfiles.Length > 0)
            {
                foreach (var cacheProfile in cacheProfiles)
                {
                    var profile = cacheProfile.Get<CacheProfile>();
                    if (profile != null)
                    {
                        opt.CacheProfiles.Add(cacheProfile.Key, profile);
                    }
                }
            }
            else
            {
                opt.CacheProfiles.Add("Default", new CacheProfile
                {
                    Duration = 3600,
                    Location = ResponseCacheLocation.Any
                });
            }
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DateTimeUniversalConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
            configureJsonOptions?.Invoke(options);
        });

        services.AddHttpContextAccessor();
        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var assemblyName = Assembly.GetEntryAssembly()?.GetName();
            if (assemblyName != null)
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, $"{assemblyName.Name}.xml");
                if (File.Exists(filePath))
                {
                    options.IncludeXmlComments(filePath);
                }
            }
            options.ResolveConflictingActions(actions =>
            {
                var action = actions.OrderByDescending(c =>
                    {
                        var o = c.ActionDescriptor.EndpointMetadata.FirstOrDefault(c => c is HttpMethodAttribute) as HttpMethodAttribute;
                        return o?.Order ?? 0;
                    }).FirstOrDefault();
                return action;
            });
            options.EnableAnnotations();
        });
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.AddScoped<IEndpointDefinitionProvider, SwaggerEndpointDefinitionProvider>();
        return services;
    }

    public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = ApiVersion.Default;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader =
            ApiVersionReader.Combine(
               new HeaderApiVersionReader("X-Api-Version"),
               new QueryStringApiVersionReader("version"));
        }).AddApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });
        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(NullAuthenticationSchemeOptions.SchemeName)
            .AddScheme<NullAuthenticationSchemeOptions, NullAuthenticationHandler>("NullAuthenticationScheme", options => options.ForwardDefaultSelector =
                    context => (context.Request.Path.StartsWithSegments("/api") && context.GetEndpoint()?.Metadata?.GetMetadata<IAllowAnonymous>() == null)
                        || context.Request.Path.StartsWithSegments("/swagger/apis")
                        ? SessionBasedAuthenticationSchemeOptions.SchemeName
                        : null)
            .AddScheme<SessionBasedAuthenticationSchemeOptions, SessionBasedAuthenticationHandler>(SessionBasedAuthenticationSchemeOptions.SchemeName, options => { });

        services.AddAuthorization();
        services.AddTransient<ICurrentUser>((provider) =>
        {
            var httpContextAccessor = provider.GetService<IHttpContextAccessor>();
            return httpContextAccessor?.HttpContext?.User is not null ? new CurrentUser(httpContextAccessor) : new AnonymousUser();
        });
        return services;
    }

    public static IServiceCollection ConfigureCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDistributedCache(configuration).AddRedisCache(configuration);
        services.AddMemoryCache(setupAction =>
        {
        });
        return services;
    }

    public static void ConfigureRateLimiter(this IServiceCollection services, IConfiguration configuration)
    {
        static string GetUserEndPoint(HttpContext context) =>
           $"User {context.User.Identity?.Name ?? "Anonymous"} endpoint:{context.Request.Path}"
           + $" {context.Connection.RemoteIpAddress}";

        var limitsOptions = new RateLimitsOptions();
        configuration.GetSection(RateLimitsOptions.RateLimits).Bind(limitsOptions);

        services.AddRateLimiter(options =>
        {
            options.OnRejected = (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.RequestServices.GetService<ILoggerFactory>()?
                    .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
                    .LogWarning("OnRejected: {GetUserEndPoint}", GetUserEndPoint(context.HttpContext));

                return new ValueTask();
            };

            options.AddPolicy(nameof(RateLimitPolicy.IdentitySliding), context =>
            {
                var username = "Anonymous";
                if (context.User.Identity?.IsAuthenticated is true)
                {
                    username = context.User.Identity.Name ?? context.User.FindUserId().ToString();
                }

                var options = limitsOptions.Policies.GetValueOrDefault(nameof(RateLimitPolicy.IdentitySliding))
                    ?? limitsOptions;

                return RateLimitPartition.GetSlidingWindowLimiter(username,
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = options.PermitLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = options.QueueLimit,
                        Window = TimeSpan.FromSeconds(options.Window),
                        SegmentsPerWindow = options.SegmentsPerWindow
                    });
            });

            options.AddPolicy(nameof(RateLimitPolicy.IdentityFixed), context =>
            {
                var username = "Anonymous";
                if (context.User.Identity?.IsAuthenticated is true)
                {
                    username = context.User.Identity.Name ?? context.User.FindUserId().ToString();
                }

                var options = limitsOptions.Policies.GetValueOrDefault(nameof(RateLimitPolicy.IdentityFixed))
                    ?? limitsOptions;

                return RateLimitPartition.GetFixedWindowLimiter(username,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = options.AutoReplenishment,
                        PermitLimit = options.PermitLimit,
                        QueueLimit = options.QueueLimit,
                        Window = TimeSpan.FromSeconds(options.Window)
                    });
            });

            options.AddPolicy(nameof(RateLimitPolicy.IdentityConcurrency), context =>
            {
                var username = "Anonymous";
                if (context.User.Identity?.IsAuthenticated is true)
                {
                    username = context.User.Identity.Name ?? context.User.FindUserId().ToString();
                }

                var options = limitsOptions.Policies.GetValueOrDefault(nameof(RateLimitPolicy.IdentityConcurrency))
                    ?? limitsOptions;

                return RateLimitPartition.GetConcurrencyLimiter(username,
                    _ => new ConcurrencyLimiterOptions
                    {
                        PermitLimit = options.PermitLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = options.QueueLimit
                    });
            });

            options.AddPolicy(nameof(RateLimitPolicy.IdentityTokenBucket), context =>
            {
                var username = "Anonymous";
                if (context.User.Identity?.IsAuthenticated is true)
                {
                    username = context.User.Identity.Name ?? context.User.FindUserId().ToString();
                }

                var options = limitsOptions.Policies.GetValueOrDefault(nameof(RateLimitPolicy.IdentityTokenBucket))
                    ?? limitsOptions;

                return RateLimitPartition.GetTokenBucketLimiter(username,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = options.TokenLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = options.QueueLimit,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(options.ReplenishmentPeriod),
                        TokensPerPeriod = options.TokensPerPeriod,
                        AutoReplenishment = options.AutoReplenishment
                    });
            });

            options.AddPolicy(nameof(RateLimitPolicy.IpAddressSliding), context =>
            {
                var remoteIpAddress = context.Request.Headers["X-Forwarded-For"].ToString()
                    ?? context.Connection.RemoteIpAddress?.ToString();

                var options = limitsOptions.Policies.GetValueOrDefault(nameof(RateLimitPolicy.IpAddressSliding))
                    ?? limitsOptions;

                return RateLimitPartition.GetSlidingWindowLimiter(remoteIpAddress,
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = options.PermitLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = options.QueueLimit,
                        Window = TimeSpan.FromSeconds(options.Window),
                        SegmentsPerWindow = options.SegmentsPerWindow
                    });
            });

            options.AddPolicy(nameof(RateLimitPolicy.IpAddressFixed), context =>
            {
                var remoteIpAddress = context.Request.Headers["X-Forwarded-For"].ToString()
                    ?? context.Connection.RemoteIpAddress?.ToString();

                var options = limitsOptions.Policies.GetValueOrDefault(nameof(RateLimitPolicy.IpAddressFixed))
                    ?? limitsOptions;

                return RateLimitPartition.GetFixedWindowLimiter(remoteIpAddress,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = options.AutoReplenishment,
                        PermitLimit = options.PermitLimit,
                        QueueLimit = options.QueueLimit,
                        Window = TimeSpan.FromSeconds(options.Window)
                    });
            });

            options.AddPolicy(nameof(RateLimitPolicy.IpAddressConcurrency), context =>
            {
                var remoteIpAddress = context.Request.Headers["X-Forwarded-For"].ToString()
                    ?? context.Connection.RemoteIpAddress?.ToString();

                var options = limitsOptions.Policies.GetValueOrDefault(nameof(RateLimitPolicy.IpAddressConcurrency))
                    ?? limitsOptions;

                return RateLimitPartition.GetConcurrencyLimiter(remoteIpAddress,
                    _ => new ConcurrencyLimiterOptions
                    {
                        PermitLimit = options.PermitLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = options.QueueLimit
                    });
            });

            options.AddPolicy(nameof(RateLimitPolicy.IpAddressTokenBucket), context =>
            {
                var remoteIpAddress = context.Request.Headers["X-Forwarded-For"].ToString()
                    ?? context.Connection.RemoteIpAddress?.ToString();

                var options = limitsOptions.Policies.GetValueOrDefault(nameof(RateLimitPolicy.IpAddressTokenBucket))
                    ?? limitsOptions;

                return RateLimitPartition.GetTokenBucketLimiter(remoteIpAddress,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = options.TokenLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = options.QueueLimit,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(options.ReplenishmentPeriod),
                        TokensPerPeriod = options.TokensPerPeriod,
                        AutoReplenishment = options.AutoReplenishment
                    });
            });

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
            {
                IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
                if (!IPAddress.IsLoopback(remoteIpAddress!) && limitsOptions.TokenLimit > 0)
                {
                    return RateLimitPartition.GetTokenBucketLimiter
                    (remoteIpAddress!, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = limitsOptions.TokenLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = limitsOptions.QueueLimit,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(limitsOptions.ReplenishmentPeriod),
                            TokensPerPeriod = limitsOptions.TokensPerPeriod,
                            AutoReplenishment = limitsOptions.AutoReplenishment
                        });
                }

                return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
            });
        });
    }

    public static void SetupSwagger(this WebApplication app,
        Action<SwaggerOptions>? setupAction = null)
    {
        app.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swagger, httpReq) =>
            {
            });
            setupAction?.Invoke(options);
        });

        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var groupName in provider.ApiVersionDescriptions.Select(c => c.GroupName))
            {
                // Use this option to enable IIS Sub-Application deployment
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "." : "..";
                options.SwaggerEndpoint(
                    $"{swaggerJsonBasePath}/swagger/{groupName}/swagger.json",
                    groupName.ToUpperInvariant());
            }
        });
    }

    public static void SetupPipelines(this WebApplication app,
        IConfiguration configuration,
        Action<IApplicationBuilder>? configureAuthorization = null)
    {
        app.UseHttpsRedirection();
        app.UseCors(configuration["Cors:PolicyName"] ?? "Default");

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseResponseCaching();
        app.UseResponseCompression();
        app.UseRequestDecompression();
        app.UseExceptionHandler();
        app.UseMiddleware<RequestResponseMiddleware>();
        app.UseMiddleware<AcceptLanguageMiddleware>();

        app.UseWhen(context => context.Request.Path.StartsWithSegments("/api") && context.GetEndpoint()?.Metadata?.GetMetadata<IAllowAnonymous>() == null,
            appBuilder =>
            {
                if (configureAuthorization != null)
                {
                    configureAuthorization.Invoke(appBuilder);
                }
                else
                {
                    appBuilder.UseMiddleware<BaseAuthorizationMiddleware>();
                }
            });

        app.UseMiddleware<TimeZoneMiddleware>();
        app.UseRateLimiter();
    }

    public static void SetupEndpoints(this WebApplication app)
    {
        app.MapControllers();

        app.MapGet("/swagger/apis", (HttpContext context) =>
        {
            var provider = context.RequestServices.GetRequiredService<IEndpointDefinitionProvider>();
            if (provider == null)
                return Results.BadRequest();
            var endpoints = provider.GetEndpoints(null, "/");
            return Results.Json(endpoints, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }).ExcludeFromDescription().AllowAnonymous();
    }

    public static WebApplicationBuilder ConfigureDefaultApplication(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureCors(builder.Configuration);
        builder.Services.ConfigureAuthentication();
        builder.Services.ConfigureApi(builder.Configuration);
        builder.Services.ConfigureApiVersioning();
        builder.Services.ConfigureSwagger();
        builder.Services.ConfigureCache(builder.Configuration);
        builder.Services.ConfigureRateLimiter(builder.Configuration);
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddResponseCaching();
        builder.Services.AddRequestDecompression();
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });
        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });

        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });

        if (builder.Configuration.GetValue("UseSentry", false))
        {
            builder.WebHost.UseSentry();
        }
        return builder;
    }

    public static void SetupDefaultApplication(this WebApplication app)
    {
        app.SetupSwagger();
        if (app.Configuration.GetValue("UseSentry", false))
        {
            app.UseSentryTracing();
        }
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStaticFiles();
        app.SetupPipelines(app.Configuration);
        app.SetupEndpoints();
    }
}