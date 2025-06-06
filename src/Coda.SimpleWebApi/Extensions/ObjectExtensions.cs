﻿namespace Coda.SimpleWebApi.Extensions;

using System.Text.Json;

internal static class ObjectExtensions
{
    private static JsonSerializerOptions WRITE_RAW_JSON_SERIALIZER_OPTIONS = new()
    {
        WriteIndented = false,
    };

    public static string ToJson<T>(this T source)
    {
        return JsonSerializer.Serialize(source, WRITE_RAW_JSON_SERIALIZER_OPTIONS);
    }
}
