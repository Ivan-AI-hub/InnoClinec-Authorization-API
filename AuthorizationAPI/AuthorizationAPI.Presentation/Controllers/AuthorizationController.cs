using AuthorizationAPI.Application.Abstractions;
using AuthorizationAPI.Application.Abstractions.Models;
using AuthorizationAPI.Presentation.Models.ErrorModels;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationAPI.Presentation.Controllers
{
    [Route("authorization/")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        public AuthorizationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
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
        [ProducesResponseType(typeof(ErrorDetails), 500)]
        public async Task<IActionResult> SingUpAsync(SingUpModel singUpModel, CancellationToken cancellationToken = default)
        {
            var user = await _authorizationService.SingUpAsync(singUpModel, RoleDTO.Admin, cancellationToken);
            return Ok(user);
        }

        /// <summary>
        /// Updates role for specific user
        /// </summary>
        [HttpPut("{email}/role")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorDetails), 404)]
        [ProducesResponseType(typeof(ErrorDetails), 500)]
        public async Task<IActionResult> UpdateRole(string email, RoleDTO role, CancellationToken cancellationToken = default)
        {
             await _authorizationService.ChangeRoleAsync(email, role, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Confirms email for the user with id = <paramref name="id"/>
        /// </summary>
        /// <param name="id">User id</param>
        [HttpGet("confirm/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        [ProducesResponseType(typeof(ErrorDetails), 500)]
        public async Task<IActionResult> ConfirmEmailAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await _authorizationService.ConfirmEmailAsync(new ConfirmEmailModel(id), cancellationToken);
            return Ok();
        }

        /// <returns>access token for user witn same email and password</returns>
        [HttpGet("SingIn")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ErrorDetails), 400)]
        [ProducesResponseType(typeof(ErrorDetails), 500)]
        public IActionResult SingIn(string email, string password)
        {
            var accessToken = _authorizationService.GetAccessToken(email, password);
            return Ok(accessToken);
        }
    }
}
