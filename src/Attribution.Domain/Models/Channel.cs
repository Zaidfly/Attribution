namespace Attribution.Domain.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Contract { get; set; }
        public string PriceTerms { get; set; }
        public string PartnerName { get; set; }
        public string ContactsEmail { get; set; }
        public string ContactsPhone { get; set; }
        public string ContactsWebsite { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
    }
}