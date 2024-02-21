namespace BlazorAdminDashboard.Domain.Identity;

public class User : IdentityUser<Ulid>
{
    // TODO: Need to make sure this is required
    // across the board as I believe we should 
    // always have a non-nullable and unique 
    // "Username" to visually identify a user.
    // Note that this maps to the "preferred_username"
    // claim within the ClaimsPrincipal / token.
    public string Username => UserName ?? string.Empty;

    public string Name 
    {   
        get
        {
            if (string.IsNullOrEmpty(FirstName) is false &&
                string.IsNullOrEmpty(LastName) is false)
            {
                return $"{FirstName} {LastName}";
            }

            if (string.IsNullOrEmpty(Username) is false)
            {
                return Username;
            }

            return string.IsNullOrEmpty(Email)
                ? string.Empty
                : Email;
        }
    }

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

