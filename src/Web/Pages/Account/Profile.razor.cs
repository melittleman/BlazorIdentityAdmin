namespace BlazorAdminDashboard.Web.Pages.Account;

/// <summary>
/// Component to manage profile
/// </summary>
public partial class Profile : ComponentBase
{
    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> authenticationStateTask { get; set; }
 
    [Inject]
    public required ISnackbar SnackBar { get; set; }

    [Inject]
    public required IdentityUserAccessor UserAccessor { get; set; }

    //[Inject]
    //public IAccountService AccountService { get; set; }

    //[Inject]
    //public IUsersService UsersService { get; set; }

    //UserDetailsViewModel user;

    private static User? user;

    // TODO: Do we really need a model for this?
    private ChangeEmailModel changeEmailModel = new();

    bool isEditingEmail = false;

    /// <summary>
    /// Get the user details for logged in user and show user details.
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            //var authState = await authenticationStateTask;

            if (HttpContext is not null)
            {
                user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            }


            //user =  await UsersService.GetUserByNameAsync(authState.User.Identity.Name);
            changeEmailModel.NewEmail = user.Email;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackBar.Add(ex.Message, Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });               
        }
        await base.OnInitializedAsync();
    }

    /// <summary>
    /// Toggle the visibility of Edit Email form
    /// </summary>
    protected void ToggleEditEmail()
    {
        isEditingEmail = !isEditingEmail;
    }

    /// <summary>
    /// Sends a verification mail to user to verify new email.
    /// Email and UserName will be updaated once new email is verified.
    /// </summary>
    /// <returns></returns>
    protected async Task ChangeEmailAsync()
    {
        //var result = await AccountService.ChangeEmailAsync(changeEmailModel);
        //if (result.IsSuccess)
        //{
        //    SnackBar.Add("Verification email sent. Email will be updated once verified.", Severity.Success);               
        //    return;
        //}
        //SnackBar.Add(result.ToString(), Severity.Error, config =>
        //{
        //    config.ShowCloseIcon = true;
        //    config.RequireInteraction = true;
        //});
    }
}
