using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NRedisKit;
using NRedisKit.DependencyInjection.Abstractions;

using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;

namespace BlazorAdminDashboard.Infrastructure.Hosted;

internal sealed class RedisIndexCreationService(IRedisClientFactory redisFactory, ILogger<RedisIndexCreationService> logger) : BackgroundService
{
    private readonly RedisClient _redis = redisFactory.CreateClient("persistent-db");

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        logger.LogInformation("RedisIndexCreationService is executing on startup.");

        Task[] indexTasks =
        [
            CreateUserIndex(),
            CreateRoleIndex()

            // More indexes can be added here...
        ];

        await Task.WhenAll(indexTasks);

        logger.LogInformation("RedisIndexCreationService has successfully finished executing on startup.");
    }

    private async Task<bool> CreateUserIndex()
    {
        logger.LogDebug("User index is being created in Redis.");

        Schema schema = new();

        schema.AddTextField(new FieldName("$.first_name", "first_name"), sortable: true);
        schema.AddTextField(new FieldName("$.last_name", "last_name"), sortable: true);

        schema.AddTagField(new FieldName("$.username", "username"), sortable: true);
        schema.AddTagField(new FieldName("$.email_addresses[*].email", "email"), sortable: true);
        schema.AddTagField(new FieldName("$.phone_numbers[*].number", "phone_number"));

        schema.AddTagField(new FieldName("$.created_at", "created_at"), sortable: true, noIndex: true);

        schema.AddTagField(new FieldName("$.mfa_tokens[*].idp", "token_provider"));
        schema.AddTagField(new FieldName("$.mfa_tokens[*].name", "token_name"));

        schema.AddTagField(new FieldName("$.external_logins[*].iss", "login_provider"));
        schema.AddTagField(new FieldName("$.external_logins[*].sub", "login_key"));

        FTCreateParams createParams = FTCreateParams.CreateParams()
            .On(IndexDataType.JSON)
            .Prefix("dashboard:users:");

        // TODO: Remove 'forceRecreate'...
        if (await _redis.CreateIndex("idx:users", schema, createParams, forceRecreate: true) is false)
        {
            throw new InvalidOperationException("Unable to create idx:users Index in Redis!");
        }

        logger.LogDebug("User index has been created in Redis.");
        return true;
    }

    private async Task<bool> CreateRoleIndex()
    {
        logger.LogDebug("Role index is being created in Redis.");

        Schema schema = new();

        schema.AddTagField(new FieldName("$.name", "name"), sortable: true);
        schema.AddTagField(new FieldName("$.created_at", "created_at"), sortable: true, noIndex: true);

        FTCreateParams createParams = FTCreateParams.CreateParams()
            .On(IndexDataType.JSON)
            .Prefix("dashboard:roles:");

        // TODO: forceRecreate only works when the index already exists... I think this should be changed to ignore it?
        if (await _redis.CreateIndex("idx:roles", schema, createParams) is false)
        {
            throw new InvalidOperationException("Unable to create idx:roles Index in Redis!");
        }

        logger.LogDebug("Role index has been created in Redis.");
        return true;
    }
}
