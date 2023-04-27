using AuthorizationAPI.Application.Commands.Users.Create;
using AuthorizationAPI.Domain;
using MediatR;

namespace AuthorizationAPI.Services
{
    public class AuthorizationService
    {
        private IMediator _mediator;
        public AuthorizationService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ServiceResult<User>> SingUpPatientAsync(string email, string password, string rePassword)
        {
            var result = await _mediator.Send(new CreateUser(email, password, rePassword, Role.Patient));
            return new ServiceResult<User>(result);
        }
    }
}
