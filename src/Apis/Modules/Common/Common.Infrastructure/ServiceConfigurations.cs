using System.Reflection;
using Common.Application.Services;
using Common.Infrastructure.MediatR;
using Common.Infrastructure.MediatR.Behaviours;
using Common.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure;

public static partial class ServiceConfigurations
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, params Assembly?[] assemblies)
    {
        services.AddAutoMapper(assemblies);
        services.AddValidatorsFromAssemblies(assemblies);
        services.AddMediatR(assemblies);
        services.AddTransient<IFeatureManager, FeatureManager>();
        return services;
    }

    public static IServiceCollection AddMediatR(this IServiceCollection services, params Assembly?[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.NotificationPublisher = new StrategyPublisher(); // Singleton
            cfg.NotificationPublisherType = typeof(StrategyPublisher); // ServiceLifetime
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            cfg.RegisterServicesFromAssemblies(assemblies);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(OverrideResponseBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        return services;
    }
}