using AuthorizationAPI.Application.Abstractions;
using AuthorizationAPI.Application.Abstractions.Models;
using AuthorizationAPI.Application.Settings;
using AuthorizationAPI.Application.StaticHelpers;
using AuthorizationAPI.Domain;
using AuthorizationAPI.Domain.Exceptions;
using AuthorizationAPI.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedEvents.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationAPI.Application
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidator<SingUpModel> _singUpValidator;
        private readonly IValidator<ConfirmEmailModel> _confirmEmailValidator;
        private readonly JwtSettings _jwtSettings;
        public AuthorizationService(IRepositoryManager repositoryManager, IMapper mapper, IPublishEndpoint publishEndpoint, IOptions<JwtSettings> jwtSettings,
                                    IValidator<SingUpModel> singUpValidator, IValidator<ConfirmEmailModel> confirmEmailValidator)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _singUpValidator = singUpValidator;
            _confirmEmailValidator = confirmEmailValidator;
            _jwtSettings = jwtSettings.Value;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<UserDTO> SingUpAsync(SingUpModel model, RoleDTO role, CancellationToken cancellationToken = default)
        {
            await ValidateModel(model, _singUpValidator, cancellationToken);

            if (await _repositoryManager.UserRepository
                .IsItemExistAsync(x => x.Email == model.Email, cancellationToken))
            {
                throw new EmailAreNotUniqueException();
            }

            var user = new User(model.Email, _mapper.Map<Role>(role), Hasher.StringToHash(model.Password));

            _repositoryManager.UserRepository.Create(user);
            await _publishEndpoint.Publish(new UserCreated(user.Id, user.Email, $"https://localhost:7191//authorization//confirm//{user.Id}"));

            return _mapper.Map<UserDTO>(user);
        }

        public async Task ConfirmEmailAsync(ConfirmEmailModel model, CancellationToken cancellationToken = default)
        {
            await ValidateModel(model, _confirmEmailValidator, cancellationToken);

            var user = _repositoryManager.UserRepository
                .GetItemsByCondition(x => x.Id == model.Id, false)
                .FirstOrDefault();

            if (user == null)
            {
                throw new UserNotFoundException(model.Id);
            }

            if (user.IsEmailConfirmed)
            {
                throw new UserEmailAlreadyConfirmed(user.Email);
            }

            user.IsEmailConfirmed = true;
            _repositoryManager.UserRepository.Update(user);
        }

        public string GetAccessToken(string email, string password)
        {
            var user = _repositoryManager.UserRepository
                    .GetItemsByCondition(x => x.Email == email && x.PasswordHach == Hasher.StringToHash(password), false)
                    .FirstOrDefault();

            if (user == null)
            {
                throw new UserAuthenticationException();
            }

            if (!user.IsEmailConfirmed)
            {
                throw new UserEmailNotConfirmedException(user.Email);
            }

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
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
