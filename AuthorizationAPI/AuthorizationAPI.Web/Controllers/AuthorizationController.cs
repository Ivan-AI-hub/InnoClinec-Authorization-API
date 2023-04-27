using Microsoft.AspNetCore.Mvc;

namespace AuthorizationAPI.Web.Controllers
{
    [Route("/")]
    public class AuthorizationController : Controller
    {
        public AuthorizationController() 
        {
        }

        public Task<IActionResult> SingUp() 
        {
        }
    }
}
