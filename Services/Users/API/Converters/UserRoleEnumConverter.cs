using System.Text.Json;
using System.Text.Json.Serialization;
using Users.Domain;
using Users.Domain.Exceptions;

namespace Users.API.Converters;

public class UserRoleJsonConverter : JsonConverter<UserRole>
{
    public override UserRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString();
            if (Enum.TryParse<UserRole>(value,true,out var role))
                return role;
            else
                throw new InvalidUserRoleException(value,Enum.GetNames<UserRole>());
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            int value = reader.GetInt32();
            if (Enum.IsDefined(typeof(UserRole),value))
                return (UserRole)value;
            else
            {
                var values = Enum.GetValues<UserRole>().Cast<int>().ToArray();
                throw new InvalidUserRoleException(value,values);
            }
        }
        else
            throw new JsonException("Role field must be of type string or number");
    }

    public override void Write(Utf8JsonWriter writer, UserRole value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}