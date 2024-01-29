using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using NRedisKit;
using NRedisKit.Extensions;
using NRedisKit.DependencyInjection.Abstractions;

using BlazorAdminDashboard.Domain.Identity;
using BlazorAdminDashboard.Domain.Documents.v1;

namespace BlazorAdminDashboard.Infrastructure.Stores;

public class RedisRoleStore(
    IRedisClientFactory redisFactory,
    IdentityErrorDescriber errorDescriber) : RoleStoreBase<Role, Ulid, UserRole, RoleClaim>(errorDescriber)
{
    private readonly RedisClient _redis = redisFactory.CreateClient("persistent-db");

    // I don't believe this is actually required for any of the .NET Identity boilerplate.
    // However, because this is an abstract property, we really have no choice but to override.
    public override IQueryable<Role> Roles => throw new NotImplementedException();

    public override Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<Role?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task<Role?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        string name = normalizedName.EscapeSpecialCharacters();

        RoleDocumentV1? document = await _redis.SearchSingleAsync<RoleDocumentV1>("idx:roles", "@name:{" + name + "}");
        if (document is null) return null;

        // Note: This is where we would be doing any neccessary conversions between v1 and v2+ etc. of the document.

        // This 'could' be made to an implicit conversion to be slightly nicer on the eye...
        // but I have become a fan of more 'explicitness' where appropriate.
        return (Role)document;
    }

    public override Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
