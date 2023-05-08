using AuthorizationAPI.Application.Results;
using AuthorizationAPI.Domain;
using MediatR;

namespace AuthorizationAPI.Application.Queries.Users.GetByEmailAndPassword
{
    public record GetUserByEmailAndPassword(string Email, string Password) : IRequest<ApplicationValueResult<User>>;
}
