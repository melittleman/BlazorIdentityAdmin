using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using NRedisKit;
using NRedisKit.DependencyInjection.Abstractions;

using BlazorAdminDashboard.Domain.Identity;

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

    public override Task<Role?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
