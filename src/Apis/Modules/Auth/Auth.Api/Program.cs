using System.Reflection;
using Auth.Api;
using Auth.Infrastructure;
using Common.Infrastructure;
using Serilog;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args)
    .ConfigureDefaultApplication();

var assemblies = new[]
{
    Assembly.GetAssembly(typeof(Auth.Application.ServiceConfigurations)),
    Assembly.GetExecutingAssembly()
};
builder.Services.AddCommonServices(assemblies);
builder.Services.AddAuthServices(builder.Configuration)
    .AddIdentityService(useInProcessService: true);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.UseAuthServices();
app.SetupDefaultApplication();
app.Run();

app.Logger.LogInformation("LAUNCHING ---AUTH-API---");