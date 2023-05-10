using AuthorizationAPI.Domain;
using AuthorizationAPI.Services.Abstractions.Models;
using AutoMapper;

namespace AuthorizationAPI.Services.Mappings
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
