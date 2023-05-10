using AuthorizationAPI.Domain.Exceptions;
using AuthorizationAPI.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AuthorizationAPI.Application.Commands.Users.ConfirmEmail
{
    internal class ConfirmUserEmailHandler : IRequestHandler<ConfirmUserEmail>
    {
        private IRepositoryManager _repositoryManager;
        private IValidator<ConfirmUserEmail> _validator;
        public ConfirmUserEmailHandler(IRepositoryManager repositoryManager, IValidator<ConfirmUserEmail> validator)
        {
            _repositoryManager = repositoryManager;
            _validator = validator;
        }

        public async Task<Unit> Handle(ConfirmUserEmail request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = _repositoryManager.UserRepository.GetItemsByCondition(x => x.Id == request.Id, true).FirstOrDefault();
            if (user == null)
                throw new UserNotFoundException(request.Id);

            if (user.IsEmailConfirmed)
                throw new UserEmailAlreadyConfirmed(user.Email);

            user.IsEmailConfirmed = true;
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            return new Unit();
        }
    }
}
