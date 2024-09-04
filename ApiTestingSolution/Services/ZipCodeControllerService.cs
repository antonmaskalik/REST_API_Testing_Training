using ApiTestingSolution.ClientFactory;
using ApiTestingSolution.Enums;

namespace ApiTestingSolution.Services
{
    public class ZipCodeControllerService : BaseService
    {
        public ZipCodeControllerService(IReadApiClient readClient, IWriteApiClient writeClient)
            : base(readClient, writeClient) { }

        public async Task<HttpResponseMessage> GetAvailableZipCodesAsync()
        {
            var request = CreateRequest("zip-codes", HttpMethod.Get);
            return await ExecuteRequestAsync(request, Scope.Read);
        }

        public async Task<HttpResponseMessage> ExpendAvailableZipCodesAsync(List<string> zipCodes)
        {
            var request = CreateRequest("zip-codes/expand", HttpMethod.Post, zipCodes);
            return await ExecuteRequestAsync(request, Scope.Write);
        }
    }
}
