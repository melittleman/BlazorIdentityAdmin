using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using NRedisKit;
using NRedisKit.DependencyInjection.Abstractions;

using BlazorAdminDashboard.Domain.Identity;
using BlazorAdminDashboard.Domain.Documents.v1;

namespace BlazorAdminDashboard.Infrastructure.Stores;

public class RedisUserStore(
    IRedisClientFactory redisFactory,
    IdentityErrorDescriber errorDescriber) : UserStoreBase<User, Ulid, UserClaim, UserLogin, UserToken>(errorDescriber), IUserSecurityStampStore<User>
{
    // TODO: Constant Redis client name
    private readonly RedisClient _redis = redisFactory.CreateClient("persistent-db");

    // I don't believe this is actually required for any of the .NET Identity boilerplate.
    // However, because this is an abstract property, we really have no choice but to override.
    public override IQueryable<User> Users => throw new NotImplementedException();

    public override Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task AddLoginAsync(User user, UserLoginInfo info, CancellationToken cancellationToken = default)
    {
        // TODO: Create a 'Document'.
        UserLogin login = new()
        {
            UserId = user.Id,
            ProviderKey = info.ProviderKey,
            LoginProvider = info.LoginProvider,
            ProviderDisplayName = info.ProviderDisplayName
        };

        if (await _redis.SetAsJsonAsync($"dashboard:users:{user.Id}", login, "$.logins[0]") is false)
        {
            // TODO: Log error

            throw new InvalidOperationException($"Unable to add login to User {user.Id}");
        }
    }

    public override async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        if (await _redis.SetAsJsonAsync($"dashboard:users:{user.Id}", (UserDocumentV1)user))
        {
            return IdentityResult.Success;
        }

        return new IdentityResult();
    }

    public override async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        if (await _redis.Db.KeyDeleteAsync($"dashboard:users:{user.Id}"))
        {
            return IdentityResult.Success;
        }

        return new IdentityResult();
    }

    public override async Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        // TODO: There has got to be a better way to achieve this... must be an existing library or helper?
        normalizedEmail = normalizedEmail.Replace(".", "\\.");
        normalizedEmail = normalizedEmail.Replace("@", "\\@");
        normalizedEmail = normalizedEmail.Replace("-", "\\-");

        // TODO: Constant for Redis index name
        // TODO: Why doesn't $ string interpolation seem to work for 'normalizedEmail'?
        UserDocumentV1? document = await _redis.SearchSingleAsync<UserDocumentV1>("idx:users", "@email:{" + normalizedEmail + "}");
        if (document is null) return null;

        // Note: This is where we would be doing any neccessary conversions between v1 and v2+ etc. of the document.

        // This 'could' be made to an implicit conversion to be slightly nicer on the eye...
        // but I have become a fan of more 'explicitness' where appropriate.
        return (User)document;
    }

    public override async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        UserDocumentV1? document = await _redis.GetFromJsonAsync<UserDocumentV1>($"dashboard:users:{userId}");

        if (document is null)
        {
            // TODO: Log error...
            return null;
        }

        return (User)document;
    }

    public override async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        // TODO: There has got to be a better way to achieve this... must be an existing library or helper?
        normalizedUserName = normalizedUserName.Replace(".", "\\.");
        normalizedUserName = normalizedUserName.Replace("@", "\\@");
        normalizedUserName = normalizedUserName.Replace("-", "\\-");

        UserDocumentV1? document = await _redis.SearchSingleAsync<UserDocumentV1>("idx:users", "@username:{" + normalizedUserName + "}");
        if (document is null) return null;

        // Note: This is where we would be doing any neccessary conversions between v1 and v2+ etc. of the document.

        // This 'could' be made to an implicit conversion to be slightly nicer on the eye...
        // but I have become a fan of more 'explicitness' where appropriate.
        return (User)document;
    }

    public override Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
    {
        IList<Claim> claims = new List<Claim>()
        {
            new("tenant", "123")
        };

        return Task.FromResult(claims);
    }

    public override Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        cancellationToken.ThrowIfCancellationRequested();

        if (await _redis.SetAsJsonAsync($"dashboard:users:{user.Id}", (UserDocumentV1)user))
        {
            return IdentityResult.Success;
        }

        return new IdentityResult();
    }

    protected override async Task AddUserTokenAsync(UserToken token)
    {
        ICollection<UserToken> tokens = new List<UserToken>() { token };

        await _redis.SetAsJsonAsync($"dashboard:users:{token.UserId}:tokens", tokens);
    }

    protected override Task<UserToken?> FindTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        return Task.FromResult((UserToken?)new UserToken()
        {
            UserId = user.Id,
            LoginProvider = loginProvider,
            Name = name,
            Value = "BPA47OU57W2BDCLOFBK5KPHJYFRH4IHD"
        });
    }

    protected override async Task<User?> FindUserAsync(Ulid userId, CancellationToken cancellationToken)
    {
        UserDocumentV1? document = await _redis.GetFromJsonAsync<UserDocumentV1>($"dashboard:users:{userId}");

        if (document is null) return null;

        return (User)document;
    }

    protected override Task<UserLogin?> FindUserLoginAsync(Ulid userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task<UserLogin?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task RemoveUserTokenAsync(UserToken token)
    {
        throw new NotImplementedException();
    }
}

