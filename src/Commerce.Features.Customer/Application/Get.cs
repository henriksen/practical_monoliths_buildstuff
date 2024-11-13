using Commerce.Features.Customer.Data;
using Commerce.SharedKernel.Exceptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Commerce.Features.Customer.Application;

public static class Get
{
    public record Query(Guid Id) : IRequest<Response>;
    public record Response(string Name, string Email);

    internal class Handler(CustomerDbContext dbContext)
        : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var customer = await dbContext.Customers
                .Where(c => c.Id == request.Id)
                .Select(c => new Response(c.Name, c.Email))
                .SingleOrDefaultAsync(cancellationToken);

            if (customer == null)
            {
                throw new NotFoundException($"Customer with ID {request.Id} not found.");
            }

            return customer;
        }
    }
}