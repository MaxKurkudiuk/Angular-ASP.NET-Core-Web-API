using Microsoft.AspNetCore.Identity;
using AuthECAPI.Shared.Models;

namespace AuthECAPI.Shared.Services;

public interface IAuthService
{
    Task<IdentityResult> SignUpAsync(UserRegistrationModel model);
    Task<(string? Token, string? ErrorMessage)> SignInAsync(LoginModel model);
}
