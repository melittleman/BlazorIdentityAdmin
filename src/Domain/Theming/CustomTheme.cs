namespace BlazorAdminDashboard.Domain.Theming;

public class CustomTheme
{
    public string LightPrimaryColor { get; set; } = "#262937";

    public string LightPrimaryAccentColor { get; set; } = "#f8f8f6";

    public string LightSecondaryColor { get; set; } = "#f4b43a";

    public string LightSecondaryAccentColor { get; set; } = "#262937";

    public string LightTertiaryColor { get; set; } = "#df2073";

    public string LightTertiaryAccentColor { get; set; } = "#ffffff";

    public string DarkPrimaryColor { get; set; } = "#f4b43a";

    public string DarkPrimaryAccentColor { get; set; } = "#262937";

    public string DarkSecondaryColor { get; set; } = "#262937";

    public string DarkSecondaryAccentColor { get; set; } = "#f8f8f6";

    public string DarkTertiaryColor { get; set; } = "#df2073";

    public string DarkTertiaryAccentColor { get; set; } = "#ffffff";

    public string BorderRadius { get; set; } = "0.5rem";

    public string[] FontFamily { get; set; } = ["Roboto", "Helvetica", "Arial", "sans-serif"];

    public string? LogoImageUrl { get; set; }
}
