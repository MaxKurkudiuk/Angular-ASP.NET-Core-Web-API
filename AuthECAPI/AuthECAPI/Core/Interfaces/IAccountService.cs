using AuthECAPI.Application.Models;

namespace AuthECAPI.Core.Interfaces;

public interface IAccountService
{
    Task<UserProfileResponse?> GetUserProfileAsync(string userId);
    Task<(UserProfileResponse? Profile, string? ErrorMessage)> UpdateUserProfileAsync(string userId, UpdateUserProfileModel model);
    Task<(bool isSuccess, string? ErrorMessage)> DeleteUserProfileAsync(string userId);
}
