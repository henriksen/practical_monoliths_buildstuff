using Commerce.Features.Customer.Application;
using Commerce.Features.Customer.Data;
using Commerce.SharedKernel.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Features.Customer.Hosting;

public class RegisterCustomerModule : IRegisterModule
{
    public void AddModule(IHostApplicationBuilder builder)
    {
        builder.AddCustomerServices();
    }

    public void UseModule(WebApplication app)
    {
        app.UseCustomerMiddleware();
        app.MapCustomerEndpoints();
    }
}

internal static class CustomerServiceRegistration
{
    public static IHostApplicationBuilder AddCustomerServices(
        this IHostApplicationBuilder builder)
    {

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<RegisterCustomerModule>();
        });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<CustomerDbContext>(options =>
            options.UseSqlServer(connectionString));
        return builder;
    }
}

internal static class CustomerMiddleware
{
    public static IApplicationBuilder UseCustomerMiddleware(this IApplicationBuilder app)
    {
        app.UseWhen(context =>
            context.Request.Path.StartsWithSegments("/api/Customer")
            , appBuilder =>
                appBuilder.UseMiddleware<CustomerLoggingMiddleware>());
        return app;
    }
}

internal class CustomerLoggingMiddleware(RequestDelegate next)
{
    // Middleware for special logging of all requests to the Customer API

    public async Task InvokeAsync(HttpContext context)
    {
        // Log request
        await next(context);
    }

}

internal static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGroup("/api/Customer")
            .MapCustomerApi();
        return routes;
    }


    public static RouteGroupBuilder MapCustomerApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllCustomersAsync);
        group.MapGet("/{id}", GetCustomerAsync);
        group.MapPost("/", CreateCustomerAsync);
        group.MapPut("/{id}/name", UpdateCustomerNameAsync);
        group.MapDelete("/{id}", DeactivateCustomerAsync);

        return group;
    }

    private static Task DeactivateCustomerAsync(IMediator mediator,
        Guid id)
    {
        return mediator.Send(new Deactivate.Command(id));
    }

    private static Task UpdateCustomerNameAsync(IMediator mediator,
        Guid id, string name)
    {
        return mediator.Send(new UpdateName.Command(id, name));
    }

    private static Task<Create.Response> CreateCustomerAsync(IMediator mediator,
        Create.Command command)
    {
        return mediator.Send(command);
    }

    private static Task<Get.Response> GetCustomerAsync(IMediator mediator, Guid id)
    {
        return mediator.Send(new Get.Query(id));
    }

    private static Task<GetPage.Response> GetAllCustomersAsync(IMediator mediator,
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