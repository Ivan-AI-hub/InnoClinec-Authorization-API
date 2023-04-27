using AuthorizationAPI.Services;
using AuthorizationAPI.Web.Models.ErrorModels;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost]
        [Route("singUp")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> SingUpAsync(string email, string password, string rePassword, CancellationToken cancellationToken = default)
        {
            var result = await _authorizationService.SingUpPatientAsync(email, password, rePassword, cancellationToken);
            if (!result.IsComplite)
                return BadRequest(new ErrorDetails(400, result.Errors));

            //string url = HttpContext.Request.GetEncodedUrl();
            //await _emailService.SendEmailAsync(result.Value.Email,
            //                                   "Confirm email address",
            //                                   $"<a href='{url}/confirm/{result.Value.Id}'>Тыкни чтобы подтвердить</a>",
            //                                   cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Confirms email for the user with id = <paramref name="id"/>
        /// </summary>
        /// <param name="id">User id</param>
        [HttpPost]
        [Route("confirm/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        public async Task<IActionResult> ConfirmEmailAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _authorizationService.ConfirmEmailAsync(id, cancellationToken);
            if (!result.IsComplite)
                return BadRequest(new ErrorDetails(400, result.Errors));

            return Ok();
        }
    }
}
