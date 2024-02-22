namespace BlazorAdminDashboard.Domain.Documents.v1;

public sealed class DeviceDocumentV1
{
    [JsonPropertyName("fingerprint")]
    public required string Fingerprint { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("last_ip_address")]
    public required string LastIpAddress { get; set; }

    [JsonPropertyName("last_location")]
    public string? LastLocation { get; set; }

    [JsonPropertyName("last_accessed_at")]
    public required DateTimeOffset LastAccessedAt { get; set; }

    public static explicit operator DeviceDocumentV1(Device device)
    {
        ArgumentNullException.ThrowIfNull(device);

        return new DeviceDocumentV1()
        {
            Name = device.Name,
            Fingerprint = device.Fingerprint,
            LastIpAddress = device.IpAddress,
            LastLocation = device.Location,
            LastAccessedAt = device.AccessedAt
        };
    }
}
