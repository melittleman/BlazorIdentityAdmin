using MudBlazor;
using MudBlazor.Services;

namespace BlazorAdminDashboard.Web;

public static class ConfigureServices
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IHostEnvironment env)
    {
        // Add Blazor / Razor pages UI services to the container.
        services.AddRazorPages();
        services.AddControllers();
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddCascadingAuthenticationState();

        // To forward the scheme from the proxy in Kestrel environments.
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;

            // TODO: We can configure these to be exact known incoming proxy addresses for
            // added security as otherwise this will allow forwarded headers from any source.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
        });

        if (env.IsDevelopment())
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                string xmlPath = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                options.IncludeXmlComments(xmlPath);
            });
        }

        return services;
    }
}
