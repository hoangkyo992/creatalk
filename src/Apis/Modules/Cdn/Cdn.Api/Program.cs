using System.Reflection;
using Cdn.Api;
using Cdn.Infrastructure;
using Common.Infrastructure;
using Serilog;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args)
    .ConfigureDefaultApplication();

var assemblies = new[]
{
    Assembly.GetAssembly(typeof(Cdn.Application.Assembly)),
    Assembly.GetExecutingAssembly()
};
builder.Services.AddCommonServices(assemblies);
builder.Services.AddCdnServices(builder.Configuration)
    .AddIdentityService();

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.UseCdnServices();
app.SetupDefaultApplication();
app.Run();

app.Logger.LogInformation("LAUNCHING ---CDN-API---");