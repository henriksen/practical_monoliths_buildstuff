using Commerce.Features.Identity.Data;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Commerce.Features.Identity.Application;

public static class GetPage
{
    public record Query(int PageNumber, int PageSize) : IRequest<Response>;
    public record Response(List<ApplicationUserDto> Users, int TotalCount);

    public record ApplicationUserDto(string Id, string? Name, string? Email);

    internal class Handler(IdentityDbContext dbContext)
        : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var totalCount = await dbContext.Users.CountAsync(cancellationToken);

            var customers = await dbContext.Users
                .OrderBy(c => c.NormalizedEmail)
                .Skip((request.PageNumber) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new ApplicationUserDto(c.Id, c.NormalizedUserName, c.NormalizedEmail))
                .ToListAsync(cancellationToken);

            return new Response(customers, totalCount);
        }
    }
}