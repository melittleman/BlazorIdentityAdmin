global using System;
global using System.IO;
global using System.Linq;
global using System.Text.Json;
global using System.Reflection;
global using System.Threading.Tasks;
global using System.Security.Claims;
global using System.Collections.Generic;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.Extensions;
global using Microsoft.AspNetCore.HttpOverrides;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Components.Authorization;

global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Primitives;
global using Microsoft.Extensions.DependencyInjection;

global using MudBlazor;
global using MudBlazor.Services;

global using BlazorAdminDashboard.Application;
global using BlazorAdminDashboard.Domain.Theming;
global using BlazorAdminDashboard.Domain.Identity;
global using BlazorAdminDashboard.Infrastructure;
global using BlazorAdminDashboard.Web;
global using BlazorAdminDashboard.Web.Components;
global using BlazorAdminDashboard.Web.Pages.Account;
global using BlazorAdminDashboard.Web.Pages.Account.Manage;
