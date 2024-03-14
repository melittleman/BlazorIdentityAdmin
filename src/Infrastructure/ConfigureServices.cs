using BlazorIdentityAdmin.Infrastructure.Extensions;

namespace BlazorIdentityAdmin.Infrastructure;

public static partial class ConfigureServices
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
            options.ClientName = "BlazorIdentityAdmin.Client";

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

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddExternalLoginProviders(configuration)
        .AddIdentityCookies(options =>
        {
            string cookiePrefix = "BlazorIdentityAdmin";

            options.ApplicationCookie?.Configure(options =>
            {
                options.LoginPath = "/login";
                options.Cookie.Name = cookiePrefix + ".Application";

                // TODO: Probably need a constant for this as it is
                // used in multiple places acrss the application.
                options.ReturnUrlParameter = "return_url";
            });

            options.ExternalCookie?.Configure(options =>
            {
                options.Cookie.Name = cookiePrefix + ".External";
            });

            options.TwoFactorRememberMeCookie?.Configure(options =>
            {
                options.Cookie.Name = cookiePrefix + "TwoFactorRememberMe";
            });

            options.TwoFactorUserIdCookie?.Configure(options =>
            {
                options.Cookie.Name = cookiePrefix + ".TwoFactorUserId";
            });
        });

        services.AddIdentityCore<User>(options =>
        {
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
            options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            options.ClaimsIdentity.SecurityStampClaimType = "stamp"; // TODO: Is there a better alternative?

            // TODO: Need to work out what we actually want to do here.
            // I feel like logging in from an external provider without a confirmed email should be fine?
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedAccount = false;

            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 4;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireDigit = true;

            options.User.RequireUniqueEmail = false;

            options.Stores.MaxLengthForKeys = 128;
            options.Stores.ProtectPersonalData = false;
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