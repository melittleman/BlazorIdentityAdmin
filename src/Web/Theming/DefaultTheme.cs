namespace BlazorIdentityAdmin.Web.Theming;

public class DefaultTheme : MudTheme
{
    public DefaultTheme(CustomTheme theme)
    {
        // TODO: How many custom properties do we actually need to pass
        // into the constructor here? The fewer the better ideally...

        Palette = new PaletteLight()
        {
            Black = "#262937",
            White = "ffffff",

            // Using this web safe blue for links against both the background and
            // surface whites, still ensures a WCAG AAA contrast ratio compliance.
            Info = "#0066ff", // or #0033ff

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
            Black = "#000000",
            White = "#f8f8f6",

            // Same as above, this web safe blue gets an AA contrast ratio of 4.5:1 against
            // the surface color, we can go a lot lighter as it's on a dark background.
            Info = "#3399ff", // or #33ccff

            Background = "#262937",
            Surface = "#2c2f3f",
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
                FontWeight = 600
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
                FontWeight = 500
            },
            H6 = new()
            {
                FontFamily = theme.FontFamily,
                FontSize = "1.25rem",
                FontWeight = 400
            }
        };
    }
}
