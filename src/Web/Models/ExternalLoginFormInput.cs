namespace BlazorAdminDashboard.Web.Models;

internal sealed record ExternalLoginFormInput
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
