namespace BlazorAdminDashboard.Web.Models;

internal sealed record ExternalLoginFormInput
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public string? Picture { get; set; }

    public string? Locale { get; set; }
}
