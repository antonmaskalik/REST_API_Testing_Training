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
        private readonly static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");

        public static (List<User> Users, string FilePath) CreateJsonFileWithCorrectUsers()
        {
            var users = GetRandomUsersList();
            var jsonContent = JsonConvert.SerializeObject(users);

            return (users, CreateJsonFile(jsonContent));
        }

        public static (List<User> Users, string FilePath) CreateJsonFileWithIncorrectZipCodeForUser()
        {
            var users = GetRandomUsersList();
            users.FirstOrDefault().ZipCode = RandomHelper.GetRandomString();
            var jsonContent = JsonConvert.SerializeObject(users);

            return (users, CreateJsonFile(jsonContent));
        }

        public static (List<User> Users, string FilePath) CreateJsonFileWithMissedRequiredField()
        {
            var users = GetRandomUsersList();
            var jsonContent = JsonConvert.SerializeObject(users);
            var jsonArray = JArray.Parse(jsonContent);
            var firstElement = (JObject)jsonArray.FirstOrDefault();
            firstElement.Remove("name");
            var modifiedJsonContent = jsonArray.ToString();

            return (users, CreateJsonFile(modifiedJsonContent));
        }

        public static List<User> GetRandomUsersList()
        {
            var users = new List<User>();
            var zipCodes = ZipCodeControllerService.GetAvailableZipCodes();
            var codes = JsonHelper.DeserializeJsonContent<List<string>>(zipCodes);

            foreach (var code in codes)
            {
                var user = new User()
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
