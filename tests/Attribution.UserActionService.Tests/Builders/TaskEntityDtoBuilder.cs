using System;
using Attribution.UserActionService.Models;

namespace Attribution.UserActionService.Tests.Builders
{
    public class TaskEntityDtoBuilder
    {
        private OptionValue<int> _id;
        private OptionValue<int> _creatorId;
        private OptionValue<Guid?> _guid;
        
        public TaskEntityDtoBuilder WithId(int id) => _id.WithValue(id, this);
        public TaskEntityDtoBuilder WithCreatorId(int creatorId) => _creatorId.WithValue(creatorId, this);
        public TaskEntityDtoBuilder WithGuid() => _guid.WithValue(Guid.NewGuid(), this);
        
        public TaskEntityDto Build()
        {
            return new TaskEntityDto
            {
                Id = _id.OrDefault(),
                CreatorId = _creatorId.OrDefault(),
                Attributes = new TaskAttributesDtoBuilder().WithGuid(_guid.OrDefault()).Build()
            };
        }
    }
}