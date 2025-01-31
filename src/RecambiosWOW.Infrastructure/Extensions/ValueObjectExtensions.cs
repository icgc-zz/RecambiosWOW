using System.Text.Json;

namespace RecambiosWOW.Infrastructure.Extensions;

public static class ValueObjectExtensions
{
    public static T FromJson<T>(string json) where T : class
    {
        if (string.IsNullOrEmpty(json))
            return null;
        return JsonSerializer.Deserialize<T>(json);
    }

    public static string ToJson<T>(T obj) where T : class
    {
        if (obj == null)
            return null;
        return JsonSerializer.Serialize(obj);
    }
}