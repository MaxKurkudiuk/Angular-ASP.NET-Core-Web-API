using Microsoft.AspNetCore.Identity;
using AuthECAPI.Shared.Models;
using AuthECAPI.Core.Interfaces;

namespace AuthECAPI.Shared.Services;

public class AuthService(UserManager<AppUser> userManager, ITokenService tokenService) : IAuthService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<IdentityResult> SignUpAsync(UserRegistrationModel model)
    {
        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Gender = model.Gender,
            DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-model.Age)),
            LibraryID = model.LibraryID
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return result;

        await _userManager.AddToRoleAsync(user, model.Role);
        return result;
    }

    public async Task<(string? Token, string? ErrorMessage)> SignInAsync(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return (null, "Username or password is incorrect.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);
        return (token, null);
    }
}
