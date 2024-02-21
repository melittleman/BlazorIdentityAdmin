namespace BlazorAdminDashboard.Domain.Identity;

public sealed record Device
{    
    public required string Name { get; set; }
    
    public required string IpAddress { get; set; }
    
    public string? Location { get; set; }

    public required DateTimeOffset AccessedAt { get; set; }

    public Device()
    {

    }

    public Device(string name, string ipAddress, string location, DateTimeOffset accessedAt)
    {
        Name = name;
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
            IpAddress = doc.IpAddress,
            Location = doc.LastLocation,
            AccessedAt = doc.LastAccessedAt
        };
    }
}
