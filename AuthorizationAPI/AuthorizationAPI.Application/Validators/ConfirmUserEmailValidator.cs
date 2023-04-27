using AuthorizationAPI.Application.Commands.Users.ConfirmEmail;
using FluentValidation;

namespace AuthorizationAPI.Application.Validators
{
    public class ConfirmUserEmailValidator : AbstractValidator<ConfirmUserEmail>
    {
        public ConfirmUserEmailValidator() 
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
        }
    }
}
