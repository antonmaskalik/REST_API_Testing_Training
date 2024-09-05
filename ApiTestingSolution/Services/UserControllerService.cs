using ApiTestingSolution.Enums;
using ApiTestingSolution.Models;
using ApiTestingSolution.Logging;
using System.Net;
using ApiTestingSolution.ClientFactory;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ApiTestingSolution.Services
{
    public class UserControllerService : BaseService
    {
        public UserControllerService(IReadApiClient readClient, IWriteApiClient writeClient)
            : base(readClient, writeClient) { }

        public async Task<(List<User> Users, HttpStatusCode StatusCode)> GetAllUsersAsync(int? olderThan = null, int? youngerThan = null, Sex? sex = null)
        {
            var endpoint = "users";
            var queryParams = new List<string>();

            if (olderThan != null)
            {
                queryParams.Add($"olderThan={olderThan}");
                Logger.Info($"Parameter olderThan={olderThan} is added to the GET request");
            }
            if (youngerThan != null)
            {
                queryParams.Add($"youngerThan={youngerThan}");
                Logger.Info($"Parameter youngerThan={youngerThan} is added to the GET request");
            }
            if (sex != null)
            {
                queryParams.Add($"sex={sex.ToString().ToUpper()}");
                Logger.Info($"Parameter sex={sex} is added to the GET request");
            }

            if (queryParams.Any())
            {
                endpoint += "?" + string.Join("&", queryParams);
            }

            var request = CreateRequest(endpoint, HttpMethod.Get);
            var response = await ExecuteRequestAsync(request,Scope.Read);

            var users = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
            return (users, response.StatusCode);
        }

        public async Task<HttpResponseMessage> CreateUserAsync(User user) 
            => await RequestUserUpdateAsync(HttpMethod.Post, user);

        public async Task<HttpResponseMessage> UpdateUserAsync(User userToUpdate, User userNewValues, HttpMethod method)
        {
            var body = new Dictionary<string, User>
            {
                { "userNewValues", userNewValues },
                { "userToChange", userToUpdate }
            };
            Logger.Info($"Trying to update user: {userToUpdate} to {userNewValues}");

            return await RequestUserUpdateAsync(method, body);
        }

        public async Task<HttpResponseMessage> DeleteUserAsync(object userToDelete) 
            => await RequestUserUpdateAsync(HttpMethod.Delete, userToDelete);

        public async Task<HttpResponseMessage> UploadFileWithUsersAsync(string filePath)
        {
            var request = CreateRequest("users/upload", HttpMethod.Post);

            using (var memoryStream = new MemoryStream(File.ReadAllBytes(filePath)))
            {
                var content = new StreamContent(memoryStream);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var formData = new MultipartFormDataContent
                {
                    { content, "file", Path.GetFileName(filePath) }
                };

                request.Content = formData;
                Logger.Info($"Uploading file: {filePath}");
                return await ExecuteRequestAsync(request, Scope.Write);
            }
        }

        private async Task<HttpResponseMessage> RequestUserUpdateAsync(HttpMethod method, object body)
        {
            var request = CreateRequest("users", method, body);
            return await ExecuteRequestAsync(request,Scope.Write);
        }
    }
}
