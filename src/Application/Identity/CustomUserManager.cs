namespace BlazorIdentityAdmin.Application.Identity;

public sealed class CustomUserManager(
    IUserStore<User> store,
    IPagedUserStore pagedStore,
    IUserDeviceStore deviceStore,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<User> passwordHasher,
    IEnumerable<IUserValidator<User>> userValidators,
    IEnumerable<IPasswordValidator<User>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<User>> logger) : UserManager<User>(
        store,
        optionsAccessor,
        passwordHasher,
        userValidators,
        passwordValidators,
        keyNormalizer,
        errors,
        services,
        logger)
{
    private readonly IUserDeviceStore _devices = deviceStore;

    public Task<IPagedList<User>> SearchUsersAsync(SearchFilter filter, CancellationToken? ct = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        ct?.ThrowIfCancellationRequested();

        return pagedStore.SearchUsersAsync(filter, ct);
    }

    public async Task<(bool isAdded, bool isUpdated)> AddOrUpdateDeviceAsync(User user, Device device, CancellationToken? ct = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(device);

        ct?.ThrowIfCancellationRequested();

        // TODO: Need to work out what role IP address plays here, do we actually want to send an email to the user
        // when they log in from the same IP? Or should it only be if both the IP and fingerprint have changed?
        Device? existingDevice = await _devices.FindByFingerprintAsync(user, device.Fingerprint, ct);

        bool isAdded = existingDevice is null;

        bool isUpdated = await _devices.AddOrUpdateDeviceAsync(user, device, ct);

        return (isAdded, isUpdated);
    }

    public async Task<IdentityResult> ValidateAsync(User user, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        // This is pretty sucky to be honest, because the 'UpdateSecurityStamp' method on UserManager is
        // private it's impossible to have our own validation method without finding a way to set the stamp.
        if (Store is IUserSecurityStampStore<User> security)
        {
            await security.SetSecurityStampAsync(user, GenerateNewAuthenticatorKey(), ct);
        }

        return await ValidateUserAsync(user);
    }
}
