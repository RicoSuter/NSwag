using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
    }
}
