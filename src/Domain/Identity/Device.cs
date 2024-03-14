namespace BlazorIdentityAdmin.Domain.Identity;

public sealed record Device
{   
    public required string Fingerprint { get; set; }

    public required string OperatingSystem { get; set; }

    public required string Browser { get; set; }

    public required string IpAddress { get; set; }
    
    public string? Location { get; set; }

    public required DateTimeOffset AccessedAt { get; set; }

    public string GetDisplayName()
    {
        // e.g. Windows 11 (Chrome 121)
        return $"{OperatingSystem} ({Browser})";
    }

    public Device() { }

    public Device(
        string fingerprint,
        string operatingSystem,
        string browserName,
        string ipAddress,
        string location,
        DateTimeOffset accessedAt)
    {
        Fingerprint = fingerprint;
        OperatingSystem = operatingSystem;
        Browser = browserName;
        
        IpAddress = ipAddress;
        Location = location;
        AccessedAt = accessedAt;
    }

    public static explicit operator Device(DeviceDocumentV1 doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        return new Device()
        {
            Fingerprint = doc.Fingerprint,
            OperatingSystem = doc.OperatingSystem,
            Browser = doc.Browser,
            
            IpAddress = doc.LastIpAddress,
            Location = doc.LastLocation,
            AccessedAt = doc.LastAccessedAt
        };
    }
}
