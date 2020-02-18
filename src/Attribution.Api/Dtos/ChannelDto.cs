using System.Runtime.Serialization;

namespace Attribution.Api.Dtos
{
    [DataContract]
    public class ChannelDto
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "owner")]
        public string Owner { get; set; }
        
        [DataMember(Name = "contract")]
        public string Contract { get; set; }
        
        [DataMember(Name = "priceTerms")]
        public string PriceTerms { get; set; }
        
        [DataMember(Name = "partnerName")]
        public string PartnerName { get; set; }
        
        [DataMember(Name = "contactsEmail")]
        public string ContactsEmail { get; set; }
        
        [DataMember(Name = "contactsPhone")]
        public string ContactsPhone { get; set; }
        
        [DataMember(Name = "contactsWebsite")]
        public string ContactsWebsite { get; set; }
        
        [DataMember(Name = "description")]
        public string Description { get; set; }
        
        [DataMember(Name = "comments")]
        public string Comments { get; set; }
    }

    [DataContract]
    public class ChannelWithIdDto : ChannelDto
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
    }
}