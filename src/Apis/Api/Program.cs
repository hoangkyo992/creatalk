using System.Reflection;
using Auth.Api;
using Auth.Infrastructure;
using Cdn.Api;
using Cdn.Infrastructure;
using Cms.Api;
using Cms.Infrastructure;
using Common.Api;
using Common.Infrastructure;
using Serilog;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args)
    .ConfigureDefaultApplication();

var assemblies = new[]
{
    Assembly.GetAssembly(typeof(Auth.Application.ServiceConfigurations)),
    Assembly.GetAssembly(typeof(Cdn.Application.ServiceConfigurations)),
    Assembly.GetAssembly(typeof(Cms.Application.ServiceConfigurations)),
    Assembly.GetExecutingAssembly()
};
builder.Services.AddCommonServices(assemblies);
builder.Services
    .AddAuthServices(builder.Configuration)
    .AddCdnServices(builder.Configuration)
    .AddCmsServices(builder.Configuration)
    .AddIdentityService(useInProcessService: true)
    .AddCdnService(useInProcessService: true);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.UseAuthServices();
app.UseCdnServices();
app.UseCmsServices();
app.SetupDefaultApplication();
app.Run();

app.Logger.LogInformation("LAUNCHING ---Creatalk-API---");