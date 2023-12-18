using System.Text.Json.Serialization;

namespace BlazorAdminDashboard.Domain.Documents.v1;

public sealed class ExternalLoginDocumentV1
{
    [JsonPropertyName("iss")]
    public required string Issuer { get; set; }

    [JsonPropertyName("sub")]
    public required string Subject { get; set; }

    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}
