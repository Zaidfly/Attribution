namespace Attribution.Dal.Redis
{
    /// <summary>
    /// Сообщение для инвалидации ключа в Redis
    /// </summary>
    public class RedisInvalidationMessage
    {
        public RedisInvalidationMessage() : this(string.Empty)
        {

        }

        public RedisInvalidationMessage(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Ключ, который нужно сбросить
        /// </summary>
        public string Key { get; set; }
    }
}