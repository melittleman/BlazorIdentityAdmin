namespace BlazorIdentityAdmin.Web.Components.Account;

/// <summary>
/// DisableAuthenticator component allows disabling the 2FA authentication for user account.
/// </summary>
public partial class DisableAuthenticator : ComponentBase
{
    DisableAuthenticatorViewModel model = new();

    [Inject]
    public ISnackbar SnackBar { get; set; }

    [Inject]
    public IDialogService DialogService { get; set; }

    [Inject]
    public NavigationManager Navigator { get; set; }

    [Inject]
    public UserManager<User> UserManager { get; set; }

    [Inject]
    public IdentityUserAccessor UserAccessor { get; set; }

    private User user = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        user = await UserAccessor.GetRequiredUserAsync(HttpContext);

        if (HttpMethods.IsGet(HttpContext.Request.Method) && !await UserManager.GetTwoFactorEnabledAsync(user))
        {
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");
        }
    }

    /// <summary>
    /// Disable 2FA for user account
    /// </summary>
    private async void DisableAuthenticatorAsync()
    {
        // Strip spaces and hyphens
        string? verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2faTokenValid = await UserManager.VerifyTwoFactorTokenAsync(user,
           UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        IdentityResult result = await UserManager.SetTwoFactorEnabledAsync(user, false);

        if (!result.Succeeded)
        {
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
            return;
        }

        await DialogService.ShowMessageBox("Success",
          "2FA is disabled now. You should enable 2FA for a better security of your account.");

        Navigator.NavigateTo("account/authenticator/enable");
    }

    private class DisableAuthenticatorViewModel
    {
        [Required(ErrorMessage = "Code is required to disable 2FA")]
        public string Code { get; set; } = string.Empty;
    }
}
