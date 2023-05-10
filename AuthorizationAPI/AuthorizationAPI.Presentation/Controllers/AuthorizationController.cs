using AuthorizationAPI.Presentation.Models.ErrorModels;
using AuthorizationAPI.Services.Abstractions;
using AuthorizationAPI.Services.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationAPI.Presentation.Controllers
{
    [Route("/")]
    public class AuthorizationController : ControllerBase
    {
        private IAuthorizationService _authorizationService;
        private EmailService _emailService;
        public AuthorizationController(IAuthorizationService authorizationService, EmailService emailService)
        {
            _authorizationService = authorizationService;
            _emailService = emailService;
        }

        /// <summary>
        /// Registers the user in the system
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">User password</param>
        /// <param name="rePassword">User password for validation</param>
        [HttpPost("singUp")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> SingUpAsync(SingUpModel singUpModel, CancellationToken cancellationToken = default)
        {
            var user = await _authorizationService.SingUpAsync(singUpModel, RoleDTO.Patient, cancellationToken);
            await SendEmailVerificationMessageAsync(user, cancellationToken);
            return Ok(user);
        }

        /// <summary>
        /// Confirms email for the user with id = <paramref name="id"/>
        /// </summary>
        /// <param name="id">User id</param>
        [HttpGet("confirm/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> ConfirmEmailAsync(ConfirmEmailModel confirmEmailModel, CancellationToken cancellationToken = default)
        {
            await _authorizationService.ConfirmEmailAsync(confirmEmailModel, cancellationToken);
            return Ok();
        }

        /// <returns>access token for user witn same email and password</returns>
        [HttpGet("SingIn")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> SingInAsync(GetAccessTokenModel getAccessTokenModel, CancellationToken cancellationToken = default)
        {
            var accessToken = await _authorizationService.GetAccessTokenAsync(getAccessTokenModel, cancellationToken);
            return Ok(accessToken);
        }

        private async Task SendEmailVerificationMessageAsync(UserDTO user, CancellationToken cancellationToken = default)
        {
            string url = HttpContext.Request.Host.Value;
            await _emailService.SendEmailAsync(user.Email,
                                               "Confirm email address",
                                               $"<a href='https://{url}/confirm/{user.Id}'>Тыкни чтобы подтвердить</a>",
                                               cancellationToken);
        }
    }
}
