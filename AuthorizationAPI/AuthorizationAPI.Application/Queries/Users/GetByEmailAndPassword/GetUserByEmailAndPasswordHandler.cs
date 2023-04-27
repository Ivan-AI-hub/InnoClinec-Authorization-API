using AuthorizationAPI.Application.Interfaces;
using AuthorizationAPI.Application.StaticHelpers;
using AuthorizationAPI.Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationAPI.Application.Queries.Users.GetByEmailAndPassword
{
    internal class GetUserByEmailAndPasswordHandler : IRequestHandler<GetUserByEmailAndPassword, ApplicationValueResult<User>>
    {
        private IRepositoryManager _repositoryManager;
        private IValidator<GetUserByEmailAndPassword> _validator;
        public GetUserByEmailAndPasswordHandler(IRepositoryManager repositoryManager, IValidator<GetUserByEmailAndPassword> validator)
        {
            _repositoryManager = repositoryManager;
            _validator = validator;
        }
        public async Task<ApplicationValueResult<User>> Handle(GetUserByEmailAndPassword request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
                return new ApplicationValueResult<User>(validationResult);

            var user = await _repositoryManager.UserRepository
                .GetItemsByCondition(x => x.Email == request.Email && x.PasswordHach == Hacher.StringToHach(request.Password), false)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return new ApplicationValueResult<User>(null, "User does not exist.");

            return new ApplicationValueResult<User>(user);
        }
    }
}
