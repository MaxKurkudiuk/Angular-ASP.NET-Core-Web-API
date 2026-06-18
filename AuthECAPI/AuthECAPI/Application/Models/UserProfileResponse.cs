namespace AuthECAPI.Application.Models;

public class UserProfileResponse
{
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string Age { get; set; } = "";
    public string? Gender { get; set; }
    public IList<string> Roles { get; set; } = [];
    public int? LibraryID { get; set; }
}
