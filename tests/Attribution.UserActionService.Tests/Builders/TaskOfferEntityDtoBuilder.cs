using System;
using Attribution.UserActionService.Models;

namespace Attribution.UserActionService.Tests.Builders
{
    public class TaskOfferEntityDtoBuilder
    {
        private OptionValue<int> _id;
        private OptionValue<int> _creatorId;
        private OptionValue<Guid?> _guid;
        
        public TaskOfferEntityDtoBuilder WithId(int id) => _id.WithValue(id, this);
        public TaskOfferEntityDtoBuilder WithCreatorId(int creatorId) => _creatorId.WithValue(creatorId, this);
        public TaskOfferEntityDtoBuilder WithGuid() => _guid.WithValue(Guid.NewGuid(), this);
            
        public TaskOfferEntityDto Build()
        {
            return new TaskOfferEntityDto
            {
                Id = _id.OrDefault(),
                CreatorId = _creatorId.OrDefault(),
                Guid = _guid.OrDefault()
            };
        }
    }
}