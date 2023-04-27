﻿using AuthorizationAPI.Application.Commands.Users.Create;
using FluentValidation;

namespace AuthorizationAPI.Application.Validators
{
    public class AddUserValidator : AbstractValidator<CreateUser>
    {
        private const string _emailRegex = "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";
        public AddUserValidator()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .Matches(_emailRegex);

            RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(6).MaximumLength(15);
            RuleFor(x => x.RePassword).NotNull().NotEmpty().MinimumLength(6).MaximumLength(15);

            RuleFor(x => x.Password).Matches(x => x.RePassword).WithMessage("Password and repassword are not matchs");
        }
    }
}
