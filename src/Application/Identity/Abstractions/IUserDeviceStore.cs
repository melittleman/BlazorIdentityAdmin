namespace BlazorIdentityAdmin.Application.Identity.Abstractions;

public interface IUserDeviceStore : IUserStore<User>
{
    Task<ICollection<Device>> GetDevicesAsync(User user, CancellationToken? ct = default);

    Task<Device?> FindByFingerprintAsync(User user, string fingerprint, CancellationToken? ct = default);

    Task<bool> AddOrUpdateDeviceAsync(User user, Device device, CancellationToken? ct = default);

    Task<bool> RemoveDeviceAsync(User user, Device device, CancellationToken? ct = default);
}
