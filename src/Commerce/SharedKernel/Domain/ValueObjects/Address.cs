namespace Commerce.SharedKernel.Domain.ValueObjects;

public record Address
{
    public static Address Empty { get; } = new Address("", "", "");

    public string Street { get; private set; } = "";
    public string PostalCode { get; private set; } = "";
    public string PostalPlace { get; private set; } = "";

    public Address(string street, string postalCode, string postalPlace)
    {
        Street = street;
        PostalCode = postalCode;
        PostalPlace = postalPlace;
    }
}