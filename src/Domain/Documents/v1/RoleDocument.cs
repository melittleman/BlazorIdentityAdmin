namespace BlazorAdminDashboard.Domain.Documents.v1;

// TODO: Should these 'Documents' be records instead?
public sealed class RoleDocumentV1
{
    [JsonPropertyName("id")]
    public required Ulid Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("concurrency_stamp")]
    public required string ConcurrencyStamp { get; set; }

    // This is likely where we'll need to add 'permissions' but not too sure yet...

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("last_modified_at")]
    public required DateTimeOffset LastModifiedAt { get; set; }
}
