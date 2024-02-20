namespace BlazorAdminDashboard.Infrastructure.Email;

public sealed class SendGridEmailSender(
    IOptions<SendGridConfiguration> options,
    ILogger<SendGridEmailSender> logger,
    ISendGridClient client) : IEmailSender<User>
{
    private readonly SendGridConfiguration _config = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger<SendGridEmailSender> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISendGridClient _client = client ?? throw new ArgumentNullException(nameof(client));

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        SendGridMessage message = new()
        {
            From = new EmailAddress(_config.EmailFromAddress, "Blazor Admin Dashboard"),
            Subject = subject,
            PlainTextContent = htmlMessage,
            HtmlContent = htmlMessage
        };

        message.AddTo(new EmailAddress(email));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        message.SetClickTracking(false, false);

        Response response = await _client.SendEmailAsync(message);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email send to {Address} queued successfully!", email);
            return;
        }

        _logger.LogWarning("Email send to {Address} failed to queue.", email);
    }

    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink) =>
        SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
}
