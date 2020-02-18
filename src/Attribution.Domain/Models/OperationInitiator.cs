namespace Attribution.Domain.Models
{
    /// <summary>
    /// Инициатор операции в рамках которой возникло событие
    /// </summary>
    public sealed class OperationInitiator
    {
        /// <summary>
        /// Идентификатор пользователя, если тип User/Moderator
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Тип инициатора
        /// </summary>
        public InitiatorType Type { get; set; }
    }
}