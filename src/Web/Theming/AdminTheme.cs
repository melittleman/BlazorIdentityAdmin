namespace BlazorAdminDashboard.Web.Theming;

public sealed class AdminTheme : DefaultTheme
{
    public AdminTheme(CustomTheme theme) : base(theme)
    {
        Palette.DrawerBackground = Palette.Primary;
        Palette.DrawerText = Palette.PrimaryContrastText;
        Palette.DrawerIcon = Palette.Secondary;

        Palette.AppbarBackground = Palette.Surface;
        Palette.AppbarText = Palette.TextPrimary;

        PaletteDark.DrawerBackground = PaletteDark.Primary;
        PaletteDark.DrawerText = PaletteDark.PrimaryContrastText;
        PaletteDark.DrawerIcon = PaletteDark.Secondary;

        PaletteDark.AppbarBackground = PaletteDark.Surface;
        PaletteDark.AppbarText = PaletteDark.TextPrimary;

        ZIndex = new()
        {
            // This forces the Drawer to open over the top off
            // everything else including the app header bar.
            Drawer = 1399
        };
    }
}
