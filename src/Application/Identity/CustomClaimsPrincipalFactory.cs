using OpenIddict.Abstractions;

namespace BlazorAdminDashboard.Application.Identity;

public sealed class CustomClaimsPrincipalFactory(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<User, Role>(userManager, roleManager, options)
{
    // TODO: We should override 'CreateAsync' here to define what we want the ClaimsPrincipal to look like...

    public override async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        ClaimsIdentity id = new(
            IdentityConstants.ApplicationScheme,
            Options.ClaimsIdentity.UserNameClaimType,
            Options.ClaimsIdentity.RoleClaimType);

        string userId = await UserManager.GetUserIdAsync(user);

        id.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, userId));

        string? userName = await UserManager.GetUserNameAsync(user);

        if (string.IsNullOrEmpty(userName) is false) 
        {
            id.AddClaim(new Claim(OpenIddictConstants.Claims.PreferredUsername, userName));
        }

        if (UserManager.SupportsUserEmail)
        {
            string? email = await UserManager.GetEmailAsync(user);
            if (string.IsNullOrEmpty(email) is false)
            {
                id.AddClaim(new Claim(Options.ClaimsIdentity.EmailClaimType, email));
            }
        }

        // TODO: I don't think this is needed and would allow us to remove the IUserClaimStore implementation
        // from the RedisUserStore which would be better than storing the individual claims themselves.
        if (UserManager.SupportsUserClaim)
        {
            id.AddClaims(await UserManager.GetClaimsAsync(user));
        }

        if (UserManager.SupportsUserSecurityStamp)
        {
            string securityStamp = await UserManager.GetSecurityStampAsync(user);
            id.AddClaim(new Claim(Options.ClaimsIdentity.SecurityStampClaimType, securityStamp));
        }

        if (UserManager.SupportsUserRole)
        {
            IList<string> roles = await UserManager.GetRolesAsync(user);

            foreach (string roleName in roles)
            {
                id.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, roleName));

                if (RoleManager.SupportsRoleClaims)
                {
                    Role? role = await RoleManager.FindByNameAsync(roleName);
                    if (role is not null)
                    {
                        id.AddClaims(await RoleManager.GetClaimsAsync(role));
                    }
                }
            }
        }

        return new ClaimsPrincipal(id);
    }
}
