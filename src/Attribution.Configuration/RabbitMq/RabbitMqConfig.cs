namespace Attribution.Configuration.RabbitMq
{
    public class RabbitMqConfig
    {
        public string Url { get; set; }
        public int ConnectionTimeout { get; set; }
        public int RecoveryInterval { get; set; }
    }
}