using Attribution.UserActionService.Models.YouDoEvents;

namespace Attribution.UserActionService.Tests.Builders
{
    public abstract class EventBuilderBase<TBuilder, TEvent>
        where TBuilder : class
        where TEvent : YouDoEventBase, new()
    {
        private EventContextBuilder _contextBuilder;
        
        public TBuilder WithContext() => WithContext(new EventContextBuilder());
        
        public TBuilder WithContext(EventContextBuilder contextBuilder)
        {
            _contextBuilder = contextBuilder;
            return this as TBuilder;
        }

        public TEvent Build()
        {
            var eventItem = new TEvent
            {
                Context = _contextBuilder?.Build()
            };

            return CompleteBuild(eventItem);
        }

        protected abstract TEvent CompleteBuild(TEvent eventItem);
    }
}