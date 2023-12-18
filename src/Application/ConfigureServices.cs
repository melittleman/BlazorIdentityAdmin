﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;

using BlazorAdminDashboard.Application.Identity;

namespace BlazorAdminDashboard.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        return services;
    }
}
