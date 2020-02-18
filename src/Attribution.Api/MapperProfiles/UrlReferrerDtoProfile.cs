using Attribution.Api.Dtos;
using Attribution.Domain.Models;
using AutoMapper;

namespace Attribution.Api.MapperProfiles
{
    public class UrlReferrerDtoProfile : Profile
    {
        public UrlReferrerDtoProfile()
        {
            CreateMap<UrlReferrer, UrlReferrerDto>().ReverseMap();
        }
    }
}