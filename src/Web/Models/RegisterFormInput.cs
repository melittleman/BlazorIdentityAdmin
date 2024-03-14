namespace BlazorIdentityAdmin.Web.Models;

internal sealed record RegisterFormInput
{
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required]
    [StringLength(64, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
