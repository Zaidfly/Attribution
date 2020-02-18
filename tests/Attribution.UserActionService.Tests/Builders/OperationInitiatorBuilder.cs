using Attribution.Domain.Models;

namespace Attribution.UserActionService.Tests.Builders
{
    public class OperationInitiatorBuilder
    {
        private OptionValue<int?> _id;
        private OptionValue<InitiatorType> _type;
        
        public OperationInitiatorBuilder WithInitiatorId(int initiatorId) => _id.WithValue(initiatorId, this);
        
        public OperationInitiatorBuilder WithInitiatorType(InitiatorType initiatorType) => _type.WithValue(initiatorType, this);

        public OperationInitiator Build()
        {
            return new OperationInitiator
            {
                Id = _id.OrDefault(),
                Type = _type.OrDefault()
            };
        }
    }
}