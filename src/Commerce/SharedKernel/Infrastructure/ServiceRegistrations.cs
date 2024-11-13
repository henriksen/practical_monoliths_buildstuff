using Commerce.SharedKernel.Infrastructure.Data;

using Microsoft.Extensions.DependencyInjection;

namespace Commerce.SharedKernel.Infrastructure;

public static class ServiceRegistrations
{
    public static IServiceCollection AddSharedKernelServices(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }

}