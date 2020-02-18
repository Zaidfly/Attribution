using Attribution.UserActionService.Models.YouDoEvents;

namespace Attribution.UserActionService.Tests.Builders
{
    public class TaskOfferCreatedEventBuilder : EventBuilderBase<TaskOfferCreatedEventBuilder, TaskOfferCreatedEvent>
    {
        private TaskOfferEntityDtoBuilder _taskOfferEntityDtoBuilder;
        
        public TaskOfferCreatedEventBuilder WithTaskOfferEntityDto() => WithTaskOfferEntityDto(new TaskOfferEntityDtoBuilder());
        public TaskOfferCreatedEventBuilder WithTaskOfferEntityDto(TaskOfferEntityDtoBuilder taskOfferEntityDtoBuilder)
        {
            _taskOfferEntityDtoBuilder = taskOfferEntityDtoBuilder;
            return this;
        }
        
        protected override TaskOfferCreatedEvent CompleteBuild(TaskOfferCreatedEvent eventItem)
        {
            eventItem.TaskOffer = _taskOfferEntityDtoBuilder?.Build();
            return eventItem;
        }
    }
}