namespace BlazorAdminDashboard.Web.Components.Account;

/// <summary>
/// Component to facilitate user account deletion
/// </summary>
public partial class DeleteAccount : ComponentBase
{
    //[Inject]
    //public IAccountService AccountService { get; set; }

    [Inject]
    public required ISnackbar SnackBar { get; set; }

    [Inject]
    public required  NavigationManager Navigator { get; set; }

    // TODO: Do we really need a model for this?
    internal DeleteAccountModel model = new ();

    /// <summary>
    /// Permantently delete user account
    /// </summary>
    /// <returns></returns>
    async Task DeleteAccountAsync()
    {
        //var result = await AccountService.DeleteAccountAsync(model);
        //if (result.IsSuccess)
        //{
        //    Navigator.NavigateToLogout("authentication/register");
        //    return;
        //}
        //SnackBar.Add(result.ToString(), Severity.Error, config =>
        //{
        //    config.ShowCloseIcon = true;
        //    config.RequireInteraction = true;
        //});
    }
}
