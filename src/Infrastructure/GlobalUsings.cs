global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Security.Claims;

global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using IdentityModel;
global using OpenIddict.Abstractions;

global using SendGrid;
global using SendGrid.Helpers.Mail;

global using NRedisStack;
global using NRedisStack.RedisStackCommands;
global using NRedisStack.Search;
global using NRedisStack.Search.Literals.Enums;

global using RedisKit.Extensions;
global using RedisKit.Abstractions;
global using RedisKit.Querying;
global using RedisKit.Querying.Extensions;
global using RedisKit.Querying.Abstractions;
global using RedisKit.DependencyInjection.Options;
global using RedisKit.DependencyInjection.Abstractions;
global using RedisKit.DependencyInjection.Extensions;

global using BlazorAdminDashboard.Domain.Identity;
global using BlazorAdminDashboard.Domain.Documents.v1;
global using BlazorAdminDashboard.Infrastructure.Email;
global using BlazorAdminDashboard.Infrastructure.Stores;
global using BlazorAdminDashboard.Infrastructure.Configuration;
global using BlazorAdminDashboard.Infrastructure.Hosted;
global using BlazorAdminDashboard.Application.Identity;
global using BlazorAdminDashboard.Application.Identity.Abstractions;
