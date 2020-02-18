using System;
using Attribution.UserActionService.Models.YouDoEvents;

namespace Attribution.UserActionService.Tests.Builders
{
    public class UserRegisterEventBuilder : EventBuilderBase<UserRegisterEventBuilder, UserRegisteredEvent>
    {
        private OptionValue<int> _userId;
        private OptionValue<Guid?> _guid;
        
        public UserRegisterEventBuilder WithUserId(int userId) => _userId.WithValue(userId, this);
        public UserRegisterEventBuilder WithGuid() => _guid.WithValue(Guid.NewGuid(), this);

        protected override UserRegisteredEvent CompleteBuild(UserRegisteredEvent eventItem)
        {
            eventItem.UserId = _userId.OrDefault();
            eventItem.UserGuid = _guid.OrDefault();
            return eventItem;
        }
    }
}