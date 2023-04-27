using AuthorizationAPI.Application.Commands.Users.ConfirmEmail;
using AuthorizationAPI.Application.Commands.Users.Create;
using AuthorizationAPI.Application.Queries.Users.GetByEmailAndPassword;
using AuthorizationAPI.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationAPI.Services
{
    public class AuthorizationService
    {
        private IMediator _mediator;
        private readonly IConfiguration _configuration;
        public AuthorizationService(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
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
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var jwtSettings = _configuration.GetSection("JwtSettings");

            var key = Encoding.UTF8.GetBytes(jwtSettings.GetSection("issuerSigningKey").Value);

            var jwt = new JwtSecurityToken(
                    issuer: jwtSettings.GetSection("validIssuer").Value,
                    audience: jwtSettings.GetSection("validAudience").Value,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("expires").Value)),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

            return new ServiceValueResult<string>(new JwtSecurityTokenHandler().WriteToken(jwt));
        }
    }
}
