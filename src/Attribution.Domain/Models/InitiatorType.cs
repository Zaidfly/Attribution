using System.ComponentModel;

namespace Attribution.Domain.Models
{
    /// <summary>
    /// Тип инициатора операции
    /// </summary>
    public enum InitiatorType
    {
        /// <summary>
        /// Система
        /// </summary>
        [Description("Операция совершена системой")]
        System = 1,

        /// <summary>
        /// Пользователь
        /// </summary>
        [Description("Операция совершена рядовым пользователем")]
        User = 10,

        /// <summary>
        /// Модератор
        /// </summary>
        [Description("Операция совершена модератором")]
        Moderator = 20
    }
}