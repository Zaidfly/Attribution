using System;
using Attribution.UserActionService.Models;

namespace Attribution.UserActionService.Tests.Builders
{
    public class TaskAttributesDtoBuilder
    {
        private OptionValue<Guid?> _guid;

        public TaskAttributesDtoBuilder WithGuid(Guid? guid) => _guid.WithValue(guid, this);

        public TaskAttributesDto Build()
        {
            return new TaskAttributesDto
            {
                TaskGuid = _guid.OrDefault(),
            };
        }
    }
}