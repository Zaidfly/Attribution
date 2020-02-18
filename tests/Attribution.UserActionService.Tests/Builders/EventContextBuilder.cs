using System;
using Attribution.Domain.Models;
using Attribution.UserActionService.Models.YouDoEvents;

namespace Attribution.UserActionService.Tests.Builders
{
    public class EventContextBuilder
    {
        private OptionValue<DateTime> _timestamp;
        private OptionValue<ClientType> _clientType;
        private OptionValue<WebContextBuilder> _webContextBuilder;
        private OptionValue<OperationInitiatorBuilder> _initiatorBuilder;

        public EventContextBuilder WithTimeStamp(DateTime dateTime) => _timestamp.WithValue(dateTime, this);
        
        public EventContextBuilder WithClientType(ClientType clientType) => _clientType.WithValue(clientType, this);

        public EventContextBuilder WithWebContext() => WithWebContext(new WebContextBuilder());
        
        public EventContextBuilder WithWebContext(WebContextBuilder builder) => _webContextBuilder.WithValue(builder, this);
        
        public EventContextBuilder WithInitiator(OperationInitiatorBuilder builder) => _initiatorBuilder.WithValue(builder, this);
        
        
        public EventContext Build()
        {
            return new EventContext
            {
                Timestamp = _timestamp.OrDefault().ToUniversalTime(),
                ClientType = _clientType.OrDefault(),
                WebContext = _webContextBuilder.OrDefault()?.Build(),
                Initiator = _initiatorBuilder.OrDefault()?.Build()
            };
        }
    }
}