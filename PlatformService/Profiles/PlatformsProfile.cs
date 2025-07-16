using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Entities;

namespace PlatformService.Profiles
{
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            // Source --> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();  
        }
    }
}
