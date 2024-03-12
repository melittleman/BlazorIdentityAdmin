namespace BlazorAdminDashboard.Web.Models;

internal sealed record ExternalLoginFormInput
{
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    // Not visible in UI
    public string? Picture { get; set; }

    // Not visible in UI
    public string? Locale { get; set; }
}
