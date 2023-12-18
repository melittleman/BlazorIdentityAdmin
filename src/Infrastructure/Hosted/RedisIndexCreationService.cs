using Microsoft.Extensions.Hosting;

using NRedisKit;
using NRedisKit.DependencyInjection.Abstractions;

using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;

namespace BlazorAdminDashboard.Infrastructure.Hosted;

internal sealed class RedisIndexCreationService(IRedisClientFactory redisFactory) : BackgroundService
{
    private readonly RedisClient _redis = redisFactory.CreateClient("persistent-db");

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        Schema schema = new();

        schema.AddTagField(new FieldName("$.username", "username"), sortable: true);
        schema.AddTagField(new FieldName("$.email_addresses[*].email", "email"));

        FTCreateParams createParams = FTCreateParams.CreateParams()
            .On(IndexDataType.JSON)
            .Prefix("dashboard:users:");

        // TODO: Remove 'forceRecreate'...
        if (await _redis.CreateIndex("idx:users", schema, createParams) is false)
        {
            throw new InvalidOperationException("Unable to create idx:users Index in Redis!");
        }
    }
}
