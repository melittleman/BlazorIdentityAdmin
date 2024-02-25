namespace BlazorAdminDashboard.Web.Components.Admin;

public partial class AddUserDialog : ComponentBase
{
    private bool isCreating = false;
    private readonly RegisterFormInput model = new();

    [CascadingParameter]
    public required MudDialogInstance Dialog { get; set; }

    [Inject]
    public required UserManager<User> Manager { get; set; }

    [Inject]
    public required IUserStore<User> Store { get; set; }

    [Inject]
    public required IEmailSender<User> Email { get; set; }

    [Inject]
    public required ISnackbar Snackbar { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }

    [Inject]
    public required ILogger<AddUserDialog> Logger { get; set; }

    [Inject]
    public required IHostApplicationLifetime Lifetime { get; set; }

    /// <summary>
    ///     Register a new user account with the application.
    /// </summary>
    /// <returns></returns>
    public async Task AddUserAsync()
    {
        isCreating = true;

        // TODO: There is a lot of duplication in this method with the 'Register.razor'
        // component so we can probably look to unify some of this logic.
        User user = new();

        // TODO: I really hate these these 2 operations happening separately and that we
        // have to cast between the store implementations... This is the biggest downside
        // with the built in MS Identity, so I think will be the motivation to move away.
        await Store.SetUserNameAsync(user, model.Email, Lifetime.ApplicationStopping);

        if (Store is IUserEmailStore<User> emailStore)
        {
            await emailStore.SetEmailAsync(user, model.Email, Lifetime.ApplicationStopping);
        }

        IdentityResult result = await Manager.CreateAsync(user, model.Password);

        if (result.Succeeded is false)
        {
            Snackbar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });

            isCreating = false;
            Dialog.Close(false);
            return;
        }

        Logger.LogInformation("User created a new account with password.");

        string userId = await Manager.GetUserIdAsync(user);
        string code = await Manager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        string callbackUrl = Navigation.GetUriWithQueryParameters(
            Navigation.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?>
            {
                ["userId"] = userId,
                ["code"] = code,
                ["returnUrl"] = "/" // TODO: Is this even needed?
            });


        await Email.SendConfirmationLinkAsync(user, model.Email, HtmlEncoder.Default.Encode(callbackUrl));

        Snackbar.Add("Used created successfully.", Severity.Success);

        isCreating = false;
        Dialog.Close(true);
    }
}
