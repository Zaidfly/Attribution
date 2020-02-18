using System.Linq;
using System.Threading.Tasks;
using Attribution.Domain.Dal;
using Attribution.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinqToDB;
using Npgsql;

namespace Attribution.Dal.Repositories
{
    public class ChannelAttributesRepository : IChannelAttributesRepository
    {
        private const string KeyIsAlreadyExistsCode = "23505";
        private const long DirectChannelAttributesHashId = 0L;
        
        private readonly AttributionDb _dbContext;
        private readonly IMapper _mapper;
        
        public ChannelAttributesRepository(AttributionDb dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ChannelAttributes> GetByHashAsync(long hash)
        {
            return await _dbContext.ChannelAttributes
                .Where(ca => ca.Hash == hash)
                .ProjectTo<ChannelAttributes>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<long> GetOrAddChannelAttributesIdAsync(long hash)
        {
            var channelAttributesId = await GetChannelAttributesId(hash);
            if (channelAttributesId.HasValue) return channelAttributesId.Value;
            
            var channelAttributesData = new ChannelAttributesDb {Hash = hash};
            
            try
            {
                return await _dbContext.InsertWithInt64IdentityAsync(channelAttributesData);
            }
            catch (PostgresException e)
            {
                if (e.SqlState != KeyIsAlreadyExistsCode) throw;
            }
            
            var alreadyAddedAttributesId = await GetChannelAttributesId(hash);
            return alreadyAddedAttributesId.Value;
        }

        public async Task SaveChannelAttributesAsync(ChannelAttributes channelAttributes)
        {
            await _dbContext.ChannelAttributes.InsertOrUpdateAsync(
                () => new ChannelAttributesDb
                {
                    Hash = channelAttributes.Hash,
                    UtmSource = channelAttributes.UtmSource,
                    UtmMedium = channelAttributes.UtmMedium,
                    UtmCampaign = channelAttributes.UtmCampaign,
                    UtmTerm = channelAttributes.UtmTerm,
                    UtmContent = channelAttributes.UtmContent,
                    UtmAgency = channelAttributes.UtmAgency,
                    UtmAdType = channelAttributes.UtmAdType,
                    UtmCampaignId = channelAttributes.UtmCampaignId,
                    UtmPartnerId = channelAttributes.UtmPartnerId,

                    ChannelId = channelAttributes.ChannelId,
                    UrlReferrerId = channelAttributes.UrlReferrerId,
                    UnparsedData = channelAttributes.UnparsedData
                },
                _ => new ChannelAttributesDb
                {
                    UtmSource = channelAttributes.UtmSource,
                    UtmMedium = channelAttributes.UtmMedium,
                    UtmCampaign = channelAttributes.UtmCampaign,
                    UtmTerm = channelAttributes.UtmTerm,
                    UtmContent = channelAttributes.UtmContent,
                    UtmAgency = channelAttributes.UtmAgency,
                    UtmAdType = channelAttributes.UtmAdType,
                    UtmCampaignId = channelAttributes.UtmCampaignId,
                    UtmPartnerId = channelAttributes.UtmPartnerId,

                    ChannelId = channelAttributes.ChannelId,
                    UrlReferrerId = channelAttributes.UrlReferrerId
                },
                () => new ChannelAttributesDb
                {
                    Hash = channelAttributes.Hash
                });
        }

        public async Task<long> GetDirectChannelAttributesIdAsync()
        {
            return await _dbContext.ChannelAttributes
                .Where(ca => ca.Hash == DirectChannelAttributesHashId)
                .Select(ca => ca.Id)
                .FirstAsync();
        }

        private async Task<long?> GetChannelAttributesId(long hash)
        {
            return await _dbContext.ChannelAttributes
                .Where(ca => ca.Hash == hash)
                .Select(ca => (long?)ca.Id)
                .FirstOrDefaultAsync();
        }
    }
}