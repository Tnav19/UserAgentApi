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
               .ForMember(dest => dest.Agent, opt => opt.Ignore()).ReverseMap();

            CreateMap<Agent, AgentDto>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users)) // Map Users to UserDto
            .ReverseMap();

            CreateMap<User, UserCreateDto>().ReverseMap();

            CreateMap<Agent, AgentCreateDto>().ReverseMap();
        }
    }
}
