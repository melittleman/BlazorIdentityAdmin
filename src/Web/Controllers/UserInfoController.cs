using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

using BlazorAdminDashboard.Domain.Identity;

namespace BlazorAdminDashboard.Web.Controllers;

[ApiController]
[Route("connect/userinfo")]
[Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
public class UserInfoController(UserManager<User> manager) : ControllerBase
{
    private readonly UserManager<User> _manager = manager;

    [HttpGet, HttpPost]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    public async Task<IActionResult> Userinfo()
    {
        User? user = await _manager.GetUserAsync(User);

        if (user is null)
        {
            return Challenge
            (
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified access token is bound to an account that no longer exists."
                })
            );
        }

        Dictionary<string, object> claims = new(StringComparer.Ordinal)
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            [OpenIddictConstants.Claims.Subject] = await _manager.GetUserIdAsync(user)
        };

        if (User.HasScope(OpenIddictConstants.Scopes.Email))
        {
            string? email = await _manager.GetEmailAsync(user);
            if (string.IsNullOrWhiteSpace(email) is false)
            {
                claims[OpenIddictConstants.Claims.Email] = email;
                claims[OpenIddictConstants.Claims.EmailVerified] = await _manager.IsEmailConfirmedAsync(user);
            }
        }

        if (User.HasScope(OpenIddictConstants.Scopes.Phone))
        {
            string? number = await _manager.GetPhoneNumberAsync(user);

            if (string.IsNullOrWhiteSpace(number) is false)
            {
                claims[OpenIddictConstants.Claims.PhoneNumber] = number;
                claims[OpenIddictConstants.Claims.PhoneNumberVerified] = await _manager.IsPhoneNumberConfirmedAsync(user);
            }
        }

        if (User.HasScope(OpenIddictConstants.Scopes.Roles))
        {
            claims[OpenIddictConstants.Claims.Role] = await _manager.GetRolesAsync(user);
        }

        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

        return Ok(claims);
    }
}
