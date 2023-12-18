using BlazorAdminDashboard.Domain.Documents.v1;
using Microsoft.AspNetCore.Identity;

namespace BlazorAdminDashboard.Domain.Identity;

public class User : IdentityUser<Ulid>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset LastModifiedAt { get; set; }

    public User()
    {
        Id = Ulid.NewUlid();

        DateTimeOffset now = DateTimeOffset.UtcNow;

        CreatedAt = now;
        LastModifiedAt = now;
    }

    public static explicit operator User(UserDocumentV1 doc)
    {
        return new User()
        {
            Id = doc.Id,
            FirstName = doc.FirstName,
            LastName = doc.LastName,
            UserName = doc.Username,
            Email = doc.EmailAddresses.First().Email,
            EmailConfirmed = doc.EmailAddresses.First().IsConfirmed,
            PasswordHash = doc.PasswordHash,
            ConcurrencyStamp = doc.ConcurrencyStamp,
            SecurityStamp = doc.SecurityStamp,
            CreatedAt = doc.CreatedAt,
            LastModifiedAt = doc.LastModifiedAt
        };
    }
}

