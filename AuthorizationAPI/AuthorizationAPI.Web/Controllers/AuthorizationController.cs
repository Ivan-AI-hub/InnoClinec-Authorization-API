using AuthorizationAPI.Services;
using AuthorizationAPI.Web.Models.ErrorModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace AuthorizationAPI.Web.Controllers
{
    [Route("/")]
    public class AuthorizationController : Controller
    {
        private AuthorizationService _authorizationService;
        private EmailService _emailService;
        public AuthorizationController(AuthorizationService authorizationService, EmailService emailService)
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
        public async Task<IActionResult> SingUpAsync(string email, string password, string rePassword, CancellationToken cancellationToken = default)
        {
            var result = await _authorizationService.SingUpPatientAsync(email, password, rePassword, cancellationToken);
            if (!result.IsComplite)
                return BadRequest(new ErrorDetails(400, result.Errors));

            string url = HttpContext.Request.Host.Value;
            await _emailService.SendEmailAsync(result.Value.Email,
                                               "Confirm email address",
                                               $"<a href='https://{url}/confirm/{result.Value.Id}'>Тыкни чтобы подтвердить</a>",
                                               cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Confirms email for the user with id = <paramref name="id"/>
        /// </summary>
        /// <param name="id">User id</param>
        [HttpGet("confirm/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> ConfirmEmailAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _authorizationService.ConfirmEmailAsync(id, cancellationToken);
            if (!result.IsComplite)
                return BadRequest(new ErrorDetails(400, result.Errors));

            return Ok();
        }

        /// <returns>access token for user witn same email and password</returns>
        [HttpGet("SingIn")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> SingInAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var result = await _authorizationService.GetAccessTokenAsync(email, password, cancellationToken);
            if (!result.IsComplite)
                return BadRequest(new ErrorDetails(400, result.Errors));

            return Json(result.Value);
        }
    }
}
