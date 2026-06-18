namespace AuthECAPI.Application.Models;

public class UpdateUserProfileModel
{
    public string? FullName { get; set; }
    public string? Gender { get; set; }
    public int? Age { get; set; }
    public int? LibraryID { get; set; }
}
