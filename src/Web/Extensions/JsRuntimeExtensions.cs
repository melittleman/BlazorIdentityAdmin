using Microsoft.JSInterop;

namespace BlazorIdentityAdmin.Web.Extensions;

internal static class JsRuntimeExtensions
{
    internal static async Task SubmitFormAsync(this IJSRuntime JS, string formId)
    {
        IJSObjectReference loginForm = await JS.InvokeAsync<IJSObjectReference>("document.getElementById", formId);
        await loginForm.InvokeVoidAsync("submit");
    }
}
