using AuthorizationAPI.Application;

namespace AuthorizationAPI.Services
{
    public class ServiceResult<T> where T : class
    {
        public IList<string> Errors { get; }
        public T? Value { get; internal set; }
        public bool IsComplite => Errors.Count == 0;

        public ServiceResult(T? value = default, params string[] errors)
        {
            Value = value;
            Errors = errors.ToList();
        }

        public ServiceResult(ApplicationResult<T> applicationResult)
        {
            Value = applicationResult.Value;
            Errors = applicationResult.Errors;
        }
    }
}
