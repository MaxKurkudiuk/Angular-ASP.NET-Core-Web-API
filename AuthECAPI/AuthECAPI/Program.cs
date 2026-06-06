using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Service from Identity Core
builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DevDB"),
        sqlOptions => sqlOptions.EnableRetryOnFailure())
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Map the visual UI page (e.g., /scalar/v1)
    app.MapScalarApiReference();
}

#region Config CORS
app.UseCors(options =>
    options.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader());
#endregion

app.UseAuthorization();

app.MapControllers();

app.MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapPost("/api/signup", async (
    UserManager<AppUser> userManager,
    [FromBody] UserRegiastrationModel userRegiastrationModel
    ) => {
        AppUser user = new() {
            UserName = userRegiastrationModel.Email,
            Email = userRegiastrationModel.Email,
            FullName = userRegiastrationModel.FullName
        };
        var result = await userManager.CreateAsync(
            user, 
            userRegiastrationModel.Password);

        if (result.Succeeded)
            return Results.Ok(result);
        return Results.BadRequest(result);
});

app.Run();