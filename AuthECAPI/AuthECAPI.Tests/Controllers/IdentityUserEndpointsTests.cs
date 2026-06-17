using AuthECAPI.Application.Models;
using AuthECAPI.Controllers;
using AuthECAPI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AuthECAPI.Tests.Controllers;

public class IdentityUserEndpointsTests
{
    private readonly Mock<IAuthService> _authServiceMock;

    public IdentityUserEndpointsTests()
    {
        _authServiceMock = new Mock<IAuthService>();
    }

    #region POST /api/signup

    [Fact]
    public async Task CreateUser_Success_ReturnsOk()
    {
        var model = new UserRegistrationModel
        {
            Email = "test@example.com",
            Password = "P@ssw0rd!",
            FullName = "Test User",
            Role = "Student",
            Gender = "Male",
            Age = 25,
            LibraryID = 123
        };

        _authServiceMock
            .Setup(x => x.SignUpAsync(model))
            .ReturnsAsync(IdentityResult.Success);

        var result = await IdentityUserEndpoints.CreateUser(_authServiceMock.Object, model);

        Assert.IsAssignableFrom<IResult>(result);
        Assert.StartsWith("Ok", result.GetType().Name);

        var value = GetResultValue(result);
        var identityResult = Assert.IsType<IdentityResult>(value);
        Assert.True(identityResult.Succeeded);
    }

    [Fact]
    public async Task CreateUser_Failure_ReturnsBadRequest()
    {
        var model = new UserRegistrationModel
        {
            Email = "duplicate@example.com",
            Password = "P@ssw0rd!",
            FullName = "Test User",
            Role = "Student",
            Gender = "Male",
            Age = 25
        };

        var identityError = new IdentityError
        {
            Code = "DuplicateEmail",
            Description = "Email already exists."
        };

        _authServiceMock
            .Setup(x => x.SignUpAsync(model))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        var result = await IdentityUserEndpoints.CreateUser(_authServiceMock.Object, model);

        Assert.IsAssignableFrom<IResult>(result);
        Assert.StartsWith("BadRequest", result.GetType().Name);

        var value = GetResultValue(result);
        var identityResult = Assert.IsType<IdentityResult>(value);
        Assert.False(identityResult.Succeeded);
        Assert.Equal("DuplicateEmail", identityResult.Errors.First().Code);
    }

    #endregion

    #region POST /api/signin

    [Fact]
    public async Task SignIn_Success_ReturnsToken()
    {
        var model = new LoginModel { Email = "test@example.com", Password = "P@ssw0rd!" };
        var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test";

        _authServiceMock
            .Setup(x => x.SignInAsync(model))
            .ReturnsAsync((expectedToken, (string?)null));

        var result = await IdentityUserEndpoints.SignIn(_authServiceMock.Object, model);

        Assert.IsAssignableFrom<IResult>(result);
        Assert.StartsWith("Ok", result.GetType().Name);

        var value = GetResultValue(result);
        var tokenProp = value!.GetType().GetProperty("token");
        Assert.NotNull(tokenProp);
        Assert.Equal(expectedToken, tokenProp.GetValue(value));
    }

    [Fact]
    public async Task SignIn_Failure_ReturnsBadRequestWithError()
    {
        var model = new LoginModel { Email = "unknown@example.com", Password = "WrongPassword!" };
        var errorMessage = "Username or password is incorrect.";

        _authServiceMock
            .Setup(x => x.SignInAsync(model))
            .ReturnsAsync(((string?)null, errorMessage));

        var result = await IdentityUserEndpoints.SignIn(_authServiceMock.Object, model);

        Assert.IsAssignableFrom<IResult>(result);
        Assert.StartsWith("BadRequest", result.GetType().Name);

        var value = GetResultValue(result);
        var messageProp = value!.GetType().GetProperty("message");
        Assert.NotNull(messageProp);
        Assert.Equal(errorMessage, messageProp.GetValue(value));
    }

    #endregion

    private static object? GetResultValue(IResult result)
    {
        var prop = result.GetType().GetProperty("Value");
        return prop?.GetValue(result);
    }
}
