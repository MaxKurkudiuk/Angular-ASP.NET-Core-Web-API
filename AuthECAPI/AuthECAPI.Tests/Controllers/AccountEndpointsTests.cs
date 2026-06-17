using System.Security.Claims;
using AuthECAPI.Controllers;
using AuthECAPI.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AuthECAPI.Tests.Controllers;

public class AccountEndpointsTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;

    public AccountEndpointsTests()
    {
        var store = Mock.Of<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(
            store, null!, null!, null!, null!, null!, null!, null!, null!);
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
        var user = new AppUser
        {
            Id = userId,
            Email = "test@example.com",
            FullName = "Test User",
            UserName = "test@example.com"
        };

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        var principal = CreateUser(userId);
        var result = await AccountEndpoints.GetUserProfile(principal, _userManagerMock.Object);

        Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(result);
        Assert.StartsWith("Ok", result.GetType().Name);

        var value = result.GetType().GetProperty("Value")!.GetValue(result);
        var emailProp = value!.GetType().GetProperty("Email");
        var fullNameProp = value.GetType().GetProperty("FullName");
        Assert.NotNull(emailProp);
        Assert.NotNull(fullNameProp);
        Assert.Equal("test@example.com", emailProp.GetValue(value));
        Assert.Equal("Test User", fullNameProp.GetValue(value));
    }

    [Fact]
    public async Task GetUserProfile_NonExistentUser_ReturnsOkWithNull()
    {
        var userId = Guid.NewGuid().ToString();

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((AppUser?)null);

        var principal = CreateUser(userId);
        var result = await AccountEndpoints.GetUserProfile(principal, _userManagerMock.Object);

        Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(result);
        Assert.StartsWith("Ok", result.GetType().Name);

        var value = result.GetType().GetProperty("Value")!.GetValue(result);
        var emailProp = value!.GetType().GetProperty("Email");
        var fullNameProp = value.GetType().GetProperty("FullName");
        Assert.NotNull(emailProp);
        Assert.NotNull(fullNameProp);
        Assert.Null(emailProp.GetValue(value));
        Assert.Null(fullNameProp.GetValue(value));
    }

    [Fact]
    public async Task GetUserProfile_MissingUserIdClaim_Throws()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => AccountEndpoints.GetUserProfile(principal, _userManagerMock.Object));
    }
}
