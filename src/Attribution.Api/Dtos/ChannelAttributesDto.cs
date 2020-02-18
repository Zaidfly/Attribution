using System.Runtime.Serialization;

namespace Attribution.Api.Dtos
{
    [DataContract]
    public class ChannelAttributesDto
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        
        [DataMember(Name = "hash")]
        public long Hash { get; set; }
        
        [DataMember(Name = "utmSource")]
        public string UtmSource { get; set; }
        
        [DataMember(Name = "utmMedium")]
        public string UtmMedium { get; set; }
        
        [DataMember(Name = "utmCampaign")]
        public string UtmCampaign { get; set; }
        
        [DataMember(Name = "utmTerm")]
        public string UtmTerm { get; set; }
        
        [DataMember(Name = "utmContent")]
        public string UtmContent { get; set; }
        
        [DataMember(Name = "utmAgency")]
        public string UtmAgency { get; set; }
        
        [DataMember(Name = "utmPartnerId")]
        public string UtmPartnerId { get; set; }
        
        [DataMember(Name = "utmCampaignId")]
        public string UtmCampaignId { get; set; }
        
        [DataMember(Name = "utmAdType")]
        public string UtmAdType { get; set; }
        
        [DataMember(Name = "channelId")]
        public int? ChannelId { get; set; }
        
        [DataMember(Name = "urlReferrerId")]
        public int? UrlReferrerId { get; set; }
        
        [DataMember(Name = "unparsedData")]
        public string UnparsedData { get; set; }
    }
}