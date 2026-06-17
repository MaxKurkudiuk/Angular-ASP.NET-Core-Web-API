using AuthECAPI.Application.Services;
using AuthECAPI.Controllers;
using AuthECAPI.Core.Entities;
using AuthECAPI.Core.Interfaces;
using AuthECAPI.Extensions;
using AuthECAPI.Infrastructure.Data;
using AuthECAPI.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.ConfigureOpenApiExplorer()
   .ConfigureCors(builder.Configuration)
   .AddIdentityAuthMiddlewares();

app.MapControllers();
app.MapGroup("/api")
   .MapIdentityApi<AppUser>();
app.MapGroup("/api")
   .MapIdentityUserEndpoints()
   .MapAccountEndpoints()
   .MapAuthorizationDemoEndpoints();
app.AddSeedData();      // add test data

app.Run();