using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Attribution.Domain.Dal;
using Attribution.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinqToDB;
using YouDo.Caching;

namespace Attribution.Dal.Repositories
{
    public class ChannelRepository : IChannelRepository
    {
        private const int DirectChannelId = 1;
        private readonly AttributionDb _dbContext;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;

        public ChannelRepository(AttributionDb dbContext, IDistributedCache distributedCache, IMapper mapper)
        {
            _dbContext = dbContext;
            _distributedCache = distributedCache;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<Channel>> GetAllAsync()
        {
            return await _dbContext.Channels
                .ProjectTo<Channel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        
        public async Task<Channel> GetByIdAsync(int id)
        {
            return await _distributedCache.AddOrGetAsync(
                GetChannelIdKey(id),
                TimeSpan.FromDays(7),
                async token => await _dbContext.Channels
                    .Where(c => c.Id == id)
                    .ProjectTo<Channel>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(token),
                CancellationToken.None);
        }

        public async Task<Channel> AddAsync(Channel channel)
        {
            var channelDb = _mapper.Map<ChannelDb>(channel);

            var id = await _dbContext.InsertWithInt32IdentityAsync(channelDb);

            return await GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Channel channel)
        {
            if (channel.Id == 0)
            {
                throw new ArgumentException(nameof(channel.Id));
            }
            
            var channelDb = _mapper.Map<ChannelDb>(channel);

            var result = await _dbContext.UpdateAsync(channelDb);

            var updated = result == 1;
            if (updated)
            {
                _distributedCache.Delete(GetChannelIdKey(channel.Id));    
            }
            
            return updated;
        }

        public async Task<bool> IsChannelIdExistsAsync(int channelId)
        {
            return await _dbContext.Channels.AnyAsync(c => c.Id == channelId);
        }

        public int GetDirectChannelId() => DirectChannelId;

        private static string GetChannelIdKey(int id)
        {
            return $"attribution.channels.{id}";
        }
    }
}