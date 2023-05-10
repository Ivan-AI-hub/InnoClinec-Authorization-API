using AuthorizationAPI.Domain;
using AuthorizationAPI.Domain.Exceptions;
using AuthorizationAPI.Domain.Repositories;
using AuthorizationAPI.Services.Models;
using AuthorizationAPI.Services.StaticHelpers;
using AuthorizationAPI.Services.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationAPI.Services
{
    public class AuthorizationService
    {
        private IRepositoryManager _repositoryManager;
        private readonly JwtSettings _jwtSettings;
        public AuthorizationService(IRepositoryManager repositoryManager, IOptions<JwtSettings> jwtSettings)
        {
            _repositoryManager = repositoryManager;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Registers the patient in system
        /// </summary>
        /// <param name="email">patient email</param>
        /// <param name="password">patient password</param>
        /// <param name="rePassword">patient repassword</param>
        public async Task<User> SingUpAsync(SingUpModel model, CancellationToken cancellationToken = default)
        {
            var validator = new SingUpValidator();
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            if (await _repositoryManager.UserRepository
                .IsItemExistAsync(x => x.Email == model.Email, cancellationToken))
                throw new EmailAreNotUniqueException();

            var user = new User(model.Email, model.Role, Hacher.StringToHach(model.Password));

            _repositoryManager.UserRepository.Create(user);
            await _repositoryManager.SaveChangesAsync(cancellationToken);

            return user;
        }

        /// <summary>
        /// Confirms email for the user with id = <paramref name="id"/>
        /// </summary>
        /// <param name="id">User id</param>
        public async Task ConfirmEmailAsync(ConfirmEmailModel model, CancellationToken cancellationToken = default)
        {
            var validator = new ConfirmEmailValidator();
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

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

        public async Task<string> GetAccessTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var validator = new GetUserByEmailAndPasswordValidator();
            var request = new GetUserByEmailAndPassword(email, password);

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = _repositoryManager.UserRepository
                    .GetItemsByCondition(x => x.Email == request.Email && x.PasswordHach == Hacher.StringToHach(request.Password), false)
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
    }
}
