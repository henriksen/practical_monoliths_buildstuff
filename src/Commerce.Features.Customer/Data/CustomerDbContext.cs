using Commerce.SharedKernel.Domain.ValueObjects;
using Commerce.SharedKernel.Infrastructure.Converters;
using Commerce.SharedKernel.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Commerce.Features.Customer.Data;

internal class CustomerDbContext(DbContextOptions<CustomerDbContext> options, IDomainEventDispatcher dispatcher)
    : DomainDbContext(options, dispatcher)
{
    public DbSet<Domain.Customer> Customers { get; set; } = null!;

    public override string Schema { get; } = "Customer";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Customer>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .HasJsonConversion()
                .IsRequired();

            entity.Property(e => e.PreviousAddresses)
                 .HasJsonConversion()
                 .IsRequired()
                 .HasCollectionComparer<IReadOnlyCollection<Address>, Address>();

            entity.OwnsOne<Address>(e => e.Address);
        });
    }
}