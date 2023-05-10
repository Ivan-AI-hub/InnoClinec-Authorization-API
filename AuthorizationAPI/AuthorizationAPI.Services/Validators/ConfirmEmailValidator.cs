using AuthorizationAPI.Services.Abstractions.Models;
using FluentValidation;

namespace AuthorizationAPI.Services.Validators
{
    internal class ConfirmEmailValidator : AbstractValidator<ConfirmEmailModel>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
        }
    }
}
