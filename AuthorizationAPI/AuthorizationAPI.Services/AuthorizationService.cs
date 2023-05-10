﻿using AuthorizationAPI.Domain;
using AuthorizationAPI.Domain.Exceptions;
using AuthorizationAPI.Domain.Repositories;
using AuthorizationAPI.Services.Abstractions;
using AuthorizationAPI.Services.Abstractions.Models;
using AuthorizationAPI.Services.Settings;
using AuthorizationAPI.Services.StaticHelpers;
using AuthorizationAPI.Services.Validators;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationAPI.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private IRepositoryManager _repositoryManager;
        private IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        public AuthorizationService(IRepositoryManager repositoryManager, IMapper mapper, IOptions<JwtSettings> jwtSettings)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<UserDTO> SingUpAsync(SingUpModel model, RoleDTO role, CancellationToken cancellationToken = default)
        {
            await ValidateModel(model, new SingUpValidator(), cancellationToken);

            if (await _repositoryManager.UserRepository
                .IsItemExistAsync(x => x.Email == model.Email, cancellationToken))
                throw new EmailAreNotUniqueException();

            var user = new User(model.Email, _mapper.Map<Role>(role), Hacher.StringToHach(model.Password));

            _repositoryManager.UserRepository.Create(user);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task ConfirmEmailAsync(ConfirmEmailModel model, CancellationToken cancellationToken = default)
        {
            await ValidateModel(model, new ConfirmEmailValidator(), cancellationToken);

            var user = _repositoryManager.UserRepository
                .GetItemsByCondition(x => x.Id == model.Id, true)
                .FirstOrDefault();

            if (user == null)
                throw new UserNotFoundException(model.Id);

            if (user.IsEmailConfirmed)
                throw new UserEmailAlreadyConfirmed(user.Email);

            user.IsEmailConfirmed = true;
            await _repositoryManager.SaveChangesAsync(cancellationToken);
        }

        public string GetAccessToken(string email, string password)
        {
            var user = _repositoryManager.UserRepository
                    .GetItemsByCondition(x => x.Email == email && x.PasswordHach == Hacher.StringToHach(password), false)
                    .FirstOrDefault();

            if (user == null)
                throw new UserAuthenticationException();

            if (!user.IsEmailConfirmed)
                throw new UserEmailNotConfirmedException(user.Email);

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };


            var key = Encoding.UTF8.GetBytes(_jwtSettings.IssuerSigningKey);

            var jwt = new JwtSecurityToken(
                    issuer: _jwtSettings.ValidIssuer,
                    audience: _jwtSettings.ValidAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Expires),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task ValidateModel<Tmodel>(Tmodel model, IValidator<Tmodel> validator, CancellationToken cancellationToken = default)
        {
            var validationResult = await validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
    }
}
