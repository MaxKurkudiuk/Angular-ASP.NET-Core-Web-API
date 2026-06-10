using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthECAPI.Models;

public class AppUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName ="nvarchar(150)")]
    public string FullName { get; set; } = null!;

    [PersonalData]
    [Column(TypeName = "nvarchar(10)")]
    public string Gender { get; set; } = null!;

    [PersonalData]
    public DateOnly DOB { get; set; }   // Day of birth

    [PersonalData]
    public int? LibraryID { get; set; }
}
