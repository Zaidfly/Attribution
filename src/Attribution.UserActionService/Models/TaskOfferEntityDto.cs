using System;

namespace Attribution.UserActionService.Models
{
    public class TaskOfferEntityDto
    {
        public int Id { get; set; }
        public Guid? Guid { get; set; }
        public int CreatorId { get; set; }
    }
}