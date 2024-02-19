using BlazorAdminDashboard.Domain.Documents.v1;
using Microsoft.AspNetCore.Identity;

namespace BlazorAdminDashboard.Domain.Identity;

public class User : IdentityUser<Ulid>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? AvatarUrl { get; set; }
    
    public string CultureName { get; set; } = "en-US"; // TODO: Constant

    public string TimezoneId { get; set; } = "UTC"; // TODO: Constant

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset LastModifiedAt { get; set; }

    public bool IsLockedOut => LockoutEnabled && LockoutEnd.HasValue;

    public User()
    {
        Id = Ulid.NewUlid();

        DateTimeOffset now = DateTimeOffset.UtcNow;

        CreatedAt = now;
        LastModifiedAt = now;
    }

    public User(string username) : this()
    {
        UserName = username;
    }

    public static explicit operator User(UserDocumentV1 doc)
    {
        EmailAddressDocumentV1? email = doc.EmailAddresses.SingleOrDefault(e => e.IsPrimary);

        return new User()
        {
            Id = doc.Id,

            FirstName = doc.FirstName,
            LastName = doc.LastName,
            UserName = doc.Username,

            AvatarUrl = doc.AvatarUrl,
            CultureName = doc.CultureName,
            TimezoneId = doc.TimezoneId,

            Email = email?.Email,
            EmailConfirmed = email?.IsConfirmed ?? false,

            PhoneNumber = doc.PhoneNumbers?.First().Number,
            PhoneNumberConfirmed = doc.PhoneNumbers?.First().IsConfirmed ?? false,

            PasswordHash = doc.PasswordHash,
            ConcurrencyStamp = doc.ConcurrencyStamp,
            SecurityStamp = doc.SecurityStamp,

            CreatedAt = doc.CreatedAt,
            LastModifiedAt = doc.LastModifiedAt
        };
    }
}

