using FluentValidation.Results;

namespace AuthorizationAPI.Application.Results
{
    public class ApplicationValueResult<T> : IApplicationResult
        where T : class
    {
        public IList<string> Errors { get; }
        public T? Value { get; internal set; }
        public bool IsComplite => Errors.Count == 0;

        public ApplicationValueResult(T? value = default, params string[] errors)
        {
            Value = value;
            Errors = errors.ToList();
        }

        public ApplicationValueResult(ValidationResult validationResult, T? value = default)
        {
            Value = value;
            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
        }
    }
}
