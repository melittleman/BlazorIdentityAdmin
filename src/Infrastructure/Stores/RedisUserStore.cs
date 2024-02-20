using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using RedisKit.Extensions;
using RedisKit.Abstractions;
using RedisKit.Querying.Extensions;
using RedisKit.DependencyInjection.Options;
using RedisKit.DependencyInjection.Abstractions;

using NRedisStack.RedisStackCommands;

using IdentityModel;
using OpenIddict.Abstractions;

using BlazorAdminDashboard.Domain.Identity;
using BlazorAdminDashboard.Domain.Documents.v1;
using BlazorAdminDashboard.Application.Identity.Abstractions;
using RedisKit.Querying;
using RedisKit.Querying.Abstractions;
using NRedisStack;

namespace BlazorAdminDashboard.Infrastructure.Stores;

public class RedisUserStore(
    IRedisConnectionProvider redisProvider,
    IOptionsMonitor<RedisJsonOptions> options,
    IdentityErrorDescriber errorDescriber) : UserStoreBase<User, Ulid, UserClaim, UserLogin, UserToken>(errorDescriber), IPagedUserStore<User>
{
    // TODO: Constant Redis client name
    private readonly IRedisConnection _redis = redisProvider.GetRequiredConnection("persistent-db");
    private readonly RedisJsonOptions _jsonOptions = options.Get("persistent-db");

    private SearchCommands Search => _redis.Db.FT();

    // I don't believe this is actually required for any of the .NET Identity boilerplate.
    // However, because this is an abstract property, we really have no choice but to override.
    // TODO: We can remove the IQueryableUserStore implementation if we stop using 'UserStoreBase'.
    public override IQueryable<User> Users => throw new NotImplementedException();

    public async Task<IPagedList<User>> SearchUsersAsync(SearchFilter filter, string? query = null)
    {
        ArgumentNullException.ThrowIfNull(nameof(filter));

        // Default to "*" to return everything if not provided.
        query = query?.EscapeSpecialCharacters() ?? "*";

        IPagedList<UserDocumentV1> documents = await Search.SearchAsync<UserDocumentV1>("idx:users", query, filter);

        // TODO: See if there's a better way to achieve this...
        IEnumerable<User> users = documents.Select(doc => (User)doc);

        return users.ToPagedList(documents.TotalResults, filter);
    }

    public override async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
        if (await _redis.Db.JSON().SetAsync($"dashboard:users:{user.Id}", "$", (UserDocumentV1)user, serializerOptions: _jsonOptions.Serializer))
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
        UserDocumentV1? document = await _redis.Db.JSON().GetAsync<UserDocumentV1>($"dashboard:users:{userId}", serializerOptions: _jsonOptions.Serializer);

        if (document is null)
        {
            // TODO: Log error...
            return null;
        }

        return (User?)document;
    }

    public override async Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        string email = normalizedEmail.EscapeSpecialCharacters();

        // TODO: Constant for Redis index name
        // TODO: Why doesn't $ string interpolation seem to work for 'normalizedEmail'?
        // TODO: Are we able to use Redis.OM for this querying despite not actually using it for index creation?
        UserDocumentV1? document = await Search.SearchSingleAsync<UserDocumentV1>("idx:users", "@email:{" + email + "}");
        if (document is null) return null;

        // Note: This is where we would be doing any neccessary conversions between v1 and v2+ etc. of the document.

        // This 'could' be made to an implicit conversion to be slightly nicer on the eye...
        // but I have become a fan of more 'explicitness' where appropriate.
        return (User)document;
    }

    public override async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        string username = normalizedUserName.EscapeSpecialCharacters();

        UserDocumentV1? document = await Search.SearchSingleAsync<UserDocumentV1>("idx:users", "@username:{" + username + "}");
        if (document is null) return null;

        // Note: This is where we would be doing any neccessary conversions between v1 and v2+ etc. of the document.

        // This 'could' be made to an implicit conversion to be slightly nicer on the eye...
        // but I have become a fan of more 'explicitness' where appropriate.
        return (User)document;
    }

    public override async Task<IdentityResult> UpdateAsync(User user, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(user.UserName, nameof(user));
        ArgumentException.ThrowIfNullOrWhiteSpace(user.SecurityStamp, nameof(user));
        ArgumentException.ThrowIfNullOrWhiteSpace(user.ConcurrencyStamp, nameof(user)); // When does this need to be checked for concurrent access?

        ct.ThrowIfCancellationRequested();

        string key = $"dashboard:users:{user.Id}";

        UserDocumentV1? doc = await _redis.Db.JSON().GetAsync<UserDocumentV1>(key, serializerOptions: _jsonOptions.Serializer);
        if (doc is null) return IdentityResult.Failed(new IdentityError { Description = $"key {key} does not exist." });

        // TODO: How are we going to update more emails here as we are only provided one by default?
        EmailAddressDocumentV1? email = doc.EmailAddresses.SingleOrDefault(e => e.Email == user.Email);
        if (email is not null)
        {
            email.IsConfirmed = user.EmailConfirmed;
        }

        doc.Username = user.UserName;
        doc.FirstName = user.FirstName;
        doc.LastName = user.LastName;
        doc.AvatarUrl = user.AvatarUrl;

        doc.CultureName = user.CultureName;
        doc.TimezoneId = user.TimezoneId;
        doc.PasswordHash = user.PasswordHash;

        doc.SecurityStamp = user.SecurityStamp;
        doc.ConcurrencyStamp = user.ConcurrencyStamp;
        doc.LastModifiedAt = user.LastModifiedAt; // TODO: Does this also need updating whenever the security stamp changes?

        doc.IsMultiFactorEnabled = user.TwoFactorEnabled;
        doc.IsLockoutEnabled = user.LockoutEnabled;
        doc.LockoutEndsAt = user.LockoutEnd;

        // TODO: Can we pass in the cancellation token?
        if (await _redis.Db.JSON().SetAsync(key, "$", doc, serializerOptions: _jsonOptions.Serializer)) return IdentityResult.Success;

        return IdentityResult.Failed(new IdentityError { Description = $"Unable to update Redis user document at {key}." });

        // TODO: All this serialization is pretty horrible...
        // If we can find a better way to achieve this then I think this could work well.

        // OR

        // We could look to use the new JSON.MERGE e.g. _redis.Json.MergeAsync(key, "$", doc)

        //List<KeyPathValue> values = 
        //[
        //    new KeyPathValue(key, "$.username", JsonSerializer.Serialize(user.UserName)),
        //    new KeyPathValue(key, "$.culture_name", JsonSerializer.Serialize(user.CultureName)),
        //    new KeyPathValue(key, "$.timezone_id", JsonSerializer.Serialize(user.TimezoneId)),
        //    new KeyPathValue(key, "$.security_stamp", JsonSerializer.Serialize(user.SecurityStamp)),
        //    new KeyPathValue(key, "$.concurrency_stamp", JsonSerializer.Serialize(user.ConcurrencyStamp)),
        //    new KeyPathValue(key, "$.last_modified_at", JsonSerializer.Serialize(user.LastModifiedAt)),
        //    new KeyPathValue(key, "$.is_mfa_enabled", JsonSerializer.Serialize(user.TwoFactorEnabled)),
        //    new KeyPathValue(key, "$.is_lockout_enabled", JsonSerializer.Serialize(user.LockoutEnabled))
        //];

        //// TODO: Neeed to add Email and Phone Numbers to these...
        //if (user.FirstName != null) values.Add(new KeyPathValue(key, "$.first_name", JsonSerializer.Serialize(user.FirstName)));
        //if (user.LastName != null) values.Add(new KeyPathValue(key, "$.last_name", JsonSerializer.Serialize(user.LastName)));
        //if (user.AvatarUrl != null) values.Add(new KeyPathValue(key, "$.avatar_url", JsonSerializer.Serialize(user.AvatarUrl)));
        //if (user.PasswordHash != null) values.Add(new KeyPathValue(key, "$.password_hash", JsonSerializer.Serialize(user.PasswordHash)));
        //if (user.LockoutEnd != null) values.Add(new KeyPathValue(key, "$.lockout_ends_at", JsonSerializer.Serialize(user.LockoutEnd)));

        //bool result = await _redis.Json.MSetAsync([.. values]);

        //return result 
        //    ? IdentityResult.Success 
        //    : IdentityResult.Failed(new IdentityError { Description = $"Unable to update Redis document for user {user.Id}." });
    }

    #region Claims

    public override Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
    {
        // TODO: I hate this hard-coded approach to a SessionId... we need to investigate what other providers
        // like Pixel, IdentityServer or OrchardCore actually do here because I don't think that a 'Store' should
        // just keep a static list of Claims defined for a User?

        IList<Claim> claims =
        [
            new Claim(JwtClaimTypes.SessionId, CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex)),
        ];

        if (string.IsNullOrWhiteSpace(user.FirstName) is false)
        {
            claims.Add(new Claim(OpenIddictConstants.Claims.GivenName, user.FirstName));
        }

        if (string.IsNullOrWhiteSpace(user.LastName) is false)
        {
            claims.Add(new Claim(OpenIddictConstants.Claims.FamilyName, user.LastName));
        }

        return Task.FromResult(claims);
    }

    public override Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Tokens

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
        IEnumerable<MultiFactorTokenDocumentV1?> tokens = await _redis.Db.JSON().GetEnumerableAsync<MultiFactorTokenDocumentV1>(key, "$.mfa_tokens[*]");
        
        tokens ??= [];
        tokens = tokens.Append(new MultiFactorTokenDocumentV1()
        { 
            IdentityProvider = token.LoginProvider,
            Name = token.Name,
            Value = token.Value
        });

        if (await _redis.Db.JSON().SetAsync(key, "$.mfa_tokens", tokens) is false)
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

        IEnumerable<MultiFactorTokenDocumentV1?> tokens = await _redis.Db.JSON().GetEnumerableAsync<MultiFactorTokenDocumentV1>($"dashboard:users:{user.Id}", "$.mfa_tokens[*]");

        MultiFactorTokenDocumentV1? token = tokens.ToList()?.SingleOrDefault(t => t?.IdentityProvider == loginProvider && t?.Name == name);

        if (token is null)
        {
            // Log error...
            return null;
        }

        return CreateUserToken(user, loginProvider, name, token.Value);
    }

    protected override Task RemoveUserTokenAsync(UserToken token)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Logins

    public override async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        ct.ThrowIfCancellationRequested();

        string key = $"dashboard:users:{user.Id}";

        // TODO: This could be further simplified if we store an empty array as default in the document, then we can use
        // the JSON.ARRAPPEND command to add to this existing array without the need to first read it from the database.
        IEnumerable<ExternalLoginDocumentV1?> logins = await _redis.Db.JSON().GetEnumerableAsync<ExternalLoginDocumentV1>(key, "$.external_logins[*]");

        return logins.Where(l => l is not null).Select(l =>
        {
            return new UserLoginInfo(l!.Issuer, l.Subject, l.DisplayName);

        }).ToList();
    }

    public override async Task AddLoginAsync(User user, UserLoginInfo info, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(info);

        ct.ThrowIfCancellationRequested();

        ExternalLoginDocumentV1 externalLogin = new()
        {
            Issuer = info.LoginProvider,
            Subject = info.ProviderKey,
            DisplayName = info.ProviderDisplayName ?? info.LoginProvider
        };

        string key = $"dashboard:users:{user.Id}";

        // TODO: This could be further simplified if we store an empty array as default in the document, then we can use
        // the JSON.ARRAPPEND command to add to this existing array without the need to first read it from the database.
        IEnumerable<ExternalLoginDocumentV1?> logins = await _redis.Db.JSON().GetEnumerableAsync<ExternalLoginDocumentV1>(key, "$.external_logins[*]");

        logins ??= [];
        logins = logins.Append(externalLogin);

        if (await _redis.Db.JSON().SetAsync(key, "$.external_logins", logins) is false)
        {
            // Log error...
            throw new InvalidOperationException($"Unable to add login to User {user.Id}");
        }
    }

    protected override async Task<UserLogin?> FindUserLoginAsync(Ulid userId, string loginProvider, string providerKey, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);

        ct.ThrowIfCancellationRequested();

        string key = $"dashboard:users:{userId}";

        // TODO: This could be further simplified if we store an empty array as default in the document, then we can use
        // the JSON.ARRAPPEND command to add to this existing array without the need to first read it from the database.
        IEnumerable<ExternalLoginDocumentV1?> logins = await _redis.Db.JSON().GetEnumerableAsync<ExternalLoginDocumentV1>(key, "$.external_logins[*]");

        ExternalLoginDocumentV1? login = logins
            .Where(l => l is not null)
            .SingleOrDefault(l => l!.Issuer == loginProvider && l.Subject == providerKey);

        return login is null ? null : new UserLogin()
        {
            LoginProvider = login.Issuer,
            ProviderKey = login.Subject,
            ProviderDisplayName = login.DisplayName,
        };
    }

    protected override async Task<UserLogin?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);

        ct.ThrowIfCancellationRequested();

        string issuer = loginProvider.EscapeSpecialCharacters();
        string subject = providerKey.EscapeSpecialCharacters();

        UserDocumentV1? document = await Search.SearchSingleAsync<UserDocumentV1>("idx:users", "@login_provider:{" + issuer + "} @login_key:{" + subject + "}");
        if (document is null) return null;

        ExternalLoginDocumentV1? login = document.ExternalLogins?.SingleOrDefault(l => l.Issuer == loginProvider && l.Subject == providerKey);
        if (login is null) return null;

        return new UserLogin()
        {
            LoginProvider = login.Issuer,
            ProviderKey = login.Subject,
            ProviderDisplayName = login.DisplayName,
        };
    }

    public override Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    #endregion
}

