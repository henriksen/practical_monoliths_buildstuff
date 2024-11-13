using Commerce.SharedKernel.Domain;
using Commerce.SharedKernel.Domain.ValueObjects;

namespace Commerce.Features.Customer.Domain
{
    internal class Customer : AggregateRoot
    {
        public string Name { get; private set; } = "";

        public Address Address { get; private set; } = Address.Empty;


        private readonly List<Address> _previousAddresses = [];
        public IReadOnlyCollection<Address> PreviousAddresses => _previousAddresses.AsReadOnly();

        public void UpdateAddress(Address newAddress)
        {
            if (Address != Address.Empty && !_previousAddresses.Contains(Address))
            {
                _previousAddresses.Add(Address);
            }

            Address = newAddress;
        }


        public Email Email { get; init; } = Email.Empty;

        public void Deactivate()
        {
            // Business logic for deactivating the customer
            IsActive = false;
        }

        public bool IsActive { get; private set; }

        public void UpdateName(string newName)
        {
            // Validation or business rules can be applied here
            Name = newName;
        }

        public static Customer Create(string name, Email email)
        {
            return new Customer() { Name = name, Address = Address.Empty, Email = email };
        }
    }
}