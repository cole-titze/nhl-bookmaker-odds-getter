namespace Services.RequestMaker
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message);
    }
}
