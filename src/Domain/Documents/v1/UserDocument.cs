﻿using BlazorAdminDashboard.Domain.Identity;
using BlazorAdminDashboard.Domain.Enums;
using System.Text.Json.Serialization;

namespace BlazorAdminDashboard.Domain.Documents.v1;

public sealed class UserDocumentV1
{
    [JsonPropertyName("id")]
    public required Ulid Id { get; set; }

    [JsonPropertyName("$schema_version")]
    public short SchemaVersion { get; set; } = 1; // TODO: Constant / Should this potentially be a getter only?

    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    // TOOD: I think we need a 'TenantId' or even an array also adding here?

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("email_addresses")]
    public required ICollection<EmailAddressDocumentV1> EmailAddresses { get; set; }

    [JsonPropertyName("phone_numbers")]
    public ICollection<PhoneNumberDocumentV1>? PhoneNumbers { get; set; }

    [JsonPropertyName("culture_name")]
    public string CultureName { get; set; } = "en-US"; // TODO: Constant

    [JsonPropertyName("timezone_id")]
    public string TimezoneId { get; set; } = "UTC"; // TODO: Constant

    [JsonPropertyName("password_hash")]
    public string? PasswordHash { get; set; }

    [JsonPropertyName("security_stamp")]
    public string? SecurityStamp { get; set; }

    [JsonPropertyName("concurrency_stamp")]
    public required string ConcurrencyStamp { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("last_modified_at")]
    public required DateTimeOffset LastModifiedAt { get; set; }

    [JsonPropertyName("is_mfa_enabled")]
    public bool IsMultiFactorEnabled { get; set; } = false;

    [JsonPropertyName("is_lockout_enabled")]
    public bool IsLockoutEnabled { get; set; } = false;

    [JsonPropertyName("lockout_ends_at")]
    public DateTimeOffset? LockoutEndsAt { get; set; }

    [JsonPropertyName("mfa_tokens")]
    public ICollection<MultiFactorTokenDocumentV1>? MultiFactorTokens { get; set; }

    [JsonPropertyName("external_logins")]
    public ICollection<ExternalLoginDocumentV1>? ExternalLogins { get; set; }

    [JsonPropertyName("devices")]
    public ICollection<DeviceDocumentV1>? Devices { get; set; }

    public static explicit operator UserDocumentV1(User user)
    {
        if (user.UserName is null) throw new InvalidOperationException("Username cannot be null.");
        if (user.Email is null) throw new InvalidOperationException("Email address cannot be null.");

        return new UserDocumentV1()
        {
            Id = user.Id,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddresses = new List<EmailAddressDocumentV1>()
            {
                new()
                {
                    Email = user.Email,
                    Type = EmailAddressType.School,
                    IsConfirmed = user.EmailConfirmed,
                }
            },
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt,
            LastModifiedAt = user.LastModifiedAt,
            SecurityStamp = user.SecurityStamp,
            ConcurrencyStamp = user.ConcurrencyStamp ?? Guid.NewGuid().ToString(),
            IsMultiFactorEnabled = user.TwoFactorEnabled,
            IsLockoutEnabled = user.LockoutEnabled
        };
    }
}