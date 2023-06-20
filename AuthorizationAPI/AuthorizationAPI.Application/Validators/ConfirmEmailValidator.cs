using AuthorizationAPI.Application.Abstractions.Models;
using FluentValidation;

namespace AuthorizationAPI.Application.Validators
{
    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailModel>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
        }
    }
}
