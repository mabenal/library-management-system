using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

public class GuidConverterTests
{
    private readonly GuidConverter _converter;
    private readonly JsonSerializerOptions _options;

    public GuidConverterTests()
    {
        _converter = new GuidConverter();
        _options = new JsonSerializerOptions
        {
            Converters = { _converter }
        };
    }

    [Fact]
    public void Read_ShouldReturnGuid_WhenValidGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var json = $"\"{guid}\"";
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

        // Act
        reader.Read();
        var result = _converter.Read(ref reader, typeof(Guid), _options);

        // Assert
        Assert.Equal(guid, result);
    }

    [Fact]
    public void Read_ShouldReturnNewGuid_WhenEmptyString()
    {
        // Arrange
        var json = "\"\"";
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

        // Act
        reader.Read();
        var result = _converter.Read(ref reader, typeof(Guid), _options);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public void Read_ShouldThrowJsonException_WhenInvalidGuidString()
    {
        // Arrange
        var json = "\"invalid-guid\"";
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

        // Act
        reader.Read();
        JsonException ex = null;
        try
        {
            _converter.Read(ref reader, typeof(Guid), _options);
        }
        catch (JsonException e)
        {
            ex = e;
        }

        // Assert
        Assert.NotNull(ex);
    }

    [Fact]
    public void Write_ShouldWriteGuidAsString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        _converter.Write(writer, guid, _options);
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        Assert.Equal($"\"{guid}\"", json);
    }
}