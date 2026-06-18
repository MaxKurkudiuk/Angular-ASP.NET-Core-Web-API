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

    [Fact]
    public async Task UpdateUserProfile_ExistingUser_ReturnsUpdatedProfile()
    {
        var userId = Guid.NewGuid().ToString();
        var model = new UpdateUserProfileModel
        {
            FullName = "Updated Name",
            Gender = "F",
            Age = 25,
            LibraryID = 123
        };

        var updatedProfile = new UserProfileResponse
        {
            Email = "test@example.com",
            FullName = "Updated Name",
            Age = "25",
            Gender = "F",
            Roles = new List<string> { "Student" },
            LibraryID = 123
        };

        _accountServiceMock
            .Setup(x => x.UpdateUserProfileAsync(userId, model))
            .ReturnsAsync((updatedProfile, (string?)null));

        var principal = CreateUser(userId);
        var result = await AccountEndpoints.UpdateUserProfile(principal, _accountServiceMock.Object, model);

        var returnValue = result.GetType().GetProperty("Value")!.GetValue(result)!;

        Assert.Equal("Updated Name", returnValue.GetType().GetProperty("FullName")!.GetValue(returnValue));
        Assert.Equal("F", returnValue.GetType().GetProperty("Gender")!.GetValue(returnValue));
        Assert.Equal("25", returnValue.GetType().GetProperty("Age")!.GetValue(returnValue));
        Assert.Equal(123, returnValue.GetType().GetProperty("LibraryID")!.GetValue(returnValue));
    }

    [Fact]
    public async Task UpdateUserProfile_NonExistentUser_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid().ToString();
        var model = new UpdateUserProfileModel { FullName = "New Name" };

        _accountServiceMock
            .Setup(x => x.UpdateUserProfileAsync(userId, model))
            .ReturnsAsync(((UserProfileResponse?)null, "User not found."));

        var principal = CreateUser(userId);
        var result = await AccountEndpoints.UpdateUserProfile(principal, _accountServiceMock.Object, model);

        Assert.StartsWith("BadRequest", result.GetType().Name);
    }

    [Fact]
    public async Task UpdateUserProfile_MissingUserIdClaim_Throws()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        var model = new UpdateUserProfileModel { FullName = "New Name" };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => AccountEndpoints.UpdateUserProfile(principal, _accountServiceMock.Object, model));
    }
}
