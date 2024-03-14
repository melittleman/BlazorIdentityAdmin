namespace BlazorIdentityAdmin.Infrastructure.Configuration;

public sealed record SendGridConfiguration
{
    public string? ApiKey { get; set; }

    public string? EmailFromAddress { get; set; }
}
