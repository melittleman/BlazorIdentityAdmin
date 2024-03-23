namespace BlazorIdentityAdmin.Web.Models;

public sealed class EditProfileModel
{
    [Required]
    [Display(Name = "Username")]
    public string? Username { get; set; }

    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Display(Name = "Language / Locale")]
    public string? Culture { get; set; }

    [Display(Name = "Time Zone")]
    public string? TimeZone { get; set; }
}
