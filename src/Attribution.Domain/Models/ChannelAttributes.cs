namespace Attribution.Domain.Models
{
    public class ChannelAttributes
    {
        public ChannelAttributes(long hash)
        {
            Hash = hash;
        }
        
        public long Id { get; set; }
        public long Hash { get; set; }
        
        public string UtmSource { get; set; }
        public string UtmMedium { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmTerm { get; set; }
        public string UtmContent { get; set; }
        public string UtmAgency { get; set; }
        public string UtmPartnerId { get; set; }
        public string UtmCampaignId { get; set; }
        public string UtmAdType { get; set; }
        
        public int? ChannelId { get; set; }
        public int? UrlReferrerId { get; set; }
        
        public string UnparsedData { get; set; }
    }
}