using AuthorizationAPI.Application.Commands.Users.ConfirmEmail;
using AuthorizationAPI.Application.Commands.Users.Create;
using AuthorizationAPI.Application.Queries.Users.GetByEmailAndPassword;
using AuthorizationAPI.Domain;
using AuthorizationAPI.Services.Models;
using AuthorizationAPI.Services.Results;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationAPI.Services
{
    public class AuthorizationService
    {
        private IMediator _mediator;
        private readonly JwtSettings _jwtSettings;
        public AuthorizationService(IMediator mediator, IOptions<JwtSettings> jwtSettings)
        {
            _mediator = mediator;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Registers the patient in system
        /// </summary>
        /// <param name="email">patient email</param>
        /// <param name="password">patient password</param>
        /// <param name="rePassword">patient repassword</param>
        public async Task<ServiceValueResult<User>> SingUpPatientAsync(string email, string password, string rePassword,
                                                                        CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CreateUser(email, password, rePassword, Role.Patient), cancellationToken);
            return new ServiceValueResult<User>(result);
        }

        /// <summary>
        /// Confirms email for the user with id = <paramref name="id"/>
        /// </summary>
        /// <param name="id">User id</param>
        public async Task<ServiceVoidResult> ConfirmEmailAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new ConfirmUserEmail(id), cancellationToken);
            return new ServiceVoidResult(result);
        }

        public async Task<ServiceValueResult<string>> GetAccessTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUserByEmailAndPassword(email, password), cancellationToken);
            if (!result.IsComplite)
                return new ServiceValueResult<string>(null, result.Errors.ToArray());

            var user = result.Value;

            if (!user.IsEmailConfirmed)
                return new ServiceValueResult<string>(null, "User`s email has not been confirmed");

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

            return new ServiceValueResult<string>(new JwtSecurityTokenHandler().WriteToken(jwt));
        }
    }
}
