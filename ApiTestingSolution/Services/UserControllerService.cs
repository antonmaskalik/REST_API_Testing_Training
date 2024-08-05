using RestSharp;
using ApiTestingSolution.Enums;
using ApiTestingSolution.Models;
using ApiTestingSolution.Helpers;
using ApiTestingSolution.Logging;
using System.Net;

namespace ApiTestingSolution.Services
{
    public class UserControllerService : BaseService
    {
        public static (List<User> Users, HttpStatusCode StatusCode) GetAllUsers(int? olderThan = null, int? youngerThan = null, Sex? sex = null)
        {
            var request = CreateRequest("users", Method.Get);

            if (olderThan != null)
            {
                request.AddQueryParameter("olderThan", olderThan.ToString());
                Logger.Info($"Parameter olderThan={olderThan} is added to the GET request");
            }
            if (youngerThan != null)
            {
                request.AddQueryParameter("youngerThan", youngerThan.ToString());
                Logger.Info($"Parameter youngerThan={youngerThan} is added to the GET request");
            }
            if (sex != null)
            {
                request.AddQueryParameter("sex", sex.ToString().ToUpper());
                Logger.Info($"Parameter sex={sex.ToString()} is added to the GET request");
            }

            var response = ExecuteRequest(request, Scope.Read);
            var users = JsonHelper.DeserializeJsonContent<List<User>>(response);

            return (users, response.StatusCode);
        }

        public static RestResponse CreateUser(User user) => RequestUserUpdate(Method.Post, user);

        public static RestResponse UpdateUser(User userToUpdate, User userNewValues, Method method)
        {
            var body = new Dictionary<string, User>
            {
                { "userNewValues", userNewValues},
                { "userToChange", userToUpdate}
            };
            Logger.Info($"Trying to update user: {userToUpdate} to {userNewValues}");

            return RequestUserUpdate(method, body);
        }

        public static RestResponse DeleteUser(User userToDelete) => RequestUserUpdate(Method.Delete, userToDelete);

        public static RestResponse DeleteUser(string userToDelete) => RequestUserUpdate(Method.Delete, userToDelete);

        public static RestResponse UploadFileWithUsers(string filePath)
        {
            var request = CreateRequest("users/upload", Method.Post);            
            request.AddFile("file", filePath, "application/json");
            Logger.Info($"Upload file: {filePath}");

            return ExecuteRequest(request, Scope.Write);
        }

        private static RestResponse RequestUserUpdate(Method method, object body)
        {
            var request = CreateRequest("users", method, body);
            return ExecuteRequest(request, Scope.Write);
        }
    }
}
