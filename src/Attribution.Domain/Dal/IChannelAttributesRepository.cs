using System.Threading.Tasks;
using Attribution.Domain.Models;

namespace Attribution.Domain.Dal
{
    public interface IChannelAttributesRepository
    {
        Task<ChannelAttributes> GetByHashAsync(long hash);
        Task<long> GetOrAddChannelAttributesIdAsync(long hash);
        Task SaveChannelAttributesAsync(ChannelAttributes channelAttributes);
        Task<long> GetDirectChannelAttributesIdAsync();
    }
}