using System.Security.Cryptography;
using AuthECAPI.Shared.Models;
using AuthECAPI.Shared.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AuthECAPI.Tests.Shared.Services;

public class AuthServiceTests
{
    private static Mock<UserManager<AppUser>> CreateMockUserManager()
    {
        var store = Mock.Of<IUserStore<AppUser>>();
        var mgr = new Mock<UserManager<AppUser>>(
            store, null!, null!, null!, null!, null!, null!, null!, null!);
        mgr.Object.UserValidators.Add(new UserValidator<AppUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<AppUser>());
        return mgr;
    }

    [Fact]
    public async Task SignUpAsync_Should_Create_User_And_Add_To_Role()
    {
        var userManagerMock = CreateMockUserManager();
        var tokenServiceMock = new Mock<ITokenService>();
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

        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), model.Password))
            .ReturnsAsync(IdentityResult.Success);

        userManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), model.Role))
            .ReturnsAsync(IdentityResult.Success);

        var service = new AuthService(userManagerMock.Object, tokenServiceMock.Object);
        var result = await service.SignUpAsync(model);

        Assert.True(result.Succeeded);
        userManagerMock.Verify(x => x.CreateAsync(
            It.Is<AppUser>(u => u.Email == model.Email && u.FullName == model.FullName),
            model.Password), Times.Once);
        userManagerMock.Verify(x => x.AddToRoleAsync(
            It.Is<AppUser>(u => u.Email == model.Email), model.Role), Times.Once);
    }

    [Fact]
    public async Task SignUpAsync_Should_Return_Failure_When_Create_User_Fails()
    {
        var userManagerMock = CreateMockUserManager();
        var tokenServiceMock = new Mock<ITokenService>();
        var model = new UserRegistrationModel
        {
            Email = "test@example.com",
            Password = "P@ssw0rd!",
            FullName = "Test User",
            Role = "Student",
            Gender = "Male",
            Age = 25
        };

        var identityError = new IdentityError { Code = "DuplicateEmail", Description = "Email already exists." };
        userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), model.Password))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        var service = new AuthService(userManagerMock.Object, tokenServiceMock.Object);
        var result = await service.SignUpAsync(model);

        Assert.False(result.Succeeded);
        Assert.Equal("DuplicateEmail", result.Errors.First().Code);
        userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SignInAsync_Should_Return_Token_On_Valid_Credentials()
    {
        var userManagerMock = CreateMockUserManager();
        var tokenServiceMock = new Mock<ITokenService>();
        var model = new LoginModel { Email = "test@example.com", Password = "P@ssw0rd!" };
        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.Email,
            Email = model.Email,
            FullName = "Test User",
            Gender = "Male",
            DOB = new DateOnly(2000, 1, 1),
            LibraryID = 123
        };
        var roles = new List<string> { "Student" };
        var expectedToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        userManagerMock
            .Setup(x => x.FindByEmailAsync(model.Email))
            .ReturnsAsync(user);
        userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, model.Password))
            .ReturnsAsync(true);
        userManagerMock
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);
        tokenServiceMock
            .Setup(x => x.GenerateToken(user, roles))
            .Returns(expectedToken);

        var service = new AuthService(userManagerMock.Object, tokenServiceMock.Object);
        var (token, errorMessage) = await service.SignInAsync(model);

        Assert.Equal(expectedToken, token);
        Assert.Null(errorMessage);
    }

    [Fact]
    public async Task SignInAsync_Should_Return_Error_When_User_Not_Found()
    {
        var userManagerMock = CreateMockUserManager();
        var tokenServiceMock = new Mock<ITokenService>();
        var model = new LoginModel { Email = "unknown@example.com", Password = "P@ssw0rd!" };

        userManagerMock
            .Setup(x => x.FindByEmailAsync(model.Email))
            .ReturnsAsync((AppUser?)null);

        var service = new AuthService(userManagerMock.Object, tokenServiceMock.Object);
        var (token, errorMessage) = await service.SignInAsync(model);

        Assert.Null(token);
        Assert.Equal("Username or password is incorrect.", errorMessage);
    }

    [Fact]
    public async Task SignInAsync_Should_Return_Error_When_Password_Is_Wrong()
    {
        var userManagerMock = CreateMockUserManager();
        var tokenServiceMock = new Mock<ITokenService>();
        var model = new LoginModel { Email = "test@example.com", Password = "WrongPassword!" };
        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.Email,
            Email = model.Email
        };

        userManagerMock
            .Setup(x => x.FindByEmailAsync(model.Email))
            .ReturnsAsync(user);
        userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, model.Password))
            .ReturnsAsync(false);

        var service = new AuthService(userManagerMock.Object, tokenServiceMock.Object);
        var (token, errorMessage) = await service.SignInAsync(model);

        Assert.Null(token);
        Assert.Equal("Username or password is incorrect.", errorMessage);
    }
}
