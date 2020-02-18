using Attribution.Api.Dtos;
using Attribution.Domain.Models;
using AutoMapper;

namespace Attribution.Api.MapperProfiles
{
    public class ChannelAttributesDtoProfile : Profile
    {
        public ChannelAttributesDtoProfile()
        {
            CreateMap<ChannelAttributes, ChannelAttributesDto>().ReverseMap();
            CreateMap<ChannelAttributesTitle, ChannelAttributesTitleDto>().ReverseMap();
        }
    }
}