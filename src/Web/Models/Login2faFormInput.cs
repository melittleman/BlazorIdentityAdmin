namespace BlazorAdminDashboard.Web.Models;

internal sealed record Login2faFormInput
{
    [Required]
    [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Text)]
    [Display(Name = "Authenticator Code")]
    public string? TwoFactorCode { get; set; }

    [Display(Name = "Remember this device?")]
    public bool RememberDevice { get; set; }   
}
