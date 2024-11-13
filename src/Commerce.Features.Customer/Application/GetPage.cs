using Commerce.Features.Customer.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Commerce.Features.Customer.Application;

public static class GetPage
{
    public record Query(int PageNumber, int PageSize) : IRequest<Response>;
    public record Response(List<CustomerDto> Customers, int TotalCount);

    public record CustomerDto(Guid Id, string Name, string Email);

    internal class Handler(CustomerDbContext dbContext)
        : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var totalCount = await dbContext.Customers.CountAsync(cancellationToken);

            var customers = await dbContext.Customers
                .OrderBy(c => c.Name)
                .Skip((request.PageNumber) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CustomerDto(c.Id, c.Name, c.Email))
                .ToListAsync(cancellationToken);

            return new Response(customers, totalCount);
        }
    }
}