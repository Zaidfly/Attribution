using System.Collections.Generic;
using System.Threading.Tasks;
using Attribution.Domain.Models;

namespace Attribution.Domain.Dal
{
    public interface IChannelRepository
    {
        Task<bool> IsChannelIdExistsAsync(int channelId);
        int GetDirectChannelId();
        
        Task<IReadOnlyCollection<Channel>> GetAllAsync();
        Task<Channel> GetByIdAsync(int id);
        Task<Channel> AddAsync(Channel toUrlReferrer);
        Task<bool> UpdateAsync(Channel channel);
    }
}