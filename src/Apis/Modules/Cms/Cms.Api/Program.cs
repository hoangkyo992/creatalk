using System.Reflection;
using Cms.Api;
using Cms.Infrastructure;
using Common.Infrastructure;
using Serilog;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args)
    .ConfigureDefaultApplication();

var assemblies = new[]
{
    Assembly.GetAssembly(typeof(Cms.Application.Assembly)),
    Assembly.GetExecutingAssembly()
};
builder.Services.AddCommonServices(assemblies);
builder.Services.AddCmsServices(builder.Configuration)
    .AddIdentityService()
    .AddCdnService();

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.UseCmsServices();
app.SetupDefaultApplication();
app.Run();

app.Logger.LogInformation("LAUNCHING ---CMS-API---");