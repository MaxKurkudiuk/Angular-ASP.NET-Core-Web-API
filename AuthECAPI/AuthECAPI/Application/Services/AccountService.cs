using AuthECAPI.Application.Models;
using AuthECAPI.Core.Entities;
using AuthECAPI.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthECAPI.Application.Services;

public class AccountService(
    UserManager<AppUser> userManager,
    ITokenService tokenService) : IAccountService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<UserProfileResponse?> GetUserProfileAsync(string userId)
    {
        var userDetails = await _userManager.FindByIdAsync(userId);
        if (userDetails is null)
            return null;

        var roles = await _userManager.GetRolesAsync(userDetails);

        return new UserProfileResponse
        {
            Email = userDetails.Email,
            FullName = userDetails.FullName,
            Age = (DateTime.Now.Year - userDetails.DOB.Year).ToString(),
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

        var claimsChanged = false;

        var isFullNameChanged = userDetails.FullName != model.FullName;
        var isGenderChanged = userDetails.Gender != model.Gender;
        var isAgeChanged = userDetails.DOB.Year != DateTime.Now.AddYears(-model.Age).Year;
        var isLibraryIdChanged = userDetails.LibraryID != model.LibraryID;
        if (isFullNameChanged || isGenderChanged || isAgeChanged || isLibraryIdChanged)
        {
            userDetails.FullName = model.FullName;
            userDetails.Gender = model.Gender;
            userDetails.DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-model.Age));
            userDetails.LibraryID = model.LibraryID;
            claimsChanged = true;
        }

        var result = await _userManager.UpdateAsync(userDetails);
        if (!result.Succeeded)
            return (null, string.Join("; ", result.Errors.Select(e => e.Description)));

        var roles = await _userManager.GetRolesAsync(userDetails);

        var profile = new UserProfileResponse
        {
            Email = userDetails.Email,
            FullName = userDetails.FullName,
            Age = (DateTime.Now.Year - userDetails.DOB.Year).ToString(),
            Gender = userDetails.Gender,
            Roles = roles,
            LibraryID = userDetails.LibraryID
        };

        if (claimsChanged)
        {
            profile.Token = _tokenService.GenerateToken(userDetails, roles);
        }

        return (profile, null);
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
