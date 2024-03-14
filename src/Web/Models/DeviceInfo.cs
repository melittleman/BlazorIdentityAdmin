using System.Security.Cryptography;
using PSC.Blazor.Components.BrowserDetect;

namespace BlazorIdentityAdmin.Web.Models;

/// <inheritdoc />
public sealed class DeviceInfo : BrowserInfo
{
    private const string SEPARATOR = "_";

    /// <summary>
    ///     Uses the navigator.brave.isBrave() API if available
    ///     to determine if the users browser is Brave. Otherwise
    ///     the User-Agent would only identify as 'Chrome'.
    /// </summary>
    public bool? IsBraveBrowser { get; set; }

    public string GetFingerprint()
    {
        // Skipping 'BrowserVersion' because it would cause the 
        // fingerprint to update too regularly compared to just 'Major'.

        // Skipping 'ScreenResolution' because this would cause the fingerprint to change
        // simply when the user moves their browser window to a different monitor.

        string data =
            BrowserMajor + SEPARATOR +
            BrowserName + SEPARATOR +
            CPUArchitect + SEPARATOR +
            DeviceModel + SEPARATOR +
            DeviceType + SEPARATOR +
            DeviceVendor + SEPARATOR +
            EngineName + SEPARATOR +
            EngineVersion + SEPARATOR +
            GPURenderer + SEPARATOR +
            GPUVendor + SEPARATOR +
            IsAndroid + SEPARATOR +
            IsBraveBrowser + SEPARATOR +
            IsDesktop + SEPARATOR +
            IsIPad + SEPARATOR +
            IsIPhone + SEPARATOR +
            IsMobile + SEPARATOR +
            IsTablet + SEPARATOR +
            OSName + SEPARATOR +
            OSVersion + SEPARATOR +
            TimeZone;

        // Using SHA1 here as it is simpler and faster, we don't actually need the added
        // security of SHA256 as this is not being used to transfer sensitive information.
        byte[] hash = SHA1.HashData(Encoding.UTF8.GetBytes(data));

        // Truncated to 36 charcters long (2 per byte) for ease of storage.
        return Convert.ToHexString(hash, 0, 18).ToLowerInvariant();        
    }

    public string GetDeviceType()
    {
        if (IsDesktop is true) return "PC";

        if (IsMobile is true) return "Mobile";

        if (IsTablet is true) return "Tablet";

        // TODO: I don't think we also need to check
        // IsAndroid, IsIPhone etc. here as they should
        // all just be caught by IsMobile, but need
        // to test that theory on different devices.

        return "Unknown Device";
    }

    public string GetOperatingSystem()
    {
        if (string.IsNullOrEmpty(OSName) is false)
        {
            if (string.IsNullOrEmpty(OSVersion) is false)
            {
                // e.g. "Windows 11 or later"
                return $"{OSName} {OSVersion}";
            }

            // e.g. "MacOS"
            return OSName;
        }

        return "Unknown OS";
    }

    public string GetBrowser()
    {
        if (string.IsNullOrEmpty(BrowserName) is false)
        {
            if (string.IsNullOrEmpty(BrowserMajor) is false)
            {
                // e.g. "Chrome 121"
                return $"{BrowserName} {BrowserMajor}";
            }

            // e.g. "Brave"
            return BrowserName;
        }

        return "Unknown Browser";
    }
}
