namespace BlazorAdminDashboard.Application.Identity;

public sealed class CustomUserManager(
    IUserStore<User> store,
    IPagedUserStore<User> pagedStore,
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
    public Task<IPagedList<User>> SearchUsersAsync(SearchFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        
        // TODO: Can we pass in a cancellation token here?

        return pagedStore.SearchUsersAsync(filter);
    }
}
