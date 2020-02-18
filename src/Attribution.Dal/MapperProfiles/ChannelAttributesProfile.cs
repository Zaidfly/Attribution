using Attribution.Domain.Models;
using AutoMapper;

namespace Attribution.Dal.MapperProfiles
{
    public class ChannelAttributesProfile : Profile
    {
        public ChannelAttributesProfile()
        {
            CreateMap<ChannelAttributesDb, ChannelAttributes>().ReverseMap();
        }
    }
}