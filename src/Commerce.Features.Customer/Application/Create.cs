using Commerce.Features.Customer.Data;
using Commerce.SharedKernel.Domain.ValueObjects;

using MediatR;

namespace Commerce.Features.Customer.Application;

public static class Create
{
    public record Command(string Name, Email Email) : IRequest<Response>;
    public record Response(Guid Id);

    internal class Handler(CustomerDbContext dbContext)
        : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request,
            CancellationToken cancellationToken)
        {
            var customer = Domain.Customer.Create(request.Name, request.Email);

            dbContext.Customers.Add(customer);

            await dbContext.SaveChangesAsync(cancellationToken);

            return new Response(customer.Id);
        }
    }
}