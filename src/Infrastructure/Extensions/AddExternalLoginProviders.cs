using AspNet.Security.OAuth.GitHub;

namespace BlazorAdminDashboard.Infrastructure.Extensions;

public static partial class ConfigureServices
{
    internal static AuthenticationBuilder AddExternalLoginProviders(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        // Google

        string? googleclientId = configuration["Authentication:Google:ClientId"];
        string? googleclientSecret = configuration["Authentication:Google:ClientSecret"];

        if (string.IsNullOrEmpty(googleclientId) is false && string.IsNullOrEmpty(googleclientSecret) is false)
        {
            builder.AddGoogle(options =>
            {
                options.ClientId = googleclientId;
                options.ClientSecret = googleclientSecret;

                // TODO: Is there a constant for this also?
                options.CallbackPath = "/signin-google";

                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.Subject, "sub");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.Name, "name");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.GivenName, "given_name");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.FamilyName, "family_name");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.Email, "email");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.Locale, "locale");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.Picture, "picture");

                // By default, the Google OAuth handler requests the "openid", "profile"
                // and "email" scopes, so we can safely omit this here.
                //options.Scope.Add("openid");

                options.SaveTokens = true;
                options.UsePkce = true;
            });
        }

        // Microsoft

        string? microsoftclientId = configuration["Authentication:Microsoft:ClientId"];
        string? microsoftclientSecret = configuration["Authentication:Microsoft:ClientSecret"];

        if (string.IsNullOrEmpty(microsoftclientId) is false && string.IsNullOrEmpty(microsoftclientSecret) is false)
        {
            builder.AddMicrosoftAccount(options =>
            {
                options.ClientId = microsoftclientId;
                options.ClientSecret = microsoftclientSecret;
                options.CallbackPath = "/signin-microsoft";

                // TODO

                options.SaveTokens = true;
                options.UsePkce = true;

                options.Events.OnCreatingTicket = (context) =>
                {



                    return Task.CompletedTask;
                };
            });
        }

        // GitHub

        string? gitHubclientId = configuration["Authentication:GitHub:ClientId"];
        string? gitHubclientSecret = configuration["Authentication:GitHub:ClientSecret"];

        if (string.IsNullOrEmpty(gitHubclientId) is false && string.IsNullOrEmpty(gitHubclientSecret) is false)
        {
            builder.AddGitHub(options =>
            {
                options.ClientId = gitHubclientId;
                options.ClientSecret = gitHubclientSecret;
                options.CallbackPath = GitHubAuthenticationDefaults.CallbackPath;

                // See: https://docs.github.com/en/rest/users/users
                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.Subject, "id");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.Name, "name");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.Email, "email");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.PreferredUsername, "login");
                options.ClaimActions.MapJsonKey(OpenIddictConstants.Claims.Picture, "avatar_url");
                options.ClaimActions.MapJsonKey(GitHubAuthenticationConstants.Claims.Url, "url");

                options.Scope.Add("read:user");
                options.Scope.Add("user:email");
                options.SaveTokens = true;
                options.UsePkce = true;

                options.Events.OnCreatingTicket = (context) =>
                {
                    // TODO: GitHub can actually return more than 1 email address
                    // so it's potentially possible to add the secondary ones to the
                    // Claims Principal here, and then allow the user to choose which
                    // one they would like to register with.

                    return Task.CompletedTask;
                };
            });
        }

        return builder;
    }
}
