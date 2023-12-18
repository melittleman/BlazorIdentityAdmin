using System.Text.Json.Serialization;

namespace BlazorAdminDashboard.Domain.Documents.v1;

public sealed class DeviceDocumentV1
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("ip_address")]
    public required string IpAddress { get; set; }

    [JsonPropertyName("last_location")]
    public required string LastLocation { get; set; }

    [JsonPropertyName("last_accessed_at")]
    public required DateTimeOffset LastAccessedAt { get; set; }
}
