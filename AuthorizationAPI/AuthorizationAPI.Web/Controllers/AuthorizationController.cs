using AuthorizationAPI.Services;
using AuthorizationAPI.Web.Models.ErrorModels;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationAPI.Web.Controllers
{
    [Route("/")]
    public class AuthorizationController : Controller
    {
        private AuthorizationService _authorizationService;
        public AuthorizationController(AuthorizationService authorizationService) 
        {
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Route("singUp")]
        public async Task<IActionResult> SingUpAsync(string email, string password, string rePassword) 
        {
            var result = await _authorizationService.SingUpPatientAsync(email, password, rePassword);
            if (!result.IsComplite)
                return BadRequest(new ErrorDetails(400, result.Errors));

            return Ok();
        }
    }
}
