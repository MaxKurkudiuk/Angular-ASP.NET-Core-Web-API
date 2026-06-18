using System.Security.Claims;
using AuthECAPI.Application.Models;
using AuthECAPI.Controllers;
using AuthECAPI.Core.Interfaces;
using Moq;

namespace AuthECAPI.Tests.Controllers;

public class AccountEndpointsTests
{
    private readonly Mock<IAccountService> _accountServiceMock;

    public AccountEndpointsTests()
    {
        _accountServiceMock = new Mock<IAccountService>();
    }

    private static ClaimsPrincipal CreateUser(string userId)
    {
        var claims = new List<Claim> { new("userID", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task GetUserProfile_ExistingUser_ReturnsUserData()
    {
        var userId = Guid.NewGuid().ToString();
        var profile = new UserProfileResponse
        {
            Email = "test@example.com",
            FullName = "Test User",
            Age = "36",
            Gender = "M",
            Roles = new List<string>(),
            LibraryID = null
        };

        _accountServiceMock
            .Setup(x => x.GetUserProfileAsync(userId))
            .ReturnsAsync(profile);

        var principal = CreateUser(userId);
        var result = await AccountEndpoints.GetUserProfile(principal, _accountServiceMock.Object);

        var okResult = Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(result);
        var returnValue = result.GetType().GetProperty("Value")!.GetValue(result)!;

        Assert.Equal("test@example.com", returnValue.GetType().GetProperty("Email")!.GetValue(returnValue));
        Assert.Equal("Test User", returnValue.GetType().GetProperty("FullName")!.GetValue(returnValue));
        Assert.Equal("36", returnValue.GetType().GetProperty("Age")!.GetValue(returnValue));
        Assert.Equal("M", returnValue.GetType().GetProperty("Gender")!.GetValue(returnValue));
        Assert.Empty((IEnumerable<string>)returnValue.GetType().GetProperty("Roles")!.GetValue(returnValue)!);
    }

    [Fact]
    public async Task GetUserProfile_NonExistentUser_ReturnsOkWithNull()
    {
        var userId = Guid.NewGuid().ToString();

        _accountServiceMock
            .Setup(x => x.GetUserProfileAsync(userId))
            .ReturnsAsync((UserProfileResponse?)null);

        var principal = CreateUser(userId);
        var result = await AccountEndpoints.GetUserProfile(principal, _accountServiceMock.Object);

        Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(result);
        Assert.StartsWith("Ok", result.GetType().Name);
    }

    [Fact]
    public async Task GetUserProfile_MissingUserIdClaim_Throws()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => AccountEndpoints.GetUserProfile(principal, _accountServiceMock.Object));
    }
}
