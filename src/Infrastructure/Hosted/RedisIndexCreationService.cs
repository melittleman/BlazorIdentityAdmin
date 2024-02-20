namespace BlazorAdminDashboard.Infrastructure.Hosted;

internal sealed class RedisIndexCreationService : BackgroundService
{
    private readonly IRedisConnection _connection;
    private readonly RedisJsonOptions _jsonOptions;
    private readonly ILogger<RedisIndexCreationService> _logger;

    public RedisIndexCreationService(
        IRedisConnectionProvider redisProvider,
        ILogger<RedisIndexCreationService> logger,
        IOptionsMonitor<RedisJsonOptions> options)
    {
        // TODO: Name constant. Is there an easier / better way to sync this up as we are nearly
        // always going to want  the JSON options at the same time as the connection itself?
        _connection = redisProvider.GetRequiredConnection("persistent-db");
        _jsonOptions = options.Get("persistent-db");

        // TODO: The clashing of class names between Redis.OM and RedisKit here could become confusing,
        // e.g. both libraries have 'IRedisConnection' and 'IRedisConnectionProvider' so we should
        // probably think of a better alternative, maybe 'factory' or 'accessor'?

        // We aren't able to use Redis.OM yet anyway due to the lack of support
        // for JsonPropertyName attributes when creating the indexes.
        // _provider = new RedisConnectionProvider(_connection.Multiplexer);

        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("RedisIndexCreationService is executing on startup.");

        Task[] indexTasks =
        [
            CreateUserIndex(),
            CreateRoleIndex()

            // More indexes can be added here...
        ];

        await Task.WhenAll(indexTasks);

        _logger.LogInformation("RedisIndexCreationService has successfully finished executing on startup.");
    }

    private async Task<bool> CreateUserIndex()
    {
        _logger.LogDebug("User index is being created in Redis.");

        Schema schema = new();

        schema.AddTextField(new FieldName("$.first_name", "first_name"), sortable: true);
        schema.AddTextField(new FieldName("$.last_name", "last_name"), sortable: true);

        schema.AddTagField(new FieldName("$.username", "username"), sortable: true);
        schema.AddTagField(new FieldName("$.email_addresses[*].email", "email"), sortable: true);
        schema.AddTagField(new FieldName("$.phone_numbers[*].number", "phone_number"));

        schema.AddTagField(new FieldName("$.created_at", "created_at"), sortable: true, noIndex: true);

        schema.AddTagField(new FieldName("$.external_logins[*].iss", "login_provider"));
        schema.AddTagField(new FieldName("$.external_logins[*].sub", "login_key"));

        FTCreateParams createParams = FTCreateParams.CreateParams()
            .On(IndexDataType.JSON)
            .Prefix("dashboard:users:");

        if (await _connection.Db.FT().CreateIndexAsync("idx:users", createParams, schema) is false)
        {
            throw new InvalidOperationException("Unable to create idx:users Index in Redis!");
        }

        _logger.LogDebug("User index has been created in Redis.");
        return true;
    }

    private async Task<bool> CreateRoleIndex()
    {
        _logger.LogDebug("Role index is being created in Redis.");

        Schema schema = new();

        schema.AddTagField(new FieldName("$.name", "name"), sortable: true);
        schema.AddTagField(new FieldName("$.created_at", "created_at"), sortable: true, noIndex: true);

        FTCreateParams createParams = FTCreateParams.CreateParams()
            .On(IndexDataType.JSON)
            .Prefix("dashboard:roles:");

        if (await _connection.Db.FT().CreateIndexAsync("idx:roles", createParams, schema) is false)
        {
            throw new InvalidOperationException("Unable to create idx:users Index in Redis!");
        }

        _logger.LogDebug("Role index has been created in Redis.");
        return true;
    }
}
