﻿using BlazorAdminDashboard.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlazorAdminDashboard.Application.Identity;

public sealed class CustomRoleManager(
    IRoleStore<Role> store,
    IEnumerable<IRoleValidator<Role>> roleValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    ILogger<RoleManager<Role>> logger) : RoleManager<Role>(
        store,
        roleValidators,
        keyNormalizer,
        errors,
        logger)
{

}