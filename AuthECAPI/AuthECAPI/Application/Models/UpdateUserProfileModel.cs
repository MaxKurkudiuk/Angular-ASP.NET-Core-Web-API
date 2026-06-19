namespace AuthECAPI.Application.Models;

public class UpdateUserProfileModel
{
    public string FullName { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public int Age { get; set; }
    public int? LibraryID { get; set; }
}
