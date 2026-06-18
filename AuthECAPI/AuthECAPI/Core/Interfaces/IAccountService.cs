using AuthECAPI.Application.Models;

namespace AuthECAPI.Core.Interfaces;

public interface IAccountService
{
    Task<UserProfileResponse?> GetUserProfileAsync(string userId);
}
