using System.Text.Json;
using System.Text.Json.Serialization;

namespace Commerce.SharedKernel.Domain.ValueObjects;

public record Email
{
    private const string EmptyValue = "";
    private const string InvalidValue = "**invalid**";
    private readonly string _value;

    public static Email Empty => new Email(EmptyValue);
    public static Email Invalid => new Email(InvalidValue);

    public static bool TryParse(string value, out Email email)
    {
        email = Email.Empty;
        if (!Validate(value))
        {
            return false;
        }

        email = new Email(value);
        return email != Email.Invalid;
    }

    public Email(string value)
    {
        if (!Validate(value))
        {
            _value = InvalidValue;
            return;
        }
        _value = value;
    }

    public static implicit operator string(Email email)
    {
        return email == Email.Invalid ? "Not valid" : email._value;
    }

    private static bool Validate(string email)
    {
        int atIndex = email.IndexOf('@');
        if (atIndex <= 0) // Must have an @ and content before it
            return false;

        int dotIndex = email.IndexOf('.', atIndex);
        if (dotIndex <= atIndex + 1) // Must have content between @ and .
            return false;

        return email.Length >= dotIndex + 3; // Must have at least 2 chars after dot
    }


    public class JsonConverter : JsonConverter<Email>
    {
        public override Email Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var email = reader.GetString();
            return email != null ? new Email(email) : Email.Invalid;
        }

        public override void Write(Utf8JsonWriter writer, Email value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value._value);
        }
    }
}