namespace AuthorizationAPI.Services.Results
{
    public interface IServiceResult
    {
        public IList<string> Errors { get; }
        public bool IsComplite { get; }
    }
}
