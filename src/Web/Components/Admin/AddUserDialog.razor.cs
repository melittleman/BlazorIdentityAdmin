using Microsoft.AspNetCore.Components;
using MudBlazor;

using BlazorAdminDashboard.Domain.Identity;
using BlazorAdminDashboard.Web.Models;

namespace BlazorAdminDashboard.Web.Components.Admin;

/// <summary>
/// Dialog to create a new user
/// </summary>
public partial class AddUserDialog : ComponentBase
{
    private readonly RegisterInputModel model = new();

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }

    [Inject]
    UserManager<User> UsersService { get; set; }

    [Inject]
    public ISnackbar SnackBar { get; set; }

    /// <summary>
    /// Register a new user account with the application
    /// </summary>
    /// <returns></returns>
    public async Task AddUserAsync()
    {
        var user = new User();
        //var user = CreateUser();

        //await userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
        //await emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);


        //var result = await userManager.CreateAsync(user, model.Password);

        //if (result.Succeeded)
        //{
        //    logger.LogInformation("User created a new account with password.");

        //    var userId = await userManager.GetUserIdAsync(user);
        //    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        //    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //    var callbackUrl = Url.Page(
        //        "/Account/ConfirmEmail",
        //        pageHandler: null,
        //        values: new { area = "Identity", userId = userId, code = code, returnUrl = Url.Content("~/") },
        //        protocol: Request.Scheme);

        //    await emailSender.SendEmailAsync(model.Email, "Confirm your email",
        //        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        //    return Ok();


        var result = await UsersService.CreateAsync(user);

        if (result.Succeeded)
        {
            SnackBar.Add("Used created successfully.", Severity.Success);
            MudDialog.Close(true);
            return;
        }

        SnackBar.Add(result.ToString(), Severity.Error, config =>
        {
            config.ShowCloseIcon = true;
            config.RequireInteraction = true;
        });
        MudDialog.Close(false);
    }
}
