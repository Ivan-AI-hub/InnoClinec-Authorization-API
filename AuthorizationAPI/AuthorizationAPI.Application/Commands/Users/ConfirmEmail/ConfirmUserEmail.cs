using MediatR;

namespace AuthorizationAPI.Application.Commands.Users.ConfirmEmail
{
    public record ConfirmUserEmail(Guid Id) : IRequest<ApplicationVoidResult>;
}
