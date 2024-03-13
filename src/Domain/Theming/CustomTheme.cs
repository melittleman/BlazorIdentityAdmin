namespace BlazorAdminDashboard.Domain.Theming;

public sealed record CustomTheme
{
    public string[] FontFamily { get; set; } = [ "Lexend", "Roboto", "Helvetica", "Arial", "sans-serif" ];

    public string BorderRadius { get; set; } = "0.5rem";

    public string? LogoImageUrl { get; set; }

    // Light Theme

    public string LightPrimaryColor { get; set; } = "#f4b43a";

    public string LightPrimaryAccentColor { get; set; } = "#262937";

    public string LightSecondaryColor { get; set; } = "#262937";

    public string LightSecondaryAccentColor { get; set; } = "#f8f8f6";

    public string LightTertiaryColor { get; set; } = "#df2073";

    public string LightTertiaryAccentColor { get; set; } = "#ffffff";

    // Dark Theme

    public string DarkPrimaryColor { get; set; } = "#f4b43a";

    public string DarkPrimaryAccentColor { get; set; } = "#262937";

    public string DarkSecondaryColor { get; set; } = "#2c2f3f";

    public string DarkSecondaryAccentColor { get; set; } = "#f8f8f6";

    public string DarkTertiaryColor { get; set; } = "#df2073";

    public string DarkTertiaryAccentColor { get; set; } = "#f8f8f6";
}
