namespace BlazorIdentityAdmin.Web.Models;

internal sealed record ResetPasswordFormInput
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    // TODO: Move the 'Minimum' length value into a constant.
    [Required]
    [StringLength(64, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}
