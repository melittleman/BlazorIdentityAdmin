namespace BlazorIdentityAdmin.Domain.Identity;

public class Role : IdentityRole<Ulid>
{
    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset LastModifiedAt { get; set; }

    public Role()
    {
        Id = Ulid.NewUlid();

        DateTimeOffset now = DateTimeOffset.UtcNow;

        CreatedAt = now;
        LastModifiedAt = now;
    }

    public Role(string name) : this()
    {
        Name = name;
    }

    public static explicit operator Role(RoleDocumentV1 doc)
    {
        return new Role()
        {
            Id = doc.Id,

            Name = doc.Name,
            Description = doc.Description,

            ConcurrencyStamp = doc.ConcurrencyStamp,

            CreatedAt = doc.CreatedAt,
            LastModifiedAt = doc.LastModifiedAt
        };
    }
}
