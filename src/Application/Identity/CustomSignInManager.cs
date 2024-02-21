namespace BlazorAdminDashboard.Application.Identity;

public sealed class CustomSignInManager(
    UserManager<User> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<User> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<SignInManager<User>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<User> confirmation) : SignInManager<User>(
        userManager,
        contextAccessor,
        claimsFactory,
        optionsAccessor,
        logger,
        schemes,
        confirmation)
{
    private readonly IAuthenticationSchemeProvider _schemes = schemes;

    public async Task<SignInResult> PasswordSignInDeviceAsync(
        User user,
        Device device,
        bool isPersistent)
    {
        // TODO: Investigate if there's a better way to sort this. I hate that we have to 
        // implement our own two factor logic here just because there's no better method to
        // override for us to do our password signin with custom properties etc.

        if (await IsTwoFactorEnabledAsync(user) && await IsTwoFactorClientRememberedAsync(user) is false)
        {
            if (await _schemes.GetSchemeAsync(IdentityConstants.TwoFactorUserIdScheme) != null)
            {
                // Store the userId for use after two factor check
                string userId = await UserManager.GetUserIdAsync(user);

                ClaimsIdentity identity = new(IdentityConstants.TwoFactorUserIdScheme);

                identity.AddClaim(new Claim(ClaimTypes.Name, userId));

                await Context.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, new ClaimsPrincipal(identity));
            }

            return SignInResult.TwoFactorRequired;
        }

        AuthenticationProperties props = new()
        {
            IsPersistent = isPersistent,
            AllowRefresh = true,
        };

        long authTime = device.AccessedAt.ToUnixTimeSeconds();
        string sessionId = CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex);

        // Note too sure if we actually need / want the session id in both the claims principal and the authentication
        // properties, but could come in handy down the line so leaving it in for now until we find a reason not to.
        props.SetString("session_id", sessionId);
        props.SetString("device.name", device.Name);
        props.SetString("device.location", device.Location);

        Claim[] claims = 
        [
            // TODO: Potentially more to add including 'locale' and 'zoneinfo' ?

            new Claim(JwtClaimTypes.IdentityProvider, "local"), // TODO: Should this really be a fully qualified URI?
            new Claim(JwtClaimTypes.SessionId, sessionId),
            new Claim(JwtClaimTypes.AuthenticationMethod, "pwd"),
            new Claim(JwtClaimTypes.AuthenticationTime, authTime.ToString(), ClaimValueTypes.Integer64)
        ];

        await SignInWithClaimsAsync(user, props, claims);
        return SignInResult.Success;
    }
}
