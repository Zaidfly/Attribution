using System;

namespace Attribution.Domain.Models
{
    public class UserAction
    {
        public int UserId { get; set; }
        public DateTime ActionDateTimeUtc { get; set; }
        public UserActionType Type { get; set; }
        public ObjectType ObjectType { get; set; }
        public int? InitiatorId { get; set; }
        public InitiatorType? InitiatorType { get; set; }
        public int ObjectId { get; set; }
        public Guid? ObjectGuid { get; set; }
        public long ChannelAttributesId { get; set; }
        public long? ActualHash { get; set; }
        public long? LingeringHash { get; set; }
    }
}