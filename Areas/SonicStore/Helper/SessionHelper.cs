using System.Text.Json;

namespace SonicStore.Areas.SonicStore.Helper
{
    public static class SessionHelper
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }
        public static T GetObject<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return json != null ? JsonSerializer.Deserialize<T>(json) : default;
        }
        public static void Remove(this ISession session, string key)
        {
            session.Remove(key);
        }
    }

}
