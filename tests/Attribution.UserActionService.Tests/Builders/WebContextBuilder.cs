using Attribution.UserActionService.Models.YouDoEvents;

namespace Attribution.UserActionService.Tests.Builders
{
    public class WebContextBuilder
    {
        private OptionValue<long?> _attributionDataHash;
        private OptionValue<long?> _attributionDataLingeringHash;
        
        public WebContextBuilder WithAttributionDataHash(long attributionDataHash) 
            => _attributionDataHash.WithValue(attributionDataHash, this);
        
        public WebContextBuilder WithLingeringAttributionDataHash(long lingeringAttributionDataHash) 
            => _attributionDataLingeringHash.WithValue(lingeringAttributionDataHash, this);

        public WebContext Build()
        {
            return new WebContext
            {
                AttributionDataHash = _attributionDataHash.OrDefault(),
                AttributionDataLingeringHash = _attributionDataLingeringHash.OrDefault()
            };
        }
    }
}