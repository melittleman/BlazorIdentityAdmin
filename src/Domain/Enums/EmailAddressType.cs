﻿namespace BlazorIdentityAdmin.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EmailAddressType
{
    NotSet = 0,

    Personal = 1,

    Work = 2,

    School = 3,
}
