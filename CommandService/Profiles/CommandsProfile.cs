using AutoMapper;
using CommandService.DTOs;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles {
    public class CommandsProfile : Profile {
        public CommandsProfile() {
            //Source -> Target
            CreateMap<Platform, PlatformReadDTO>();
            CreateMap<Command, CommandReadDTO>();
            CreateMap<CommandCreateDTO, Command>();
            CreateMap<PlatformPublishedDTO, Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id));
            CreateMap<GrpcPlatformModel, Platform>().ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId));
        }
    }
}
