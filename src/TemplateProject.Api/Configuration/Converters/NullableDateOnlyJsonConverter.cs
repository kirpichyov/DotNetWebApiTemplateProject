using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TemplateProject.Api.Configuration.Converters;

public sealed class NullableDateOnlyJsonConverter : JsonConverter<DateOnly?>
{
    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out DateTime dateTime))
        {
            return DateOnly.FromDateTime(dateTime);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToString("O", CultureInfo.InvariantCulture));
    }
}