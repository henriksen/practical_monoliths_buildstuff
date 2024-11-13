using Commerce.SharedKernel.Domain;

using MediatR;

namespace Commerce.SharedKernel.Infrastructure.Data;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(List<AggregateRoot> aggregate);
}

public class DomainEventDispatcher(IMediator mediator)
    : IDomainEventDispatcher
{
    public async Task DispatchEventsAsync(List<AggregateRoot> aggregates)
    {
        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents).ToList();
        aggregates.ForEach(a => a.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }

    }
}


