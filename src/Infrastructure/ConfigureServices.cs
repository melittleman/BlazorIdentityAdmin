namespace BlazorAdminDashboard.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        // SendGrid

        services.Configure<SendGridConfiguration>(configuration.GetSection("SendGrid"));

        services.AddSingleton<ISendGridClient>(new SendGridClient(configuration["SendGrid:ApiKey"]));
        services.AddSingleton<IEmailSender<User>, SendGridEmailSender>();
        services.AddSingleton<IDeviceEmailSender, SendGridEmailSender>();

        // Redis

        services.AddRedisConnection("persistent-db", options =>
        {
            // TODO: Get from appsettings
            options.ConnectionString = "localhost:6379";
            options.ClientName = "BlazorAdminDashboard.Client";

        }).AddRedisDataProtection(env, options =>
        {
            options.KeyName = "dashboard:data-protection:keys";

        }).AddRedisTicketStore(options =>
        {
            options.KeyPrefix = "dashboard:auth-tickets:";
            options.CookieSchemeName = IdentityConstants.ApplicationScheme;

        }).ConfigureRedisJson(options =>
        {
            // These serialization options are specific to this Redis connection.
            // We can use IServiceCollection.ConfigureRedisJson to configure them
            // globally, however it would be nice to have a single way to configure both?

            options.Serializer.Converters.Add(new JsonStringEnumConverter());
            options.Serializer.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.Serializer.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        });

        services.AddHostedService<RedisIndexCreationService>();

        // TODO: I'm not sure if these should actually be registered multiple times or not?
        // Need to investigate what the Pros/Cons are, because we could just cast from IUserStore...
        services.AddScoped<IUserDeviceStore, RedisUserStore>();
        services.AddScoped<IPagedUserStore, RedisUserStore>();
        services.AddScoped<IPagedTicketStore, RedisPagedTicketStore>();

        // Authentication

        AuthenticationBuilder builder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        });

        string? gitHubclientId = configuration["Authentication:GitHub:ClientId"];
        string? gitHubclientSecret = configuration["Authentication:GitHub:ClientSecret"];

        if (string.IsNullOrEmpty(gitHubclientId) is false && string.IsNullOrEmpty(gitHubclientSecret) is false)
        {
            builder.AddGitHub(options =>
            {
                // TODO: Move into user secrets...
                options.ClientId = gitHubclientId;
                options.ClientSecret = gitHubclientSecret;
                options.CallbackPath = "/connect/callback/signin-github";

                // TODO: Save AvatarUrl from www.github.com/<username>.png
                // What else do we want to save in our own claims?

                options.Scope.Add("read:user");
                options.SaveTokens = true;
            });
        }

        builder.AddIdentityCookies();
        //services.ConfigureApplicationCookie(options =>
        //{
        //    options.LoginPath = "/login";
        //});
        
        services.AddIdentityCore<User>(options =>
        {
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
            options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            options.ClaimsIdentity.SecurityStampClaimType = "sec_stamp"; // TODO: Is there a better alternative?

            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedAccount = true;

            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 4;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;

            options.Stores.MaxLengthForKeys = 128;
        })
        .AddRoles<Role>()
        .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
        .AddUserManager<CustomUserManager>()
        .AddRoleManager<CustomRoleManager>()
        .AddSignInManager<CustomSignInManager>()
        .AddUserStore<RedisUserStore>()
        .AddRoleStore<RedisRoleStore>()
        .AddDefaultTokenProviders();

        // OpenIddict
        services.AddOpenIddict().AddServer(options =>
        {
            // Enable the authorization, logout, token and userinfo endpoints.
            options.SetCryptographyEndpointUris(".well-known/jwks.json")
                .SetAuthorizationEndpointUris("connect/authorize")
                .SetLogoutEndpointUris("connect/logout")
                .SetTokenEndpointUris("connect/token")
                .SetUserinfoEndpointUris("connect/userinfo")
                .SetIntrospectionEndpointUris("connect/introspect")
                .SetDeviceEndpointUris("connect/device")
                .SetVerificationEndpointUris("connect/verify");

            // When integration with third-party APIs/resource servers is desired.
            options.DisableAccessTokenEncryption();
            options.DisableScopeValidation();

            options.UseAspNetCore()
                .EnableAuthorizationEndpointPassthrough()
                .EnableLogoutEndpointPassthrough()
                .EnableTokenEndpointPassthrough()
                .EnableUserinfoEndpointPassthrough()
                .EnableVerificationEndpointPassthrough()
                .EnableStatusCodePagesIntegration();

            options.AllowAuthorizationCodeFlow()
                .AllowDeviceCodeFlow()
                .AllowRefreshTokenFlow()
                .AllowClientCredentialsFlow();

            if (env.IsDevelopment())
            {
                options.AddDevelopmentEncryptionCertificate();
                options.AddDevelopmentSigningCertificate();
            }
            else
            {
                // TODO...
            }

        }).AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseAspNetCore();
        });

        // TODO: Need to register all the custom Store implementations that the OpenIddict.UseEntityFrameworkCore extension otherwise does.

        //builder.Services.AddAuthorizationCore(options =>
        //{
        //    options.AddPolicy(Policies.CanManageApplications, policy =>
        //    {
        //        policy.RequireAuthenticatedUser();
        //        policy.RequireClaim(Claims.ReadWriteClaim, "applications");
        //    });
        //    options.AddPolicy(Policies.CanManageScopes, policy =>
        //    {
        //        policy.RequireAuthenticatedUser();
        //        policy.RequireClaim(Claims.ReadWriteClaim, "scopes");
        //    });
        //    options.AddPolicy(Policies.CanManageUsers, policy =>
        //    {
        //        policy.RequireAuthenticatedUser();
        //        policy.RequireClaim(Claims.ReadWriteClaim, "users");
        //    });
        //    options.AddPolicy(Policies.CanManageRoles, policy =>
        //    {
        //        policy.RequireAuthenticatedUser();
        //        policy.RequireClaim(Claims.ReadWriteClaim, "roles");
        //    });
        //});

        return services;
    }
}