using Attribution.UserActionService.Models.YouDoEvents;

namespace Attribution.UserActionService.Tests.Builders
{
    public class TaskCreatedEventBuilder : EventBuilderBase<TaskCreatedEventBuilder, TaskCreatedEvent>
    {
        private OptionValue<TaskEntityDtoBuilder> _taskEntityDtoBuilder;
        
        public TaskCreatedEventBuilder WithTaskEntityDto() => WithTaskEntityDto(new TaskEntityDtoBuilder());

        public TaskCreatedEventBuilder WithTaskEntityDto(TaskEntityDtoBuilder builder) =>
            _taskEntityDtoBuilder.WithValue(builder, this);
        
        protected override TaskCreatedEvent CompleteBuild(TaskCreatedEvent eventItem)
        {
            eventItem.NewTaskEntity = _taskEntityDtoBuilder.OrDefault()?.Build();
            return eventItem;
        }
    }
}