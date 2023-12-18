using System.Text.Json.Serialization;

namespace BlazorAdminDashboard.Domain.Documents.v1;

public sealed class MultiFactorTokenDocumentV1
{
    [JsonPropertyName("idp")]
    public string IdentityProvider { get; set; } = "LOCAL_AUTHORITY";

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("value")]
    public required string Value { get; set; }
}
