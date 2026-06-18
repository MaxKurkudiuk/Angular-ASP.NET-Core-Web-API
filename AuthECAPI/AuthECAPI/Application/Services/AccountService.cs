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

    public async Task<(UserProfileResponse? Profile, string? ErrorMessage)> UpdateUserProfileAsync(string userId, UpdateUserProfileModel model)
    {
        var userDetails = await _userManager.FindByIdAsync(userId);
        if (userDetails is null)
            return (null, "User not found.");

        if (model.FullName is not null)
            userDetails.FullName = model.FullName;

        if (model.Gender is not null)
            userDetails.Gender = model.Gender;

        if (model.Age is not null)
            userDetails.DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-model.Age.Value));

        if (model.LibraryID is not null)
            userDetails.LibraryID = model.LibraryID;

        var result = await _userManager.UpdateAsync(userDetails);
        if (!result.Succeeded)
            return (null, string.Join("; ", result.Errors.Select(e => e.Description)));

        var age = (DateTime.Now.Year - userDetails.DOB.Year).ToString();
        var roles = await _userManager.GetRolesAsync(userDetails);

        return (new UserProfileResponse
        {
            Email = userDetails.Email,
            FullName = userDetails.FullName,
            Age = age,
            Gender = userDetails.Gender,
            Roles = roles,
            LibraryID = userDetails.LibraryID
        }, null);
    }

    public async Task<(bool isSuccess, string? ErrorMessage)> DeleteUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return (false, "User not found.");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }
}
