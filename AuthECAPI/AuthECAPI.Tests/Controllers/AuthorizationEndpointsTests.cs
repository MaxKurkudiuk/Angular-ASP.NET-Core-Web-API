using System.Net;
using System.Net.Http.Headers;

namespace AuthECAPI.Tests.Controllers;

[Collection("Integration Tests")]
public class AuthorizationEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    private const string AdminToken = "AdminToken";
    private const string TeacherFemaleToken = "TeacherFemaleToken";
    private const string TeacherMaleToken = "TeacherMaleToken";
    private const string StudentFemaleUnder10LibraryIdToken = "StudentFemaleUnder10LibraryIdToken";
    private const string StudentFemaleNoLibraryIdToken = "StudentFemaleNoLibraryIdToken";
    private const string StudentMaleToken = "StudentMaleToken";

    public AuthorizationEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private void SetAuthHeader(string tokenKey)
    {
        var token = tokenKey switch
        {
            AdminToken => TestTokenHelper.GenerateToken("admin1", "Admin", "Male", 51),
            TeacherFemaleToken => TestTokenHelper.GenerateToken("teacher1", "Teacher", "Female", 36),
            TeacherMaleToken => TestTokenHelper.GenerateToken("teacher2", "Teacher", "Male", 40),
            StudentFemaleUnder10LibraryIdToken => TestTokenHelper.GenerateToken("student1", "Student", "Female", 9, 555434),
            StudentFemaleNoLibraryIdToken => TestTokenHelper.GenerateToken("student2", "Student", "Female", 21),
            StudentMaleToken => TestTokenHelper.GenerateToken("student4", "Student", "Male", 39),
            _ => throw new ArgumentException("Unknown token key")
        };

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private void ClearAuthHeader()
    {
        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task AdminOnly_Returns_200_For_Admin()
    {
        SetAuthHeader(AdminToken);
        var response = await _client.GetAsync("/api/AdminOnly");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(TeacherFemaleToken)]
    [InlineData(TeacherMaleToken)]
    [InlineData(StudentFemaleUnder10LibraryIdToken)]
    [InlineData(StudentMaleToken)]
    public async Task AdminOnly_Returns_403_For_NonAdmin(string tokenKey)
    {
        SetAuthHeader(tokenKey);
        var response = await _client.GetAsync("/api/AdminOnly");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminOnly_Returns_401_For_Unauthenticated()
    {
        ClearAuthHeader();
        var response = await _client.GetAsync("/api/AdminOnly");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(AdminToken)]
    [InlineData(TeacherFemaleToken)]
    [InlineData(TeacherMaleToken)]
    public async Task AdminOrTeacher_Returns_200_For_Admin_And_Teacher(string tokenKey)
    {
        SetAuthHeader(tokenKey);
        var response = await _client.GetAsync("/api/AdminOrTeacher");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(StudentFemaleUnder10LibraryIdToken)]
    [InlineData(StudentMaleToken)]
    public async Task AdminOrTeacher_Returns_403_For_Student(string tokenKey)
    {
        SetAuthHeader(tokenKey);
        var response = await _client.GetAsync("/api/AdminOrTeacher");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminOrTeacher_Returns_401_For_Unauthenticated()
    {
        ClearAuthHeader();
        var response = await _client.GetAsync("/api/AdminOrTeacher");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(StudentFemaleUnder10LibraryIdToken)]
    public async Task LibraryMembersOnly_Returns_200_For_User_With_LibraryID(string tokenKey)
    {
        SetAuthHeader(tokenKey);
        var response = await _client.GetAsync("/api/LibraryMembersOnly");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(AdminToken)]
    [InlineData(TeacherFemaleToken)]
    [InlineData(StudentFemaleNoLibraryIdToken)]
    [InlineData(StudentMaleToken)]
    public async Task LibraryMembersOnly_Returns_403_For_User_Without_LibraryID(string tokenKey)
    {
        SetAuthHeader(tokenKey);
        var response = await _client.GetAsync("/api/LibraryMembersOnly");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task LibraryMembersOnly_Returns_401_For_Unauthenticated()
    {
        ClearAuthHeader();
        var response = await _client.GetAsync("/api/LibraryMembersOnly");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(TeacherFemaleToken)]
    public async Task ApplyForMaternityLeave_Returns_200_For_Female_Teacher(string tokenKey)
    {
        SetAuthHeader(tokenKey);
        var response = await _client.GetAsync("/api/ApplyForMaternityLeave");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(AdminToken)]
    [InlineData(TeacherMaleToken)]
    [InlineData(StudentFemaleUnder10LibraryIdToken)]
    [InlineData(StudentMaleToken)]
    public async Task ApplyForMaternityLeave_Returns_403_For_Other(string tokenKey)
    {
        SetAuthHeader(tokenKey);
        var response = await _client.GetAsync("/api/ApplyForMaternityLeave");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ApplyForMaternityLeave_Returns_401_For_Unauthenticated()
    {
        ClearAuthHeader();
        var response = await _client.GetAsync("/api/ApplyForMaternityLeave");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(StudentFemaleUnder10LibraryIdToken)]
    public async Task Under10sAndFemale_Returns_200_For_Under10_Female(string tokenKey)
    {
        SetAuthHeader(tokenKey);
        var response = await _client.GetAsync("/api/Under10sAndFemale");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(AdminToken)]
    [InlineData(TeacherFemaleToken)]
    [InlineData(TeacherMaleToken)]
    [InlineData(StudentFemaleNoLibraryIdToken)]
    [InlineData(StudentMaleToken)]
    public async Task Under10sAndFemale_Returns_403_For_Others(string tokenKey)
    {
        SetAuthHeader(tokenKey);
        var response = await _client.GetAsync("/api/Under10sAndFemale");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Under10sAndFemale_Returns_401_For_Unauthenticated()
    {
        ClearAuthHeader();
        var response = await _client.GetAsync("/api/Under10sAndFemale");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
