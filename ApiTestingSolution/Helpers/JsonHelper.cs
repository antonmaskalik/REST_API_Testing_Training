using Newtonsoft.Json;

namespace ApiTestingSolution.Helpers
{
    public static class JsonHelper
    {
        public static async Task<T> DeserializeJsonContentAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(content);
                }
                catch (JsonException ex)
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }
    }
}
