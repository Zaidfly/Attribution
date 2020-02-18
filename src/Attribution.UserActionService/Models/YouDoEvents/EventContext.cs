using System;
using Attribution.Domain.Models;

namespace Attribution.UserActionService.Models.YouDoEvents
{
    public sealed class EventContext
    {
        private DateTime _timestamp;
        
        /// <summary>
        /// Точное время события в формате Utc
        /// </summary>
        public DateTime Timestamp 
        { 
            get => _timestamp;
            set => _timestamp = value == default ? DateTime.UtcNow : value;
        }

        /// <summary>
        /// Тип клиента
        /// </summary>
        public ClientType ClientType { get; set; }

        /// <summary>
        /// Инициатор события
        /// </summary>
        public OperationInitiator Initiator { get; set; }
        
        /// <summary>
        /// Дополнительные контекстные данные, описывающие веб-операцию
        /// </summary>
        public WebContext WebContext { get; set; }
    }
}