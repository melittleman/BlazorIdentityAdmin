using Darnton.Blazor.DeviceInterop.Geolocation;

namespace BlazorAdminDashboard.Web.Models;

/// <inheritdoc />
public sealed class GeolocationInfo : GeolocationCoordinates
{
    public string GetCoordinates() => $"{Latitude}, {Longitude}";
}
