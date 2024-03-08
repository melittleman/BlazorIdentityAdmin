using System.Globalization;
using BlazorAdminDashboard.Web.Pages.Public;
using Darnton.Blazor.DeviceInterop.Geolocation;

namespace BlazorAdminDashboard.Web;

public static class ConfigureServices
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IHostEnvironment env)
    {
        // Add Blazor / UI services to the container.
        services.AddControllers();
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddCascadingAuthenticationState();

        services.AddScoped<IGeolocationService, GeolocationService>();

        // To forward the scheme from the proxy in Kestrel environments.
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;

            // TODO: We can configure these to be exact known incoming proxy addresses for
            // added security as otherwise this will allow forwarded headers from any source.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddRequestLocalization(options =>
        {
            CultureInfo[] supportedCultures =
            [
                new CultureInfo("en"),
                new CultureInfo("en-US"),
                new CultureInfo("en-GB")
            ];

            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            options.SetDefaultCulture("en");
            
            // TODO: This is where we could set what was saved against the user's account,
            // although this does seem to get hit an awful lot, so maybe not such a great idea...
            // See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization/select-language-culture?view=aspnetcore-8.0#use-a-custom-provider
            //options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
            //{
            //    // My custom request culture logic
            //    return await Task.FromResult(new ProviderCultureResult("en"));
            //}));
        });

        services.AddMudServices(options =>
        {
            // TODO...
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

    // TODO: Should these all be moved to controllers?
    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    // TODO: Move into a separate extensions class rather than ConfigureServices...
    public static IEndpointRouteBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapPost("/perform-external-login", (
            HttpContext context,
            [FromServices] SignInManager<User> signInManager,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            IEnumerable<KeyValuePair<string, StringValues>> query = [
                new("return_url", returnUrl),
                new("action", ExternalLogin.LoginCallbackAction)];

            string redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/external-login",
                QueryString.Create(query));

            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return TypedResults.Challenge(properties, [provider]);
        });

        endpoints.MapPost("/logout", async (
            ClaimsPrincipal user,
            SignInManager<User> signInManager,
            [FromForm] string returnUrl) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect(returnUrl.StartsWith('/') ? returnUrl : "/" + returnUrl);
        });

        endpoints.MapPost("/account/link-external-login", async (
            HttpContext context,
            [FromServices] SignInManager<User> signInManager,
            [FromForm] string provider) =>
        {
            // Clear the existing external cookie to ensure a clean login process
            await context.SignOutAsync(IdentityConstants.ExternalScheme);

            string redirectUrl = UriHelper.BuildRelative(
                context.Request.PathBase,
                "/account/external-logins",
                QueryString.Create("Action", ExternalLogins.LinkLoginCallbackAction));

            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
            return TypedResults.Challenge(properties, [provider]);

        }).RequireAuthorization();

        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

        endpoints.MapPost("/account/download-personal-data", async (
            HttpContext context,
            [FromServices] UserManager<User> userManager,
            [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null)
            {
                return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
            }

            var userId = await userManager.GetUserIdAsync(user);
            downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(User).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
            var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");

        }).RequireAuthorization();

        return endpoints;
    }
}
