namespace BlazorIdentityAdmin.Web.Components.Account;

/// <summary>
/// ResetAuthenticator component allows user to reset the authenticator key.
/// 2FA is disabled after this operation and user needs to re-enable 2FA and
/// verify his authenticator app.
/// </summary>
public partial class ResetAuthenticator : ComponentBase
{
    ResetAuthenticatorViewModel model = new();

    [Inject]
    public ISnackbar SnackBar { get; set; }

    [Inject]
    public IDialogService DialogService { get; set; }

    [Inject]
    public NavigationManager Navigator { get; set; }

    public UserManager<User> UserManager { get; set; }

    [Inject]
    public IdentityUserAccessor UserAccessor { get; set; }

    [Inject]
    public IdentityRedirectManager RedirectManager { get; set; }

    [Inject]
    public SignInManager<User> SignInManager { get; set; }

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


    // Taken from template page...
    private async Task OnSubmitAsync()
    {
        var user = await UserAccessor.GetRequiredUserAsync(HttpContext);
        await UserManager.SetTwoFactorEnabledAsync(user, false);
        await UserManager.ResetAuthenticatorKeyAsync(user);
        var userId = await UserManager.GetUserIdAsync(user);
        //Logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", userId);

        await SignInManager.RefreshSignInAsync(user);

        RedirectManager.RedirectToWithStatus(
            "/account/enable-authenticator",
            "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.",
            HttpContext);
    }

    /// <summary>
    /// Disable 2FA for user account and reset Authenticator key.      
    /// </summary>
    private async void ResetAuthenticatorAsync()
    {

        // Strip spaces and hyphens
        string? verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        var is2faTokenValid = await UserManager.VerifyTwoFactorTokenAsync(user,
           UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        await UserManager.SetTwoFactorEnabledAsync(user, false);
        await UserManager.ResetAuthenticatorKeyAsync(user);
        var userId = await UserManager.GetUserIdAsync(user);
        await SignInManager.RefreshSignInAsync(user);

        if (!is2faTokenValid)
        {
            SnackBar.Add("Uh oh...", Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
            return;
        }
        await DialogService.ShowMessageBox("Success",
            "Authenticator is reset now. You need to re-configure authenticator again to enable 2FA!");
      
        Navigator.NavigateTo("account/authenticator/enable");
    }

    private class ResetAuthenticatorViewModel
    {
        [Required(ErrorMessage = "Code is required to reset Authenticator")]
        public string Code { get; set; } = string.Empty;
    }
}
