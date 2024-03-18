WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddWebServices(builder.Environment);

WebApplication app = builder.Build();

// TODO: Move WebApplication configuring into extensions also?

app.UseForwardedHeaders();
app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "Blazor Identity Admin | Swagger UI";
    });
}
else
{
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios.
    // See https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseExceptionHandler("/error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithRedirects("/error?code={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// TODO: Wrap in a try... catch
await app.RunAsync();
