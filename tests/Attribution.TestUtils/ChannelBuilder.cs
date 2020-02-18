using Attribution.Domain.Models;

namespace Attribution.TestUtils
{
    public class ChannelBuilder
    {
        private OptionValue<int> _id;
        private OptionValue<string> _name;
        
        public ChannelBuilder WithId(int id)
            => _id.WithValue(id, this);

        public ChannelBuilder WithName(string name)
            => _name.WithValue(name, this);
        
        public Channel Build()
        {
            return new Channel
            {
                Id = _id.OrValue(77),
                Name = _name.OrDefault()
            };
        }
    }
}