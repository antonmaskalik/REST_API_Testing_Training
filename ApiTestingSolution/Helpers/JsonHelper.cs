using Newtonsoft.Json;
using RestSharp;

namespace ApiTestingSolution.Helpers
{
    public static class JsonHelper
    {
        public static T DeserializeJsonContent<T>(RestResponse response)
        {
            if (!string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(response.Content);
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
