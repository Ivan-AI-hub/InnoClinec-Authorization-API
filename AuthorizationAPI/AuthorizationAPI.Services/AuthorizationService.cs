using AuthorizationAPI.Application.Commands.Users.ConfirmEmail;
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

        public async Task<ServiceValueResult<User>> SingUpPatientAsync(string email, string password, string rePassword, 
                                                                        CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CreateUser(email, password, rePassword, Role.Patient), cancellationToken);
            return new ServiceValueResult<User>(result);
        }

        public async Task<ServiceVoidResult> ConfirmEmailAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new ConfirmUserEmail(id), cancellationToken);
            return new ServiceVoidResult(result);
        }
    }
}
