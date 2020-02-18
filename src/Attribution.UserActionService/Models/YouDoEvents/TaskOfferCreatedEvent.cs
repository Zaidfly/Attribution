namespace Attribution.UserActionService.Models.YouDoEvents
{
    public class TaskOfferCreatedEvent : YouDoEventBase
    {
        public TaskOfferEntityDto TaskOffer { get; set; }
    }
}