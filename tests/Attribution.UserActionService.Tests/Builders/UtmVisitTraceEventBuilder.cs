using Attribution.UserActionService.Models.YouDoEvents;

namespace Attribution.UserActionService.Tests.Builders
{
    public class UtmVisitTraceEventBuilder : EventBuilderBase<UtmVisitTraceEventBuilder, UtmVisitTraceEvent>
    {
        private OptionValue<long> _hash;
        private OptionValue<string> _attributionData;
        
        public UtmVisitTraceEventBuilder WithHash(long hash) => _hash.WithValue(hash, this);
        
        public UtmVisitTraceEventBuilder WithAttributionData(string attributionData) => _attributionData.WithValue(attributionData, this);
        
        protected override UtmVisitTraceEvent CompleteBuild(UtmVisitTraceEvent eventItem)
        {
            eventItem.Hash = _hash.OrValue(123L);
            eventItem.AttributionData = _attributionData.OrValue("utm_source=213");
            return eventItem;
        }
    }
}