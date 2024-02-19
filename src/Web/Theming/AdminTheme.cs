using BlazorAdminDashboard.Domain.Theming;
using MudBlazor;

namespace BlazorAdminDashboard.Web.Theming;

public sealed class AdminTheme : DefaultTheme
{
    public AdminTheme(CustomTheme theme) : base(theme)
    {
        Palette.DrawerBackground = Palette.Primary;
        Palette.DrawerText = Palette.PrimaryContrastText;
        Palette.DrawerIcon = Palette.Secondary;

        Palette.AppbarBackground = Colors.Shades.White;
        Palette.AppbarText = Colors.Grey.Darken3;

        PaletteDark.DrawerBackground = PaletteDark.Primary;
        PaletteDark.DrawerText = PaletteDark.PrimaryContrastText;
        PaletteDark.DrawerIcon = Palette.Secondary;

        PaletteDark.AppbarBackground = "#32333d";
        PaletteDark.AppbarText = "rgba(255,255,255, 0.70)";

        ZIndex = new()
        {
            // This forces the Drawer to open over the top off
            // everything else including the app header bar.
            Drawer = 1399
        };
    }
}
