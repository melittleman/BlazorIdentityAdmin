namespace BlazorAdminDashboard.Application.Identity.Abstractions;

public interface IDeviceEmailSender
{
    Task<bool> SendNewDeviceEmailAsync(User user, string email, Device device, string resetPasswordLink);
}
