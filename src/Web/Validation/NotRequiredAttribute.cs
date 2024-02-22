namespace BlazorAdminDashboard.Web.Validation;

public sealed class NotRequiredAttribute : ValidationAttribute
{
    public const string DefaultErrorMessage = "{0} is not required.";

    public NotRequiredAttribute() : base(DefaultErrorMessage) { }

    public override bool IsValid(object? value) => true;
}
