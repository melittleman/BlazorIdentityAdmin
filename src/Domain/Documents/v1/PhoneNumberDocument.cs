namespace BlazorAdminDashboard.Domain.Documents.v1;

public sealed class PhoneNumberDocumentV1
{
    [JsonPropertyName("number")]
    public required string Number { get; set; }

    [JsonPropertyName("type")]
    public required PhoneNumberType Type { get; set; }

    [JsonPropertyName("is_primary")]
    public bool IsPrimary { get; set; } = false;

    [JsonPropertyName("is_private")]
    public bool IsPrivate { get; set; } = false;

    [JsonPropertyName("is_confirmed")]
    public bool IsConfirmed { get; set; } = false;
}
