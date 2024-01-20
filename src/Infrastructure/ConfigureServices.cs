using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NRedisKit.DependencyInjection.Extensions;

using BlazorAdminDashboard.Domain.Identity;
using BlazorAdminDashboard.Infrastructure.Email;
using BlazorAdminDashboard.Infrastructure.Stores;
using BlazorAdminDashboard.Infrastructure.Configuration;
using BlazorAdminDashboard.Infrastructure.Hosted;
using BlazorAdminDashboard.Application.Identity;
using SendGrid;

namespace BlazorAdminDashboard.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        // SendGrid

        services.Configure<SendGridConfiguration>(configuration.GetSection("SendGrid"));

        services.AddSingleton<ISendGridClient>(new SendGridClient(configuration["SendGrid:ApiKey"]));
        services.AddSingleton<IEmailSender<User>, SendGridEmailSender>();

        // Redis

        services.AddRedisConnection("persistent-db", options =>
        {
            // TODO: Get from appsettings
            options.ConnectionString = "localhost:6379";
            options.ClientName = "BlazorAdminDashboard.Client";

        }).AddRedisDataProtection(env, options =>
        {
            options.KeyName = "data-protection:keys";
        });

        // TODO: Note that this isn't actually being used until
        // NRedisStack update their version to 0.10.2
        services.ConfigureRedisJson(options =>
        {
            options.Serializer.Converters.Add(new JsonStringEnumConverter());
            options.Serializer.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.Serializer.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        });

        services.AddHostedService<RedisIndexCreationService>();

        // Authentication

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;

        }).AddIdentityCookies();

        services.AddIdentityCore<User>(options =>
        {
            options.Stores.MaxLengthForKeys = 128;
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddRoles<Role>()
        .AddUserManager<CustomUserManager>()
        .AddRoleManager<CustomRoleManager>()
        .AddUserStore<RedisUserStore>()
        .AddRoleStore<RedisRoleStore>()
        .AddDefaultTokenProviders()
        .AddSignInManager()
        .AddApiEndpoints();

        return services;
    }
}
