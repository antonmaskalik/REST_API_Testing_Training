using RestSharp;
using ApiTestingSolution.Enums;
using ApiTestingSolution.Models;
using ApiTestingSolution.Helpers;
using ApiTestingSolution.Logging;

namespace ApiTestingSolution.Services
{
    public class UserControllerService : BaseService
    {
        public static List<User> GetAllUsers()
        {
            var request = CreateRequest("users", Method.Get);
            var response = ExecuteRequest(request, Scope.Read);
            var users = JsonHelper.DeserializeJsonContent<List<User>>(response);
            Logger.Info($"The app contains following users: {users}");

            return users;
        }

        public static RestResponse CreateUser(User user)
        {
            var request = CreateRequest("users", Method.Post, user);
            var response = ExecuteRequest(request, Scope.Write);

            return response;
        }
    }
}
