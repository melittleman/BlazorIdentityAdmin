using BlazorAdminDashboard.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlazorAdminDashboard.Application.Identity;

public sealed class CustomClaimsPrincipalFactory(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<User, Role>(userManager, roleManager, options)
{
    // TODO: We should override 'CreateAsync' here to define what we want the ClaimsPrincipal to look like...
}
