namespace Attribution.UserActionService.Models.YouDoEvents
{
    public class TaskCreatedEvent : YouDoEventBase
    {
        public TaskEntityDto NewTaskEntity { get; set; }
    }
}