using System.Runtime.Serialization;

namespace Attribution.Api.Dtos
{
    [DataContract]
    public class ChannelAttributesTitleDto
    {
        [DataMember(Name = "title")] 
        public string Title { get; set; }

        [DataMember(Name = "channelName")] 
        public string ChannelName { get; set; }
    }
}