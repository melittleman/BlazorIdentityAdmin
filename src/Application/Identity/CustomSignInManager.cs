namespace BlazorAdminDashboard.Application.Identity;

public sealed class CustomSignInManager(
    UserManager<User> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<User> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<SignInManager<User>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<User> confirmation,
    IDeviceEmailSender deviceEmail) : SignInManager<User>(
        userManager,
        contextAccessor,
        claimsFactory,
        optionsAccessor,
        logger,
        schemes,
        confirmation)
{
    private readonly IDeviceEmailSender _deviceEmail = deviceEmail;
    private readonly IAuthenticationSchemeProvider _schemes = schemes;

    public async Task<SignInResult> PasswordSignInDeviceAsync(
        User user,
        Device device,
        bool isPersistent)
    {
        if (await IsTwoFactorRequiredAsync(user)) return SignInResult.TwoFactorRequired;

        // Need to make sure IsNewDevice executes first, as this is
        // what adds or updates the 'devices' array for the user.
        if (await IsNewDeviceAsync(user, device) && user.Email is not null)
        {
            // Using the forgot password URL here instead of the reset page, because resetting requires
            // a unique code to be created up front which may not actually end up getting used.
            string resetPasswordUrl = $"{Context.Request.Scheme}://{Context.Request.Host.Value}/Account/ForgotPassword";

            bool isSuccess = await _deviceEmail.SendNewDeviceEmailAsync(user, user.Email, device, resetPasswordUrl);
            if (isSuccess is false)
            {
                // TODO: What should we do here, continue to allow the sign in?
                // Maybe just add this as a notification for the user instead via some UI.
            }
        }

        AuthenticationProperties props = new()
        {
            IsPersistent = isPersistent,
            AllowRefresh = true,
        };

        long authTime = device.AccessedAt.ToUnixTimeSeconds();

        // TODO: Internally this still uses a BitConverter, but I think
        // the newer Convert.ToHexString() method is more efficient?
        string sessionId = CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex);

        // Note too sure if we actually need / want the session id in both the claims principal and the authentication
        // properties, but could come in handy down the line so leaving it in for now until we find a reason not to.
        props.SetString("session_id", sessionId);
        props.SetString("device.fingerprint", device.Fingerprint);
        props.SetString("device.os", device.OperatingSystem);
        props.SetString("device.browser", device.Browser);
        props.SetString("device.ip", device.IpAddress);
        props.SetString("device.location", device.Location);

        // TODO: We want the ".last_activity" property set, but I think this can be handled directly in the TicketStore...

        Claim[] claims = 
        [
            new Claim(JwtClaimTypes.Locale, user.CultureName),
            new Claim(JwtClaimTypes.ZoneInfo, user.TimezoneId),
            new Claim(JwtClaimTypes.IdentityProvider, "local"), // TODO: Constant?
            new Claim(JwtClaimTypes.SessionId, sessionId),
            new Claim(JwtClaimTypes.AuthenticationMethod, OidcConstants.AuthenticationMethods.Password),
            new Claim(JwtClaimTypes.AuthenticationTime, authTime.ToString(), ClaimValueTypes.Integer64)
        ];

        await SignInWithClaimsAsync(user, props, claims);
        return SignInResult.Success;
    }

    private async Task<bool> IsTwoFactorRequiredAsync(User user)
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

            return true;
        }

        return false;
    }

    private async Task<bool> IsNewDeviceAsync(User user, Device device)
    {
        if (UserManager is CustomUserManager custom)
        {
            (bool isAdded, bool _) = await custom.AddOrUpdateDeviceAsync(user, device);

            return isAdded;
        }

        return false;
    }
}
