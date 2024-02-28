namespace BlazorAdminDashboard.Web.Models;

internal sealed record ForgotPasswordFormInput
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
