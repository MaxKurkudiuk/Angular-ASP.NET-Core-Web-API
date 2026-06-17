namespace AuthECAPI.Application.Models;

public class UserRegistrationModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public int Age { get; set; }
    public int? LibraryID { get; set; }
}
