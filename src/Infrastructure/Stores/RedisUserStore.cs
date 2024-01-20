using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using NRedisKit;
using NRedisKit.DependencyInjection.Abstractions;

using BlazorAdminDashboard.Domain.Identity;
using BlazorAdminDashboard.Domain.Documents.v1;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BlazorAdminDashboard.Infrastructure.Stores;

public class RedisUserStore(
    IRedisClientFactory redisFactory,
    IdentityErrorDescriber errorDescriber) : UserStoreBase<User, Ulid, UserClaim, UserLogin, UserToken>(errorDescriber)
{
    // TODO: Constant Redis client name
    private readonly RedisClient _redis = redisFactory.CreateClient("persistent-db");

    // I don't believe this is actually required for any of the .NET Identity boilerplate.
    // However, because this is an abstract property, we really have no choice but to override.
    public override IQueryable<User> Users => throw new NotImplementedException();

    // TODO: Sort these out into proper regions or even a partial class?
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

    protected override Task<User?> FindUserAsync(Ulid userId, CancellationToken cancellationToken)
    {
        return FindByIdAsync(userId.ToString(), cancellationToken);
    }

    // TODO: Why on earth do they make us implement both of these?
    public override async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        UserDocumentV1? document = await _redis.GetFromJsonAsync<UserDocumentV1>($"dashboard:users:{userId}");

        if (document is null)
        {
            // TODO: Log error...
            return null;
        }

        return (User?)document;
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

        // TODO: Can we pass in the cancellation token?
        // TODO: This is too dangerous, as we could be overwriting unexpected properties
        // because 'User' does not contain everything that the 'Document' needs...
        // Need to first read it out of the database, and update each one by one.
        if (await _redis.SetAsJsonAsync($"dashboard:users:{user.Id}", (UserDocumentV1)user))
        {
            return IdentityResult.Success;
        }

        return new IdentityResult();
    }

    protected override async Task AddUserTokenAsync(UserToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        if (token.Value is null)
        {
            // Log error...
            return;
        }

        string key = $"dashboard:users:{token.UserId}";

        // TODO: This could be further simplified if we store an empty array as default in the document, then we can use
        // the JSON.ARRAPPEND command to add to this existing array without the need to first read it from the database.
        IEnumerable<MultiFactorTokenDocumentV1?> tokens = await _redis.Json.GetEnumerableAsync<MultiFactorTokenDocumentV1>(key, "$.mfa_tokens[*]");

        tokens ??= [];
        tokens.ToList().Add(new MultiFactorTokenDocumentV1()
        { 
            IdentityProvider = token.LoginProvider,
            Name = token.Name,
            Value = token.Value
        });

        if (await _redis.SetAsJsonAsync(key, tokens, "$.mfa_tokens") is false)
        {
            // Log error...
        }
    }

    protected override async Task<UserToken?> FindTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(loginProvider);
        ArgumentNullException.ThrowIfNull(name);

        cancellationToken.ThrowIfCancellationRequested();

        // TODO: We potentially don't even need the Redis index on "mfa_tokens" if we are just going to load the entire document path anyway.
        IEnumerable<MultiFactorTokenDocumentV1?> tokens = await _redis.Json.GetEnumerableAsync<MultiFactorTokenDocumentV1>($"dashboard:users:{user.Id}", "$.mfa_tokens[*]");

        MultiFactorTokenDocumentV1? token = tokens.ToList()?.SingleOrDefault(t => t?.IdentityProvider == loginProvider && t?.Name == name);

        if (token is null)
        {
            // Log error...
            return null;
        }

        return CreateUserToken(user, loginProvider, name, token.Value);
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

