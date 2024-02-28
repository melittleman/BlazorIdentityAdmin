namespace BlazorAdminDashboard.Web.Models;

internal sealed record LoginRecoveryCodeFormInput
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Recovery Code")]
    public string RecoveryCode { get; set; } = string.Empty;   
}
