using ApiTestingSolution.Enums;
using ApiTestingSolution.Logging;
using ApiTestingSolution.Models;
using ApiTestingSolution.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiTestingSolution.Helpers
{
    public static class UserHelpers
    {
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
        private static ZipCodeControllerService _zipCodeService;

        public static void Initialize(ZipCodeControllerService zipCodeService)
        {
            _zipCodeService = zipCodeService;
        }

        public static async Task<(List<User> Users, string FilePath)> CreateJsonFileWithCorrectUsersAsync()
        {
            var users = await GetRandomUsersListAsync();
            var jsonContent = JsonConvert.SerializeObject(users);

            return (users, CreateJsonFile(jsonContent));
        }

        public static async Task<(List<User> Users, string FilePath)> CreateJsonFileWithIncorrectZipCodeForUserAsync()
        {
            var users = await GetRandomUsersListAsync();
            var user = users.FirstOrDefault();
            if (user != null)
            {
                user.ZipCode = RandomHelper.GetRandomString();
            }
            var jsonContent = JsonConvert.SerializeObject(users);

            return (users, CreateJsonFile(jsonContent));
        }

        public static async Task<(List<User> Users, string FilePath)> CreateJsonFileWithMissedRequiredFieldAsync()
        {
            var users = await GetRandomUsersListAsync();
            var jsonContent = JsonConvert.SerializeObject(users);
            var jsonArray = JArray.Parse(jsonContent);
            var firstElement = (JObject)jsonArray.FirstOrDefault();
            if (firstElement != null)
            {
                firstElement.Remove("Name");
            }
            var modifiedJsonContent = jsonArray.ToString();

            return (users, CreateJsonFile(modifiedJsonContent));
        }

        public static async Task<List<User>> GetRandomUsersListAsync()
        {
            if (_zipCodeService == null)
                throw new InvalidOperationException("UserHelpers is not initialized with ZipCodeControllerService instance");

            var response = await _zipCodeService.GetAvailableZipCodesAsync();
            var codes = await JsonHelper.DeserializeJsonContentAsync<List<string>>(response);
            var users = new List<User>();

            foreach (var code in codes)
            {
                var user = new User
                {
                    Age = RandomHelper.GetRandomInt(1, 60),
                    Name = RandomHelper.GetRandomString(),
                    Sex = Sex.MALE,
                    ZipCode = code
                };
                users.Add(user);
            }

            return users;
        }

        private static string CreateJsonFile(string jsonContent)
        {
            try
            {
                File.WriteAllText(filePath, jsonContent);
                Logger.Info("The JSON file was successfully created along the path: " + filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                Logger.Error("JSON file is not created", ex);
                return null;
            }
        }
    }
}
