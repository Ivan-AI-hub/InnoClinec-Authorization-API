using AuthorizationAPI.Application.StaticHelpers;
using AuthorizationAPI.Domain;
using AuthorizationAPI.Domain.Exceptions;
using AuthorizationAPI.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace AuthorizationAPI.Application.Queries.Users.GetByEmailAndPassword
{
    internal class GetUserByEmailAndPasswordHandler : IRequestHandler<GetUserByEmailAndPassword, User>
    {
        private IRepositoryManager _repositoryManager;
        private IValidator<GetUserByEmailAndPassword> _validator;
        public GetUserByEmailAndPasswordHandler(IRepositoryManager repositoryManager, IValidator<GetUserByEmailAndPassword> validator)
        {
            _repositoryManager = repositoryManager;
            _validator = validator;
        }
        public Task<User> Handle(GetUserByEmailAndPassword request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = _repositoryManager.UserRepository
                .GetItemsByCondition(x => x.Email == request.Email && x.PasswordHach == Hacher.StringToHach(request.Password), false)
                .FirstOrDefault();

            if (user == null)
                throw new UserAuthenticationException();

            return Task.FromResult(user);
        }
    }
}
