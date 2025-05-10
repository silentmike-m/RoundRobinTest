namespace Coda.RoundRobin.Application.Extensions;

using System.Text.Json;

public static class StringExtensions
{
    private static readonly JsonSerializerOptions DEFAULT_JSON_SERIALIZER_OPTIONS = new()
    {
        WriteIndented = true,
    };

    public static T? To<T>(this string source, JsonSerializerOptions? options = null)
    {
        var jsonOptions = options ?? DEFAULT_JSON_SERIALIZER_OPTIONS;

        return JsonSerializer.Deserialize<T>(source, jsonOptions) ?? default;
    }
}
