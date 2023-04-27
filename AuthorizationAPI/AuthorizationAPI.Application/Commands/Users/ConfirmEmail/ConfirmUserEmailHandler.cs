﻿using AuthorizationAPI.Application.Commands.Users.Create;
using AuthorizationAPI.Application.Interfaces;
using AuthorizationAPI.Domain;
using FluentValidation;
using MediatR;

namespace AuthorizationAPI.Application.Commands.Users.ConfirmEmail
{
    internal class ConfirmUserEmailHandler : IRequestHandler<ConfirmUserEmail, ApplicationVoidResult>
    {
        private IRepositoryManager _repositoryManager;
        private AbstractValidator<ConfirmUserEmail> _validator;
        public ConfirmUserEmailHandler(IRepositoryManager repositoryManager, AbstractValidator<ConfirmUserEmail> validator)
        {
            _repositoryManager = repositoryManager;
            _validator = validator;
        }

        public async Task<ApplicationVoidResult> Handle(ConfirmUserEmail request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
                return new ApplicationVoidResult(validationResult);

            var user = _repositoryManager.UserRepository.GetItemsByCondition(x => x.Id == request.Id, true).FirstOrDefault();
            if (user == null)
                return new ApplicationVoidResult("User with the same Id does not exist in the database.");

            user.IsEmailConfirmed = true;
            await _repositoryManager.SaveChangesAsync(cancellationToken);
            return new ApplicationVoidResult();
        }
    }
}