using Commerce.SharedKernel.Domain.Events;

namespace Commerce.SharedKernel.Domain;
public class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents
        = [];

    public IReadOnlyCollection<DomainEvent> DomainEvents
        => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(DomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}