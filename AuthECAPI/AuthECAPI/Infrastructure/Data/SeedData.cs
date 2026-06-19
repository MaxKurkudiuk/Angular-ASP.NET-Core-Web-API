using AuthECAPI.Core.Entities;
using AuthECAPI.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;

namespace AuthECAPI.Infrastructure.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = serviceProvider.GetRequiredService<AppDbContext>();

        // 1. Configure the retry strategy
        var retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                // Only retry transient network/connection issues
                ShouldHandle = new PredicateBuilder().Handle<SqlException>(),
                MaxRetryAttempts = 15,
                // Wait 2 seconds between attempts
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Constant
            })
            .Build();

        // 2. Execute your EF Core initialization safely
        await retryPipeline.ExecuteAsync(async token =>
            await context.Database.EnsureCreatedAsync(token));

        if (context.Users.Any()) return;

        var roles = new[] { nameof(Roles.Admin), nameof(Roles.Teacher), nameof(Roles.Student) };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var seedUsers = new[]
        {
            new { Email = "admin1_m_age51@gmail.com",    Password = "Admin1_m_age51",    FullName = "Admin 1",   Role = nameof(Roles.Admin),     Gender = "Male",    Age = (int?)51, LibraryID = (int?)null },
            new { Email = "teacher1_f_age36@gmail.com",  Password = "Teacher1_f_age36",  FullName = "Teacher 1", Role = nameof(Roles.Teacher),   Gender = "Female",  Age = (int?)36, LibraryID = (int?)null },
            new { Email = "teacher2_m_age40@gmail.com",  Password = "Teacher2_m_age40",  FullName = "Teacher 2", Role = nameof(Roles.Teacher),   Gender = "Male",    Age = (int?)40, LibraryID = (int?)null },
            new { Email = "student1_f_age9@gmail.com",   Password = "Student1_f_age9",   FullName = "Student 1", Role = nameof(Roles.Student),   Gender = "Female",  Age = (int?)9,  LibraryID = (int?)555434 },
            new { Email = "student2_f_age21@gmail.com",  Password = "Student2_f_age21",  FullName = "Student 2", Role = nameof(Roles.Student),   Gender = "Female",  Age = (int?)21, LibraryID = (int?)null },
            new { Email = "student15_f_age9@gmail.com",  Password = "Student3_f_age15",  FullName = "Student 3", Role = nameof(Roles.Student),   Gender = "Female",  Age = (int?)15, LibraryID = (int?)555435 },
            new { Email = "student4_m_age30@gmail.com",  Password = "Student4_m_age30",  FullName = "Student 4", Role = nameof(Roles.Student),   Gender = "Male",    Age = (int?)39, LibraryID = (int?)null }
        };

        foreach (var userData in seedUsers)
        {
            var user = new AppUser
            {
                UserName = userData.Email,
                Email = userData.Email,
                FullName = userData.FullName,
                Gender = userData.Gender,
                LibraryID = userData.LibraryID
            };
            if (userData.Age is int age)
            {
                user.DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-age));
            }
            await userManager.CreateAsync(user, userData.Password);
            await userManager.AddToRoleAsync(user, userData.Role);
        }

        Console.WriteLine($"{seedUsers.Length} test users seeded");
    }
}
