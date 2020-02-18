using Attribution.Domain.Models;
using AutoMapper;

namespace Attribution.Dal.MapperProfiles
{
    public class ChannelProfile : Profile
    {
        public ChannelProfile()
        {
            CreateMap<ChannelDb, Channel>().ReverseMap();
        }
    }
}