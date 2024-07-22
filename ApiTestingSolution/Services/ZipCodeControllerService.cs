using ApiTestingSolution.Enums;
using RestSharp;

namespace ApiTestingSolution.Services
{
    public class ZipCodeControllerService : BaseService
    {
        public static RestResponse GetAvailableZipCodes()
        {
            var request = CreateRequest("zip-codes", Method.Get);

            return ExecuteRequest(request, Scope.Read);
        }

        public static RestResponse ExpendAvailableZipCodes(List<string> zipCodes)
        {
            var request = CreateRequest("zip-codes/expand", Method.Post, zipCodes);

            return ExecuteRequest(request, Scope.Write);
        }
    }
}
