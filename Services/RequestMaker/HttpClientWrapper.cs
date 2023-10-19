using System.Net.Http;

namespace Services.RequestMaker
{
    public class HttpClientWrapper : IHttpClient
    {
        private static HttpClient _client = new HttpClient();
        public virtual async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message)
        {
            return await _client.SendAsync(message);
        }
    }
}
