using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GuidConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrEmpty(stringValue))
            {
                stringValue = Guid.NewGuid().ToString();
            }
            if (Guid.TryParse(stringValue, out var guidValue))
            {
                return guidValue;
            }
        }
        throw new JsonException($"Unable to convert \"{reader.GetString()}\" to System.Guid.");
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}