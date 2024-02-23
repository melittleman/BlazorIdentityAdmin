using System.Security.Cryptography;
using PSC.Blazor.Components.BrowserDetect;

namespace BlazorAdminDashboard.Web.Models;

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

        byte[] hash = SHA256.HashData(Encoding.ASCII.GetBytes(data));

        // Truncate the string to 48 characters which still gives us
        // enough uniqueness without storing excessive amounts of data.
        return BitConverter.ToString(hash).Replace("-", string.Empty)[..48];
    }

    public string GetOperatingSystem()
    {
        if (string.IsNullOrEmpty(OSName) is false)
        {
            if (string.IsNullOrEmpty(OSVersion) is false)
            {
                // Using the '@' character for separation here, so 
                // that we can more easily split this string later on as 
                // There could be Operating Systems that contain spaces
                // for example "Linux Red Hat".

                // e.g. "Windows@11 or later"
                return $"{OSName}@{OSVersion}";
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
                // Using the '@' character for separation here, so 
                // that we can more easily split this string later on as 
                // There could be Browser names that contain spaces
                // for example "Duck Duck Go".

                // e.g. "Chrome@121"
                return $"{BrowserName}@{BrowserMajor}";
            }

            // e.g. "Brave"
            return BrowserName;
        }

        return "Unknown Browser";
    }
}
