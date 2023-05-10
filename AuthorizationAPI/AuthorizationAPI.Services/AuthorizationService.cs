using AuthorizationAPI.Application.Commands.Users.ConfirmEmail;
using AuthorizationAPI.Application.Commands.Users.Create;
using AuthorizationAPI.Application.Queries.Users.GetByEmailAndPassword;
using AuthorizationAPI.Domain;
using AuthorizationAPI.Domain.Exceptions;
using AuthorizationAPI.Services.Models;
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
        public async Task<User> SingUpPatientAsync(string email, string password, string rePassword,
                                                                        CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(new CreateUser(email, password, rePassword, Role.Patient), cancellationToken);
        }

        /// <summary>
        /// Confirms email for the user with id = <paramref name="id"/>
        /// </summary>
        /// <param name="id">User id</param>
        public async Task ConfirmEmailAsync(Guid id, CancellationToken cancellationToken = default)
        {
             await _mediator.Send(new ConfirmUserEmail(id), cancellationToken);
        }

        public async Task<string> GetAccessTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _mediator.Send(new GetUserByEmailAndPassword(email, password), cancellationToken);

            if (!user.IsEmailConfirmed)
                throw new UserEmailNotConfirmedException(user.Email);

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
