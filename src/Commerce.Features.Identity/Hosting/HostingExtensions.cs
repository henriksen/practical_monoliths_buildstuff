using Commerce.Features.Identity.Application;
using Commerce.Features.Identity.Data;
using Commerce.SharedKernel.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Features.Identity.Hosting;

public class RegisterIdentityModule : IRegisterModule
{
    public void AddModule(IHostApplicationBuilder builder)
    {
        builder.AddIdentityServices();
    }

    public void UseModule(WebApplication app)
    {
        app.MapIdentityEndpoints();
    }
}

internal static class IdentityServiceRegistration
{
    public static IHostApplicationBuilder AddIdentityServices(
        this IHostApplicationBuilder builder)
    {

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<RegisterIdentityModule>();
        });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(connectionString));
        return builder;
    }
}


internal static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGroup("/api/user")
            .MapIdentityApi();
        return routes;
    }


    public static RouteGroupBuilder MapIdentityApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllUsersAsync);


        return group;
    }

    private static Task<GetPage.Response> GetAllUsersAsync(IMediator mediator,
           [FromQuery] int pageNumber = 0,
           [FromQuery] int pageSize = 20)
    {
        if (pageSize > 500)
        {
            throw new ArgumentException("PageSize cannot be larger than 500", nameof(pageSize));
        }

        return mediator.Send(new GetPage.Query(pageNumber, pageSize));
    }
}