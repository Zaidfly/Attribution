using System.Runtime.Serialization;
using Attribution.Domain.Models;

namespace Attribution.Api.Dtos
{
    [DataContract]
    public class UrlReferrerDto
    {
        public UrlReferrerDto()
        {
        }

        public UrlReferrerDto(UrlReferrer urlReferrer)
        {
            Host = urlReferrer.Host;
            ChannelId = urlReferrer.ChannelId;
        }

        [DataMember(Name = "host")]
        public string Host { get; set; }
        
        [DataMember(Name = "channelId")]
        public int? ChannelId { get; set; }
        
        public UrlReferrer ToUrlReferrer() => new UrlReferrer
        {
            Host = Host,
            ChannelId = ChannelId
        };
    }
}