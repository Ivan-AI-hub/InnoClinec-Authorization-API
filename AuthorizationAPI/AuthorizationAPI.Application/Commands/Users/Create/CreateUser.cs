using AuthorizationAPI.Domain;
using MediatR;

namespace AuthorizationAPI.Application.Commands.Users.Create
{
    public record CreateUser(string Email,
                          string Password,
                          string RePassword,
                          Role Role) : IRequest<ApplicationValueResult<User>>;
}
