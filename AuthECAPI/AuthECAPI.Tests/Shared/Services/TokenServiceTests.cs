using System.IdentityModel.Tokens.Jwt;
using AuthECAPI.Core.Entities;
using AuthECAPI.Shared.Models;
using AuthECAPI.Shared.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;

namespace AuthECAPI.Tests.Shared.Services;

public class TokenServiceTests
{
    private const string SecretKey = "ThisIsATestSecretKeyThatIsAtLeast32Characters!";

    private static IOptions<AppSettings> CreateSettings()
    {
        return Options.Create(new AppSettings { JWTSeecret = SecretKey });
    }

    private static AppUser CreateTestUser(bool withLibraryId = true)
    {
        return new AppUser
        {
            Id = "test-user-id-123",
            UserName = "test@example.com",
            Email = "test@example.com",
            FullName = "Test User",
            Gender = "Female",
            DOB = new DateOnly(2010, 6, 15),
            LibraryID = withLibraryId ? 555434 : null
        };
    }

    private static void AssertClaim(string token, string claimType, string expectedValue)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var claim = Assert.Single(jwtToken.Claims, c => c.Type == claimType);
        Assert.Equal(expectedValue, claim.Value);
    }

    [Fact]
    public void GenerateToken_Should_Include_All_Required_Claims()
    {
        var appSettings = CreateSettings();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(x => x.EnvironmentName).Returns("Development");
        var user = CreateTestUser();
        var roles = new List<string> { "Student" };

        var service = new TokenService(appSettings, envMock.Object);
        var token = service.GenerateToken(user, roles);

        AssertClaim(token, "userID", user.Id!);
        AssertClaim(token, "gender", user.Gender!);
        AssertClaim(token, "age", "16");
        AssertClaim(token, "role", "Student");
        AssertClaim(token, "libraryID", user.LibraryID.ToString()!);
    }

    [Fact]
    public void GenerateToken_Should_Exclude_LibraryID_Claim_When_Null()
    {
        var appSettings = CreateSettings();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(x => x.EnvironmentName).Returns("Development");
        var user = CreateTestUser(withLibraryId: false);
        var roles = new List<string> { "Teacher" };

        var service = new TokenService(appSettings, envMock.Object);
        var token = service.GenerateToken(user, roles);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.DoesNotContain(jwtToken.Claims, c => c.Type == "libraryID");
    }

    [Fact]
    public void GenerateToken_Should_Set_Expiration_To_1_Day_In_Development()
    {
        var appSettings = CreateSettings();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(x => x.EnvironmentName).Returns("Development");
        var user = CreateTestUser();
        var roles = new List<string> { "Admin" };

        var service = new TokenService(appSettings, envMock.Object);
        var token = service.GenerateToken(user, roles);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var expiry = jwtToken.ValidTo;
        var expectedExpiry = DateTime.UtcNow.AddDays(1);

        Assert.True(expiry > DateTime.UtcNow.AddHours(23));
        Assert.True(expiry <= expectedExpiry.AddMinutes(1));
    }

    [Fact]
    public void GenerateToken_Should_Set_Expiration_To_10_Minutes_In_Production()
    {
        var appSettings = CreateSettings();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(x => x.EnvironmentName).Returns("Production");
        var user = CreateTestUser();
        var roles = new List<string> { "Admin" };

        var service = new TokenService(appSettings, envMock.Object);
        var token = service.GenerateToken(user, roles);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var expiry = jwtToken.ValidTo;

        Assert.True(expiry > DateTime.UtcNow.AddMinutes(9));
        Assert.True(expiry <= DateTime.UtcNow.AddMinutes(10).AddSeconds(30));
    }

    [Fact]
    public void GenerateToken_Should_Return_Valid_JWT()
    {
        var appSettings = CreateSettings();
        var envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(x => x.EnvironmentName).Returns("Development");
        var user = CreateTestUser();
        var roles = new List<string> { "Student" };

        var service = new TokenService(appSettings, envMock.Object);
        var token = service.GenerateToken(user, roles);

        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));
    }
}
