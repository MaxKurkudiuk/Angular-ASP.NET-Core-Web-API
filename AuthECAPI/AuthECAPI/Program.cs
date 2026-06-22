using AuthECAPI.Application.Services;
using AuthECAPI.Controllers;
using AuthECAPI.Core.Entities;
using AuthECAPI.Core.Interfaces;
using AuthECAPI.Extensions;
using AuthECAPI.Infrastructure.Data;
using AuthECAPI.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

try {
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(new RenderedCompactJsonFormatter())
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .CreateLogger();
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddOpenApiExplorer()
                    .AddAppConfig(builder.Configuration)
                    .AddIdentityHandlersAndStores()
                    .ConfigureIdentityOptions()
                    .AddIdentityAuth(builder.Configuration);

    if (builder.Environment.IsEnvironment("Test"))
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
    } else {
        builder.Services.InjectDbContext(builder.Configuration);
    }

    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IAccountService, AccountService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseRouting();
    app.UseHttpsRedirection();
    app.ConfigureOpenApiExplorer()
       .ConfigureCors(builder.Configuration)
       .AddIdentityAuthMiddlewares();

    app.MapFallback(context =>
    {
        Log.Fatal("Host terminated unexpectedly.");
        return context.Response.WriteAsync("Host terminated unexpectedly.");
    });
    app.MapControllers();
    app.MapGroup("/api")
       .MapIdentityApi<AppUser>();
    app.MapGroup("/api")
       .MapIdentityUserEndpoints()
       .MapAccountEndpoints()
       .MapAdminEndpoints()
       .MapAuthorizationDemoEndpoints();
    app.AddSeedData();      // add test data

    Log.Information("Starting host...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}