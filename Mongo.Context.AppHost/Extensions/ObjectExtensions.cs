using System.Text.Json;
using System;

namespace Mongo.Context.AppHost.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            if (source is null) return default(T);
            var serialized = JsonSerializer.Serialize(source);
            return JsonSerializer.Deserialize<T>(serialized);
        }

        public static string ToJson<T>(this T obj, bool addFormatting = false)
        {
            if (obj is null) return "{}";
            var jsonSettings = new System.Text.Json.JsonSerializerOptions();
            jsonSettings.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            jsonSettings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            jsonSettings.WriteIndented = addFormatting;
            return JsonSerializer.Serialize<T>(obj);
        }
    }
}
