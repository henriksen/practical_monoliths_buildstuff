using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Commerce.SharedKernel.Infrastructure.Converters;

public class JsonValueConverter<TDomain>() : ValueConverter<TDomain, string>(
    domain => JsonSerializer.Serialize(domain, Options),
    provider => JsonSerializer.Deserialize<TDomain>(provider, Options) ?? default!)
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}

public static class AddressConverterExtensions
{
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        return propertyBuilder.HasConversion<JsonValueConverter<T>>()
            .HasColumnType("nvarchar(max)"); // or json if using PostgreSQL
    }

    public static void HasCollectionComparer<TCollection, TElement>(
        this PropertyBuilder<TCollection> propertyBuilder)
        where TCollection : IEnumerable<TElement>
        where TElement : notnull
    {
        propertyBuilder.Metadata.SetValueComparer(
            new ValueComparer<TCollection>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c
            ));
    }
}
