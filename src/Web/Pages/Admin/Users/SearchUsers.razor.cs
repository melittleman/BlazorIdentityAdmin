namespace BlazorAdminDashboard.Web.Pages.Admin.Users;

public partial class SearchUsers
{
    [Inject]
    public required UserManager<User> Manager { get; set; }

    [Inject]
    public required IDialogService Dialog { get; set; }

    [Inject]
    public required ISnackbar Snackbar { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }

    [Inject]
    public required IHostApplicationLifetime Lifetime { get; set; }

    private MudTable<User> usersTable = new();
    private readonly SearchFilter filter = new();
    private readonly int[] pageSizeOptions = [ 10, 25, 50, 100 ];
    private bool resetCurrentPage = false;

    private async Task<TableData<User>> GetUsersDataAsync(TableState state)
    {
        // Max of 32k pages
        filter.Page = (short)(resetCurrentPage ? 1 : (state.Page + 1));
        filter.Count = (byte)state.PageSize; // Max of 100 results per page
        resetCurrentPage = false;

        if (state.SortDirection is SortDirection.None || string.IsNullOrEmpty(state.SortLabel))
        {
            filter.OrderBy = null;
        }
        else
        {
            // TODO: Move this into an extension as it's going to be used a lot!
            filter.SortBy = state.SortDirection switch
            {
                SortDirection.Ascending => RedisKit.Querying.Enums.SortDirection.Ascending,
                SortDirection.Descending => RedisKit.Querying.Enums.SortDirection.Descending,
                _ => throw new NotImplementedException()
            };

            filter.OrderBy = state.SortLabel;            
        }

        // TODO: Change this to 'IPagedUserStore'?
        if (Manager is CustomUserManager custom)
        {
            // TODO: Wired up to the ApplicationStopping token at the moment, but I think
            // this makes more sense for a user to be able to cancel the search themselves.
            IPagedList<User> users = await custom.SearchUsersAsync(filter, Lifetime.ApplicationStopping);

            return new TableData<User>
            {
                Items = users,
                TotalItems = (int)users.TotalResults
            };
        }

        return new TableData<User>()
        {
            Items = Enumerable.Empty<User>(),
            TotalItems = 0
        };
    }

    /// <summary>
    ///     Refresh table data for the search query.
    /// </summary>
    /// <param name="text">The text to search for.</param>
    private async Task OnSearchAsync(string text)
    {
        filter.Query = null;

        if (string.IsNullOrEmpty(text) is false)
        {
            filter.Query = text;
        }

        resetCurrentPage = true;
        await usersTable.ReloadServerData();
    }

    /// <summary>
    ///     Show a dialog to add a new user.
    /// </summary>
    private async Task OpenNewUserDialogAsync()
    {
        IDialogReference dialog = await Dialog.ShowAsync<AddUserDialog>("Add New User", new DialogOptions()
        {
            CloseButton = true,
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Medium
        });

        DialogResult? result = await dialog.Result;

        if (result.Canceled is false)
        {
            await usersTable.ReloadServerData();
        }
    }

    /// <summary>
    ///     Navigate to the edit user page.
    /// </summary>
    private void EditUser(User userDetails)
    {
        Navigation.NavigateTo($"/admin/users/edit/{userDetails.Id}");
    }

    /// <summary>
    ///     Soft delete user from the application.
    /// </summary>
    /// <param name="user">The user that will be 'soft' deleted.</param>
    /// <returns></returns>
    private async Task DeleteUserAsync(User user)
    {
        bool? dialogMessage = await Dialog.ShowMessageBox(
            "Warning",
            $"Are you sure you want to delete {user.UserName}?",
            yesText: "Confirm",
            cancelText: "Cancel",
            options: new DialogOptions() 
            {
                FullWidth = true 
            });

        if (dialogMessage.GetValueOrDefault())
        {
            // TODO: Override this method in the custom
            // manager and change to a 'soft' delete.
            IdentityResult result = await Manager.DeleteAsync(user);

            if (result.Succeeded)
            {
                Snackbar.Add("Deleted successfully.", Severity.Success);

                await usersTable.ReloadServerData();
            }
            else
            {
                Snackbar.Add(result.ToString(), Severity.Error, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = true;
                });
            }
        }
    }
}
