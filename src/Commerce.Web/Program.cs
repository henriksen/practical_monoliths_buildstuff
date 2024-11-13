using Commerce.SharedKernel.Domain.ValueObjects;
using Commerce.SharedKernel.Infrastructure;
using Commerce.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSharedKernelServices();

var moduleRegistry = new ModuleRegistry();
moduleRegistry.AddModuleServices(builder);

builder.Services.AddRazorComponents();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new Email.JsonConverter());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

moduleRegistry.UseModuleServices(app);

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(Commerce.Features.Identity.Components.Account.Pages.Login).Assembly)
    ;

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
public partial class Program { }