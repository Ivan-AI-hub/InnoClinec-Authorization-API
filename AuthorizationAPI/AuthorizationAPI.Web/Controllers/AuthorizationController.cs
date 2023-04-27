using AuthorizationAPI.Services;
using AuthorizationAPI.Web.Models.ErrorModels;
using Microsoft.AspNetCore.Http.Extensions;
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

        [HttpPost]
        [Route("singUp")]
        public async Task<IActionResult> SingUpAsync(string email, string password, string rePassword) 
        {
            var result = await _authorizationService.SingUpPatientAsync(email, password, rePassword);
            if (!result.IsComplite)
                return BadRequest(new ErrorDetails(400, result.Errors));

            string url = HttpContext.Request.GetEncodedUrl();
            await _emailService.SendEmailAsync(result.Value.Email,
                                               "Confirm email address",
                                               $"<a href='{url}/confirm/{result.Value.Id}'>Тыкни чтобы подтвердить</a>");
            return Ok();
        }

        [HttpPost]
        [Route("confirm/{id}")]
        public async Task<IActionResult> ConfirmEmailAsync(Guid id)
        {
            var result = await _authorizationService.ConfirmEmailAsync(id);
            if (!result.IsComplite)
                return BadRequest(new ErrorDetails(400, result.Errors));

            return Ok();
        }
    }
}
