namespace BlazorAdminDashboard.Domain.Identity;

public sealed record Device
{   
    public required string Fingerprint { get; set; }

    public required string Name { get; set; }
    
    public required string IpAddress { get; set; }
    
    public string? Location { get; set; }

    public required DateTimeOffset AccessedAt { get; set; }

    public Device()
    {

    }

    public Device(
        string fingerprint,
        string name,
        string ipAddress,
        string location,
        DateTimeOffset accessedAt)
    {
        Name = name;
        Fingerprint = fingerprint;
        IpAddress = ipAddress;
        Location = location;
        AccessedAt = accessedAt;
    }

    public static explicit operator Device(DeviceDocumentV1 doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        return new Device()
        {
            Name = doc.Name,
            Fingerprint = doc.Fingerprint,
            IpAddress = doc.LastIpAddress,
            Location = doc.LastLocation,
            AccessedAt = doc.LastAccessedAt
        };
    }
}
