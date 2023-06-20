using AuthorizationAPI.Application.Abstractions.Models;
using AuthorizationAPI.Domain;
using AutoMapper;

namespace AuthorizationAPI.Application.Mappings
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
