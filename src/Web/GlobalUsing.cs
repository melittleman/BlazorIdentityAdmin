global using System;
global using System.IO;
global using System.Linq;
global using System.Text;
global using System.Text.Json;
global using System.Text.Encodings.Web;
global using System.Reflection;
global using System.Threading.Tasks;
global using System.Security.Claims;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.Extensions;
global using Microsoft.AspNetCore.HttpOverrides;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.WebUtilities;

global using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Primitives;
global using Microsoft.Extensions.DependencyInjection;

global using MudBlazor;
global using MudBlazor.Services;

global using OpenIddict.Abstractions;
global using OpenIddict.Server.AspNetCore;

global using RedisKit.Querying;
global using RedisKit.Querying.Abstractions;

global using BlazorIdentityAdmin.Application;
global using BlazorIdentityAdmin.Application.Identity;
global using BlazorIdentityAdmin.Domain.Theming;
global using BlazorIdentityAdmin.Domain.Identity;
global using BlazorIdentityAdmin.Infrastructure;
global using BlazorIdentityAdmin.Web;
global using BlazorIdentityAdmin.Web.Models;
global using BlazorIdentityAdmin.Web.Components;
global using BlazorIdentityAdmin.Web.Components.Admin;
global using BlazorIdentityAdmin.Web.Pages.Account;
