﻿using BlazorAdminDashboard.Application.Identity;
using BlazorAdminDashboard.Domain.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using RedisKit.Querying;
using RedisKit.Querying.Abstractions;

namespace BlazorAdminDashboard.Web.Pages.Admin.Users;

public partial class SearchUsers
{
    //[Inject]
    //public IUsersService Service { get; set; }

    [Inject]
    public required UserManager<User> Manager { get; set; }

    [Inject]
    public IDialogService DialogService { get; set; }

    [Inject]
    public ISnackbar SnackBar { get; set; }

    [Inject]
    public NavigationManager Navigator { get; set; }

    private MudTable<User> usersTable;


    //private GetUsersRequest usersRequest = new GetUsersRequest();
    private SearchFilter filter = new();



    private readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };
    private bool resetCurrentPage = false;

    /// <summary>
    /// Get roles from api endpoint for the current page of the data table
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private async Task<TableData<User>> GetUsersDataAsync(TableState state)
    {
        // Max of 32k pages
        filter.Page = (short)(resetCurrentPage ? 1 : (state.Page + 1));
        resetCurrentPage = false;

        // Max of 100 results per page
        filter.Count = (byte)state.PageSize;

        if (Manager is CustomUserManager custom)
        {
            IPagedList<User> users = await custom.SearchUsersAsync(filter);

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
    /// Refresh data for the search query
    /// </summary>
    /// <param name="text"></param>
    private void OnSearch(string text)
    {
        //filter.UsersFilter = string.Empty;
        //if (!string.IsNullOrEmpty(text))
        //{
        //    usersRequest.UsersFilter = text;
        //}
        //resetCurrentPage = true;
        //usersTable.ReloadServerData();
    }

    /// <summary>
    /// Show a dialog to add new users
    /// </summary>
    async Task OpenNewUserDialogAsync()
    {
        //var dialogReference = await DialogService.ShowAsync<AddUserDialog>("Add New User", new DialogOptions()
        //{
        //    CloseButton = true,
        //    CloseOnEscapeKey = true,
        //    MaxWidth = MaxWidth.Medium
        //});
        //var dialogResult = await dialogReference.Result;
        //if (!dialogResult.Canceled)
        //{
        //    await usersTable.ReloadServerData();
        //}
    }

    /// <summary>
    /// Navigate to the edit user page
    /// </summary>
    void EditUser(User userDetails)
    {
        Navigator.NavigateTo($"users/edit/{userDetails.Id}");
    }

    /// <summary>
    /// Permanently delete user from system. 
    /// </summary>
    /// <param name="userDetails"></param>
    /// <returns></returns>
    async Task DeleteUserAsync(User userDetails)
    {
        //bool? dialogResult = await DialogService.ShowMessageBox("Warning", "Delete can't be undone !!",
        //    yesText: "Delete!", cancelText: "Cancel", options: new DialogOptions() { FullWidth = true });
        //if (dialogResult.GetValueOrDefault())
        //{
        //    var result = await Service.DeleteUserAsync(userDetails);
        //    if (result.IsSuccess)
        //    {
        //        SnackBar.Add("Deleted successfully.", Severity.Success);
        //        await usersTable.ReloadServerData();
        //        return;
        //    }
        //    SnackBar.Add(result.ToString(), Severity.Error, config =>
        //    {
        //        config.ShowCloseIcon = true;
        //        config.RequireInteraction = true;
        //    });
        //}
    }
}
