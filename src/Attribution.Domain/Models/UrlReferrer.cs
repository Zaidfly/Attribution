namespace Attribution.Domain.Models
{
    public class UrlReferrer
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int? ChannelId { get; set; }
    }
}