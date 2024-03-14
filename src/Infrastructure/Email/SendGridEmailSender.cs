namespace BlazorIdentityAdmin.Infrastructure.Email;

public sealed class SendGridEmailSender(
    IOptions<SendGridConfiguration> options,
    ILogger<SendGridEmailSender> logger,
    ISendGridClient client) : IEmailSender<User>, IDeviceEmailSender
{
    private readonly SendGridConfiguration _config = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger<SendGridEmailSender> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISendGridClient _client = client ?? throw new ArgumentNullException(nameof(client));

    public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
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
            return true;
        }

        _logger.LogWarning("Email send to {Address} failed to queue.", email);
        return false;
    }

    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink) =>
        SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode) =>
        SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");

    public Task<bool> SendNewDeviceEmailAsync(User user, string email, Device device, string resetPasswordLink)
    {
        // TODO: Properly implement this template!
        string html = $"<h1>New device login</h1>" +
            "<br />" +
            $"<p>Hi {user.Name},</p>" +
            $"<p>Did you try and sign in from a new device or browser?</p>" +
            $"<ul>" +
                $"<li><strong>Device: </strong>{device.GetDisplayName()}</li>" +
                $"<li><strong>Location: </strong>{device.Location}</li>" + // TODO: Need to turn this into something friendly via IPInfo.
                $"<li><strong>IP Address: </strong>{device.IpAddress}</li>" +
                $"<li><strong>Time: </strong> {device.AccessedAt:R}</li>" +
            "</ul>" +
            "<p>If this was you then please ignore this email.</p>" +
            $"<p>However if you don't recognize this activity, we recommend that you <a href='{resetPasswordLink}'>reset your password</a> now.</p>" +
            "<br />" +
            "<p>Thanks,</p>";
            
        return SendEmailAsync(email, "New device login", html);
    }
}
