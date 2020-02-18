using System.Threading.Tasks;

using Attribution.Domain.Models;

namespace Attribution.Domain.Managers
{
    public interface IChannelAttributesManager
    {
        Task<long> GetChannelAttributesIdAsync(long? hash);
        Task<ChannelAttributesTitle> GetTitleAsync(long hash);
        Task CreateChannelAttributesDataAsync(long hash, string attributionData = null);
    }
}