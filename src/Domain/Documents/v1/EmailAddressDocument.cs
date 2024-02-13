using System.Text.Json.Serialization;
using BlazorAdminDashboard.Domain.Enums;

namespace BlazorAdminDashboard.Domain.Documents.v1;

public sealed class EmailAddressDocumentV1
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("type")]
    public required EmailAddressType Type { get; set; }

    [JsonPropertyName("is_primary")]
    public bool IsPrimary { get; set; } = false;

    [JsonPropertyName("is_private")]
    public bool IsPrivate { get; set; } = false;

    [JsonPropertyName("is_confirmed")]
    public bool IsConfirmed { get; set; } = false;
}
