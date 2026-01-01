using Newtonsoft.Json;

namespace Genetec.BookHistory.Utilities
{
    public static class JsonSerializer
    {
        public static T? Deserialize<T>(string? value)
        {
            if (value == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
