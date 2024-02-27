namespace BlazorAdminDashboard.Web.Theming;

public class DefaultTheme : MudTheme
{
    public DefaultTheme(CustomTheme theme)
    {
        // TODO: How many custom properties do we actually need to pass
        // into the constructor here? The fewer the better ideally...

        Palette = new PaletteLight()
        {
            Background = "#f8f8f6",
            Surface = "ffffff",
            TextPrimary = "#262937",

            Primary = theme.LightPrimaryColor,
            PrimaryContrastText = theme.LightPrimaryAccentColor,

            Secondary = theme.LightSecondaryColor,
            SecondaryContrastText = theme.LightSecondaryAccentColor,

            Tertiary = theme.LightTertiaryColor,
            TertiaryContrastText = theme.LightTertiaryAccentColor,
        };

        PaletteDark = new PaletteDark()
        {
            Background = "#262937",
            Surface = "#2e313f",
            TextPrimary = "#f8f8f6",

            Primary = theme.DarkPrimaryColor,
            PrimaryContrastText = theme.DarkPrimaryAccentColor,

            Secondary = theme.DarkSecondaryColor,
            SecondaryContrastText = theme.DarkSecondaryAccentColor,

            Tertiary = theme.DarkTertiaryColor,
            TertiaryContrastText = theme.DarkTertiaryAccentColor,
        };

        LayoutProperties = new()
        {
            DefaultBorderRadius = theme.BorderRadius,
        };

        Typography = new()
        {
            Default = new()
            {
                FontFamily = theme.FontFamily,
                FontSize = "1rem",
                FontWeight = 400
            },
            H1 = new()
            {
                FontFamily = theme.FontFamily,
                FontSize = "2.5rem",
                FontWeight = 700
            },
            H2 = new() 
            {
                FontFamily = theme.FontFamily,
                FontSize  = "2.25rem",
                FontWeight = 600
            },
            H3 = new()
            {
                FontFamily = theme.FontFamily,
                FontSize = "2rem",
                FontWeight = 500
            },
            H4 = new()
            {
                FontFamily = theme.FontFamily,
                FontSize = "1.75rem",
                FontWeight = 500
            },
            H5 = new()
            {
                FontFamily = theme.FontFamily,
                FontSize = "1.5rem",
                FontWeight = 400
            },
            H6 = new()
            {
                FontFamily = theme.FontFamily,
                FontSize = "1.25rem",
                FontWeight = 300
            }
        };
    }
}
