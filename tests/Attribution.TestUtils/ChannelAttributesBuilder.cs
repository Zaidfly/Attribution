using Attribution.Domain.Models;

namespace Attribution.TestUtils
{
    public class ChannelAttributesBuilder
    {
        private OptionValue<long> _hash;
        private OptionValue<string> _utmSource;
        private OptionValue<string> _utmMedium;
        private OptionValue<string> _utmCampaign;
        private OptionValue<string> _utmTerm;
        private OptionValue<int?> _urlReferrerId;
        private OptionValue<int?> _channelId;

        public ChannelAttributesBuilder WithUtmSource(string utmSource)
            => _utmSource.WithValue(utmSource, this);

        public ChannelAttributesBuilder WithUtmMedium(string utmMedium)
            => _utmMedium.WithValue(utmMedium, this);
        
        public ChannelAttributesBuilder WithUtmCampaign(string utmCampaign)
            => _utmCampaign.WithValue(utmCampaign, this);

        public ChannelAttributesBuilder WithUtmTerm(string utmTerm)
            => _utmTerm.WithValue(utmTerm, this);

        public ChannelAttributesBuilder WithUrlReferrer(int urlReferrerId)
            => _urlReferrerId.WithValue(urlReferrerId, this); 
        
        public ChannelAttributesBuilder WithChannelId(int channelId)
            => _channelId.WithValue(channelId, this);

        public ChannelAttributes Build()
        {
            return new ChannelAttributes(_hash.OrDefault())
            {
                UtmSource = _utmSource.OrDefault(),
                UtmMedium = _utmMedium.OrDefault(),
                UtmCampaign = _utmCampaign.OrDefault(),
                UtmTerm = _utmTerm.OrDefault(),
                UrlReferrerId = _urlReferrerId.OrDefault(),
                ChannelId = _channelId.OrDefault()
            };
        }
    }
}