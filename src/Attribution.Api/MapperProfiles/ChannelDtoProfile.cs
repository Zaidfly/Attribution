using Attribution.Api.Dtos;
using Attribution.Domain.Models;
using AutoMapper;

namespace Attribution.Api.MapperProfiles
{
    public class ChannelDtoProfile : Profile
    {
        public ChannelDtoProfile()
        {
            CreateMap<Channel, ChannelDto>().ReverseMap();
            
            CreateMap<Channel, ChannelWithIdDto>()
                .IncludeBase<Channel, ChannelDto>()
                .ReverseMap();
        }
    }
}