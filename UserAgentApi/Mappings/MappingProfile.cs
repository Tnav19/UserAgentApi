using AutoMapper;
using UserAgentApi.Dtos;
using UserAgentApi.Models;

namespace UserAgentApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
               .ReverseMap();

            CreateMap<Agent, AgentDto>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
            .ReverseMap();

            CreateMap<User, UserCreateDto>().ReverseMap();

            CreateMap<Agent, AgentCreateDto>().ReverseMap();
        }
    }
}
