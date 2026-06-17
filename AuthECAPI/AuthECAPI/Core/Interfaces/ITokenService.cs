using AuthECAPI.Core.Entities;

namespace AuthECAPI.Core.Interfaces;

public interface ITokenService
{
    string GenerateToken(AppUser user, IList<string> roles);
}
