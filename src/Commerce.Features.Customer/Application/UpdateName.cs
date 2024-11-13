using Commerce.Features.Customer.Data;
using Commerce.SharedKernel.Exceptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Commerce.Features.Customer.Application;

public static class UpdateName
{
    public record Command(Guid Id, string NewName) : IRequest;

    internal class Handler(CustomerDbContext dbContext)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request,
            CancellationToken cancellationToken)
        {
            var customer = await dbContext.Customers
                .SingleOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (customer == null)
            {
                throw new NotFoundException($"Customer with ID {request.Id} not found.");
            }

            customer.UpdateName(request.NewName);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}