namespace BlazorAdminDashboard.Web.Models;

internal sealed record ExternalLoginFormInput
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Picture { get; set; }

    public string? Locale { get; set; }
}
