using AuthECAPI.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthECAPI.Core.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> SignUpAsync(UserRegistrationModel model);
    Task<IdentityResult> AdminCreateUserAsync(AdminCreateUserModel model);
    Task<(string? Token, string? ErrorMessage)> SignInAsync(LoginModel model);
}
