using Darnton.Blazor.DeviceInterop.Geolocation;

namespace BlazorAdminDashboard.Web.Models;

/// <inheritdoc />
public sealed class GeolocationInfo : GeolocationCoordinates
{
    // Redis JSON wants the co-ordinates in this specific format:
    // See: https://redis.io/docs/interact/search-and-query/indexing/#index-json-arrays-as-geo
    public string GetCoordinates() => $"{Longitude},{Latitude}";
}
