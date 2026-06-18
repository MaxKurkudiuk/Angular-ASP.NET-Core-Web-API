using AuthECAPI.Application.Models;
using AuthECAPI.Core.Entities;
using AuthECAPI.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthECAPI.Application.Services;

public class AccountService(UserManager<AppUser> userManager) : IAccountService
{
    private readonly UserManager<AppUser> _userManager = userManager;

    public async Task<UserProfileResponse?> GetUserProfileAsync(string userId)
    {
        var userDetails = await _userManager.FindByIdAsync(userId);
        if (userDetails is null)
            return null;

        var age = (DateTime.Now.Year - userDetails.DOB.Year).ToString();
        var roles = await _userManager.GetRolesAsync(userDetails);

        return new UserProfileResponse
        {
            Email = userDetails.Email,
            FullName = userDetails.FullName,
            Age = age,
            Gender = userDetails.Gender,
            Roles = roles,
            LibraryID = userDetails.LibraryID
        };
    }
}
