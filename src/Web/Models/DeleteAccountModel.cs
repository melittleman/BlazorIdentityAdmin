namespace BlazorAdminDashboard.Web.Models;

internal sealed record DeleteAccountModel
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
