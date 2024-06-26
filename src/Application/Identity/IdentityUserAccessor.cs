namespace BlazorIdentityAdmin.Application.Identity;

public sealed class IdentityUserAccessor(UserManager<User> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<User> GetRequiredUserAsync(HttpContext context)
    {
        User? user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("/invalid-user", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }

    public async Task<User> GetRequiredUserAsync(AuthenticationState state)
    {
        User? user = await userManager.GetUserAsync(state.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus(
                "/invalid-user", $"Error: Unable to load user with ID '{userManager.GetUserId(state.User)}'.",
                null!, 
                forceLoad: true);
        }

        return user;
    }
}
