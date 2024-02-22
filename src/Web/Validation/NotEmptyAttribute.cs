namespace BlazorAdminDashboard.Web.Validation;

/// <summary>
/// https://andrewlock.net/creating-an-empty-guid-validation-attribute/
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class NotEmptyAttribute : ValidationAttribute
{
    public const string DefaultErrorMessage = "{0} must not be empty.";

    public NotEmptyAttribute() : base(DefaultErrorMessage) { }

    public override bool IsValid(object? value)
    {
        // NotEmpty doesn't necessarily mean required
        if (value is null) return true;

        return value switch
        {
            Guid g => g != Guid.Empty,

            string s => s != string.Empty,

            IEnumerable<object> e => e.Any(),

            _ => true
        };
    }
}
