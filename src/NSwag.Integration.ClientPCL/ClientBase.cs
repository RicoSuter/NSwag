using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NSwag.Integration.ClientPCL
{
    public class ClientBase
    {
        protected async Task<HttpClient> CreateHttpClientAsync(CancellationToken cancellationToken)
        {
            return new HttpClient();
        }

        protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            return new HttpRequestMessage();
        }

        protected void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            
        }
    }
}
