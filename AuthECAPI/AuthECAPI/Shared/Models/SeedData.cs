using AuthECAPI.Core.Entities;
using AuthECAPI.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuthECAPI.Shared.Models;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = serviceProvider.GetRequiredService<AppDbContext>();

        context.Database.EnsureCreated();

        if (context.Users.Any()) return;

        var roles = new[] { nameof(Roles.Admin), nameof(Roles.Teacher), nameof(Roles.Student) };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var users = new[]
        {
            new UserRegistrationModel { Email = "admin1_m_age51@gmail.com",    Password = "Admin1_m_age51",    FullName = "Admin 1",   Role = nameof(Roles.Admin),     Gender = "Male",    Age = 51 },
            new UserRegistrationModel { Email = "teacher1_f_age36@gmail.com",  Password = "Teacher1_f_age36",  FullName = "Teacher 1", Role = nameof(Roles.Teacher),   Gender = "Female",  Age = 36 },
            new UserRegistrationModel { Email = "teacher2_m_age40@gmail.com",  Password = "Teacher2_m_age40",  FullName = "Teacher 2", Role = nameof(Roles.Teacher),   Gender = "Male",    Age = 40 },
            new UserRegistrationModel { Email = "student1_f_age9@gmail.com",   Password = "Student1_f_age9",   FullName = "Student 1", Role = nameof(Roles.Student),   Gender = "Female",  Age = 9,    LibraryID = 555434 },
            new UserRegistrationModel { Email = "student2_f_age21@gmail.com",  Password = "Student2_f_age21",  FullName = "Student 2", Role = nameof(Roles.Student),   Gender = "Female",  Age = 21 },
            new UserRegistrationModel { Email = "student15_f_age9@gmail.com",  Password = "Student3_f_age15",  FullName = "Student 3", Role = nameof(Roles.Student),   Gender = "Female",  Age = 15,   LibraryID = 555435 },
            new UserRegistrationModel { Email = "student4_m_age30@gmail.com",  Password = "Student4_m_age30",  FullName = "Student 4", Role = nameof(Roles.Student),   Gender = "Male",    Age = 39 }
        };

        foreach (var userData in users)
        {
            var user = new AppUser { 
                UserName = userData.Email, 
                Email = userData.Email,
                FullName = userData.FullName,
                Gender = userData.Gender,
                DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-userData.Age)),
                LibraryID = userData.LibraryID
            };
            await userManager.CreateAsync(user, userData.Password);
            await userManager.AddToRoleAsync(user, userData.Role);
        }

        Console.WriteLine($"{users.Length} test users seeded");
    }
}
