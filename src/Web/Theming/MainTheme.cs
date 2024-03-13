namespace BlazorAdminDashboard.Web.Theming;

public sealed class MainTheme : DefaultTheme
{
    public MainTheme(CustomTheme theme) : base(theme)
    {
        Palette.AppbarBackground = Palette.Background;
        Palette.AppbarText = Palette.TextPrimary;

        PaletteDark.AppbarBackground = PaletteDark.Background;
        PaletteDark.AppbarText = PaletteDark.TextPrimary;
    }
}
