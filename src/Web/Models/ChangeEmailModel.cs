namespace BlazorAdminDashboard.Web.Models;

internal sealed record ChangeEmailModel
{
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; } = string.Empty;
}
