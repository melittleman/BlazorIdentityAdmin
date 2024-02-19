using BlazorAdminDashboard.Domain.Theming;
using MudBlazor;

namespace BlazorAdminDashboard.Web.Theming;

public sealed class MainTheme : DefaultTheme
{
    public MainTheme(CustomTheme theme) : base(theme)
    {
        Palette.AppbarBackground = Palette.Primary;
        Palette.AppbarText = Palette.PrimaryContrastText;

        PaletteDark.AppbarBackground = Palette.Primary;
        PaletteDark.AppbarText = Palette.PrimaryContrastText;
    }
}
