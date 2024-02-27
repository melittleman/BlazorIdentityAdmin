namespace BlazorAdminDashboard.Web.Theming;

public sealed class MainTheme : DefaultTheme
{
    public MainTheme(CustomTheme theme) : base(theme)
    {
        Palette.AppbarBackground = Palette.Secondary;
        Palette.AppbarText = Palette.SecondaryContrastText;

        PaletteDark.AppbarBackground = PaletteDark.Secondary;
        PaletteDark.AppbarText = PaletteDark.SecondaryContrastText;
    }
}
