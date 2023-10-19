namespace Services.RequestMaker
{
    public interface IRequestMaker
    {
        public Task<dynamic?> MakeRequest(string url, string query);
    }
}

