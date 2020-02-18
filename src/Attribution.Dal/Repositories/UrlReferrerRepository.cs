using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Attribution.Domain.Dal;
using Attribution.Domain.Models;
using LinqToDB;
using YouDo.Caching;

namespace Attribution.Dal.Repositories
{
    public class UrlReferrerRepository : IUrlReferrerRepository
    {
        public const string WhiteListCacheKey = "attribution.urlReferrer.whitelist";
        
        private readonly AttributionDb _dbContext;
        private readonly IDistributedCache _distributedCache;

        public UrlReferrerRepository(AttributionDb dbContext, IDistributedCache distributedCache)
        {
            _dbContext = dbContext;
            _distributedCache = distributedCache;
        }

        public async Task<IReadOnlyCollection<UrlReferrer>> GetAllAsync(bool isDeleted = false)
        {
            if (isDeleted)
            {
                return await GetFilteredAsync(r => r.IsDeleted == isDeleted);
            }
            
            return await _distributedCache.AddOrGetAsync(
                WhiteListCacheKey,
                TimeSpan.FromDays(7),
                async token => await GetFilteredAsync(r => r.IsDeleted == isDeleted),
                CancellationToken.None);
        }

        public async Task<UrlReferrer> GetByHostAsync(string host)
        {
            return (await GetAllAsync()).FirstOrDefault(ur => ur.Host == host);
        }

        public async Task<UrlReferrer> GetByIdAsync(int id)
        {
            return (await GetAllAsync()).FirstOrDefault(ur => ur.Id == id);
        }

        public async Task AddOrUpdateAsync(UrlReferrer urlReferrer)
        {
            await _dbContext.UrlReferrers.InsertOrUpdateAsync(
                () => new UrlReferrerDb
                {
                    ChannelId = urlReferrer.ChannelId,
                    Host = urlReferrer.Host,
                    IsDeleted = false
                },
                dbValue => new UrlReferrerDb
                {
                    IsDeleted = false,
                    ChannelId = urlReferrer.ChannelId ?? dbValue.ChannelId
                },
                () => new UrlReferrerDb
                {
                    Host = urlReferrer.Host
                }
            );
            
            _distributedCache.Delete(WhiteListCacheKey);
        }

        public async Task RemoveByHostAsync(string host)
        {
            var updated = await _dbContext.UrlReferrers
                .Where(r => r.Host == host)
                .Set(r => r.IsDeleted, true)
                .UpdateAsync();

            if (updated == 0)
            {
                throw new KeyNotFoundException();
            }
            
            _distributedCache.Delete(WhiteListCacheKey);
        }

        private async Task<IReadOnlyCollection<UrlReferrer>> GetFilteredAsync(Expression<Func<UrlReferrerDb, bool>> filter)
        {
            return await _dbContext.UrlReferrers
                .Where(filter)
                .Select(dbEntity => new UrlReferrer
                {
                    Id = dbEntity.Id,
                    ChannelId = dbEntity.ChannelId,
                    Host = dbEntity.Host
                }).ToListAsync();
        }
    }
}