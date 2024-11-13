using Commerce.SharedKernel.Domain;

using Microsoft.EntityFrameworkCore;

namespace Commerce.SharedKernel.Infrastructure.Data;

public abstract class DomainDbContext(DbContextOptions options, IDomainEventDispatcher domainEventDispatcher) : DbContext(options)
{
    public abstract string Schema { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        // Get all entity types that inherit from AggregateRoot
        var aggregateRootTypes = modelBuilder.Model
            .GetEntityTypes()
            .Where(t => typeof(AggregateRoot).IsAssignableFrom(t.ClrType));

        foreach (var entityType in aggregateRootTypes)
        {
            // Ignore the DomainEvents property on each entity
            modelBuilder.Entity(entityType.ClrType)
                .Ignore(nameof(AggregateRoot.DomainEvents));
        }

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        var entitiesWithEvents = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        await domainEventDispatcher.DispatchEventsAsync(entitiesWithEvents);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(true, cancellationToken);
    }
    public override int SaveChanges()
    {
        return SaveChangesAsync(true).GetAwaiter().GetResult();
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        return SaveChangesAsync(acceptAllChangesOnSuccess).GetAwaiter().GetResult();
    }
}
