namespace BlazorAdminDashboard.Web.Models;

internal sealed record ResendEmailFormInput
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;   
}
