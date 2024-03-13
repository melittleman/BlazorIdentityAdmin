namespace BlazorAdminDashboard.Web.Theming;

public sealed class AdminTheme : DefaultTheme
{
    public AdminTheme(CustomTheme theme) : base(theme)
    {
        Palette.DrawerBackground = Palette.Secondary;
        Palette.DrawerText = Palette.SecondaryContrastText;
        Palette.DrawerIcon = Palette.Primary;

        Palette.AppbarBackground = Palette.Background;
        Palette.AppbarText = Palette.TextPrimary;

        PaletteDark.DrawerBackground = PaletteDark.Secondary;
        PaletteDark.DrawerText = PaletteDark.SecondaryContrastText;
        PaletteDark.DrawerIcon = PaletteDark.Primary;

        PaletteDark.AppbarBackground = PaletteDark.Background;
        PaletteDark.AppbarText = PaletteDark.TextPrimary;

        ZIndex = new()
        {
            // This forces the Drawer to open over the top off
            // everything else including the app header bar.
            Drawer = 1399
        };
    }
}
