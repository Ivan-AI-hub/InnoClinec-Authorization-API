using AuthorizationAPI.Application;

namespace AuthorizationAPI.Services
{
    public class ServiceValueResult<T> where T : class
    {
        public IList<string> Errors { get; }
        public T? Value { get; internal set; }
        public bool IsComplite => Errors.Count == 0;

        public ServiceValueResult(T? value = default, params string[] errors)
        {
            Value = value;
            Errors = errors.ToList();
        }

        public ServiceValueResult(ApplicationValueResult<T> applicationResult)
        {
            Value = applicationResult.Value;
            Errors = applicationResult.Errors;
        }
    }
}
