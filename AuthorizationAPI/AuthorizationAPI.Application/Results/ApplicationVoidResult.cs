using FluentValidation.Results;

namespace AuthorizationAPI.Application.Results
{
    public class ApplicationVoidResult : IApplicationResult
    {
        public IList<string> Errors { get; }
        public bool IsComplite => Errors.Count == 0;

        public ApplicationVoidResult(params string[] errors)
        {
            Errors = errors.ToList();
        }

        public ApplicationVoidResult(ValidationResult validationResult)
        {
            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
        }
    }
}
