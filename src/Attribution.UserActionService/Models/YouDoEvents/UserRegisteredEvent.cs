using System;

namespace Attribution.UserActionService.Models.YouDoEvents
{
    public class UserRegisteredEvent : YouDoEventBase
    {
        public int UserId { get; set; }
        
        public Guid? UserGuid { get; set; }
    }
}