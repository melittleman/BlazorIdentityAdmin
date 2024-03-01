namespace BlazorAdminDashboard.Web.Models;

internal sealed record LoginFormInput
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public string RememberMe { get; set; } = "off";

    // This is a bit of a hack because currently the MudCheckbox
    // component defaults to a string as it's generic T Value.
    // We cannot change it because we're rendering in a Static SSR
    // mode for the Login page, so hopefully this will be resolved
    // once MudBlazor is fully compatible with .NET 8.
    public bool IsRememberMeEnabled => RememberMe is "on";
}
