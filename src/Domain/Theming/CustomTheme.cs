namespace BlazorAdminDashboard.Domain.Theming;

public class CustomTheme
{
    public string PrimaryColor { get; set; } = "#262937";

    public string PrimaryAccentColor { get; set; } = "#f8f8f6";

    public string SecondaryColor { get; set; } = "#f4b43a";

    public string SecondaryAccentColor { get; set; } = "#262937";

    public string TertiaryColor { get; set; } = "#df2073";

    public string TertiaryAccentColor { get; set; } = "ffffff";

    public string BorderRadius { get; set; } = "0.5rem";

    public string[] FontFamily { get; set; } = ["Roboto", "Helvetica", "Arial", "sans-serif"];

    public string? LogoImageUrl { get; set; }
}
