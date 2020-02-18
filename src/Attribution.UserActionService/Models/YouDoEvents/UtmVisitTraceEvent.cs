namespace Attribution.UserActionService.Models.YouDoEvents
{
    public class UtmVisitTraceEvent : YouDoEventBase
    {
        public long Hash { get; set; }
        public string AttributionData { get; set; }
    }
}