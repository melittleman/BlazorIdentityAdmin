namespace BlazorIdentityAdmin.Domain.Identity;

public class User : IdentityUser<Ulid>
{
    public string Name 
    {   
        get
        {
            if (string.IsNullOrEmpty(FirstName) is false &&
                string.IsNullOrEmpty(LastName) is false)
            {
                return $"{FirstName} {LastName}";
            }

            if (string.IsNullOrEmpty(UserName) is false)
            {
                return UserName;
            }

            return string.IsNullOrEmpty(Email)
                ? string.Empty
                : Email;
        }
    }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? AvatarUrl { get; set; }
    
    public string? CultureName { get; set; }

    public string? TimezoneId { get; set; }

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

