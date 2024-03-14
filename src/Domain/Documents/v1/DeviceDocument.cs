namespace BlazorIdentityAdmin.Domain.Documents.v1;

public sealed class DeviceDocumentV1
{
    [JsonPropertyName("fingerprint")]
    public required string Fingerprint { get; set; }

    [JsonPropertyName("os")]
    public required string OperatingSystem { get; set; }

    [JsonPropertyName("browser")]
    public required string Browser { get; set; }

    [JsonPropertyName("last_ip")]
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
            Fingerprint = device.Fingerprint,
            OperatingSystem = device.OperatingSystem,
            Browser = device.Browser,

            LastIpAddress = device.IpAddress,
            LastLocation = device.Location,
            LastAccessedAt = device.AccessedAt
        };
    }
}
