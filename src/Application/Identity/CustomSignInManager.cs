namespace BlazorIdentityAdmin.Application.Identity;

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

    public async Task<SignInResult> SignInDeviceOrTwoFactorAsync(
        User user, 
        Device device, 
        AuthenticationProperties props, 
        string? loginProvider = null, 
        bool bypassTwoFactor = false)
    {
        if (bypassTwoFactor is false && await IsTwoFactorRequiredAsync(user, loginProvider))
        {
            return SignInResult.TwoFactorRequired;
        }

        await ValidateDeviceAsync(user, device);
        SetDeviceProperties(props, device);

        string sessionId = CreateSessionId(props);
        ICollection<Claim> claims;

        if (loginProvider is not null)
        {
            // Cleanup external cookie
            await Context.SignOutAsync(IdentityConstants.ExternalScheme);

            claims = GetAdditionalClaims(sessionId, loginProvider);
        }
        else
        {
            claims = GetAdditionalClaims(sessionId);
        }

        await SignInWithClaimsAsync(user, props, claims);
        return SignInResult.Success;
    }

    public Task<SignInResult?> CheckSignInAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return PreSignInCheck(user);
    }

    private async Task<bool> IsTwoFactorRequiredAsync(User user, string? loginProvider = null)
    {
        if (await IsTwoFactorEnabledAsync(user) is false) return false;

        if (await IsTwoFactorClientRememberedAsync(user)) return false;

        if (await _schemes.GetSchemeAsync(IdentityConstants.TwoFactorUserIdScheme) != null)
        {
            // Store the userId for use after two factor check
            string userId = await UserManager.GetUserIdAsync(user);

            ClaimsIdentity identity = new(IdentityConstants.TwoFactorUserIdScheme);

            identity.AddClaim(new Claim(JwtClaimTypes.Subject, userId));
            if (loginProvider != null)
            {
                identity.AddClaim(new Claim(JwtClaimTypes.AuthenticationMethod, loginProvider));
            }

            await Context.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, new ClaimsPrincipal(identity));
        }

        return true;
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

    private async Task ValidateDeviceAsync(User user, Device device)
    {
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
    }

    private static string CreateSessionId(AuthenticationProperties props)
    {
        // TODO: Internally this still uses a BitConverter, but I think
        // the newer Convert.ToHexString() method is more efficient?
        string sessionId = CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex);

        // Note too sure if we actually need / want the session id in both the claims principal and the authentication
        // properties, but could come in handy down the line so leaving it in for now until we find a reason not to.
        props.SetString("session_id", sessionId);

        return sessionId;
    }

    private ICollection<Claim> GetAdditionalClaims(
        string sessionId,
        string authenticationMethod = OidcConstants.AuthenticationMethods.Password)
    {
        long authTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        ICollection<Claim> claims =
        [
            new Claim(JwtClaimTypes.IdentityProvider, "local"), // TODO: Constant?
            new Claim(JwtClaimTypes.SessionId, sessionId),
            new Claim(JwtClaimTypes.AuthenticationMethod, authenticationMethod),
            new Claim(JwtClaimTypes.AuthenticationTime, authTime.ToString(), ClaimValueTypes.Integer64)
        ];

        return claims;
    }

    private static void SetDeviceProperties(AuthenticationProperties props, Device device)
    {
        props.SetString("device.fingerprint", device.Fingerprint);
        props.SetString("device.os", device.OperatingSystem);
        props.SetString("device.browser", device.Browser);
        props.SetString("device.ip", device.IpAddress);
        props.SetString("device.location", device.Location);
    }
}
