namespace BlazorAdminDashboard.Web.Theming;

public class DefaultTheme : MudTheme
{
    public DefaultTheme(CustomTheme theme)
    {
        // TODO: How many custom properties do we actually need to pass
        // into the constructor here? The fewer the better ideally...

        Palette = new PaletteLight()
        {
            Primary = theme.PrimaryColor,
            PrimaryContrastText = theme.PrimaryAccentColor,

            Secondary = theme.SecondaryColor,
            SecondaryContrastText = theme.SecondaryAccentColor,

            Tertiary = theme.TertiaryColor,
            TertiaryContrastText = theme.TertiaryAccentColor,
        };

        PaletteDark = new PaletteDark()
        {
            Primary = theme.PrimaryColor,
            PrimaryContrastText = theme.PrimaryAccentColor,

            Secondary = theme.SecondaryColor,
            SecondaryContrastText = theme.SecondaryAccentColor,

            Tertiary = theme.TertiaryColor,
            TertiaryContrastText = theme.TertiaryAccentColor,
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
            }
        };
    }
}
