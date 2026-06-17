using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthECAPI.Tests.Controllers;

public static class TestTokenHelper
{
    private const string SecretKey = "GiveASecretKeyHavingAtLeast32Characters";

    public static string GenerateToken(string userId, string role, string gender, int age, int? libraryId = null)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

        var claims = new List<Claim>
        {
            new("userID", userId),
            new("gender", gender),
            new("age", age.ToString()),
            new(ClaimTypes.Role, role)
        };

        if (libraryId.HasValue)
            claims.Add(new Claim("libraryID", libraryId.Value.ToString()));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }
}
