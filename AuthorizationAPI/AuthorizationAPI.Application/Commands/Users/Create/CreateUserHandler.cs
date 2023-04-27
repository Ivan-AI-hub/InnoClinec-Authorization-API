﻿using AuthorizationAPI.Application.Interfaces;
using AuthorizationAPI.Application.StaticHelpers;
using AuthorizationAPI.Domain;
using FluentValidation;
using MediatR;

namespace AuthorizationAPI.Application.Commands.Users.Create
{
    internal class CreateUserHandler : IRequestHandler<CreateUser, ApplicationValueResult<User>>
    {
        private IRepositoryManager _repositoryManager;
        private AbstractValidator<CreateUser> _validator;
        public CreateUserHandler(IRepositoryManager repositoryManager, AbstractValidator<CreateUser> validator)
        {
            _repositoryManager = repositoryManager;
            _validator = validator;
        }

        public async Task<ApplicationValueResult<User>> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
                return new ApplicationValueResult<User>(validationResult);

            if (await _repositoryManager.UserRepository.IsItemExistAsync(x => x.Email == request.Email, cancellationToken))
                return new ApplicationValueResult<User>(null, "User with the same Email exist in the database.");

            var user = new User(request.Email, request.Role, Hacher.StringToHach(request.Password));

            _repositoryManager.UserRepository.Create(user);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            return new ApplicationValueResult<User>(user);
        }
    }
}