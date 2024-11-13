using Commerce.Features.Identity.Components.Account;
using Commerce.Features.Identity.Data;
using Commerce.Features.Identity.Domain;
using Commerce.SharedKernel.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Commerce.Features.Identity.Components.Hosting;
public class RegisterIdentityComponentsModule : IRegisterModule
{
    public void AddModule(IHostApplicationBuilder builder)
    {
        builder.AddIdentityComponentsServices();
    }

    public void UseModule(WebApplication app)
    {
    }
}

internal static class IdentityServiceRegistration
{
    public static IHostApplicationBuilder AddIdentityComponentsServices(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
        builder.Services.AddAuthorization();


        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        return builder;
    }
}
