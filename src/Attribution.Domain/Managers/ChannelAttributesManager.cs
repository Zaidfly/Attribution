using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Attribution.Domain.Collections;
using Attribution.Domain.Dal;
using Attribution.Domain.Models;

using Newtonsoft.Json;

namespace Attribution.Domain.Managers
{
    public class ChannelAttributesManager : IChannelAttributesManager
    {
        private readonly IChannelAttributesRepository _channelAttributesRepository;
        private readonly AttributionDataParser _attributionDataParser;
        private readonly IUrlReferrerRepository _urlReferrerRepository;
        private readonly IChannelRepository _channelRepository;
        
        public ChannelAttributesManager(
            IChannelAttributesRepository channelAttributesRepository, 
            IUrlReferrerRepository urlReferrerRepository,
            IChannelRepository channelRepository)
        {
            _channelAttributesRepository = channelAttributesRepository;
            _urlReferrerRepository = urlReferrerRepository;
            _channelRepository = channelRepository;
            _attributionDataParser = new AttributionDataParser(urlReferrerRepository, channelRepository);
        }

        public async Task<long> GetChannelAttributesIdAsync(long? hash)
        {
            if (!hash.HasValue || hash == 0)
            {
                return await _channelAttributesRepository.GetDirectChannelAttributesIdAsync();
            }

            return await _channelAttributesRepository.GetOrAddChannelAttributesIdAsync(hash.Value);
        }

        public async Task<ChannelAttributesTitle> GetTitleAsync(long hash)
        {
            var channelAttributes = await _channelAttributesRepository.GetByHashAsync(hash);
            if (channelAttributes == null)
            {
                return null;
            }

            var channelName = await GetChannelNameOrNullAsync(channelAttributes);
            var title = await GetAttributesTitleAsync(channelAttributes);
            
            return new ChannelAttributesTitle
            {
                Title = title,
                ChannelName = channelName
            };
        }

        private async Task<string> GetAttributesTitleAsync(ChannelAttributes channelAttributes)
        {
            var titleParts = new List<string>
                {
                    channelAttributes.UtmSource,
                    channelAttributes.UtmMedium,
                    channelAttributes.UtmCampaign,
                }
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            if (titleParts.Any())
            {
                return string.Join("_", titleParts);
            }

            if (channelAttributes.UrlReferrerId != null)
            {
                var urlReferrer = await _urlReferrerRepository.GetByIdAsync(channelAttributes.UrlReferrerId.Value);
                return urlReferrer.Host;
            }

            return "untitled";
        }

        public async Task CreateChannelAttributesDataAsync(long hash, string attributionData)
        {
            if (hash == 0L)
            {
                throw new ArgumentException("Hash cannot be zero", nameof(hash));
            }

            if (string.IsNullOrWhiteSpace(attributionData))
            {
                throw new ArgumentException("Argument cannot be null or empty", nameof(attributionData));
            }

            var channelAttributes = await _attributionDataParser.TryParseAsync(hash, attributionData);
            
            await _channelAttributesRepository.SaveChannelAttributesAsync(channelAttributes);
        }

        private async Task<string> GetChannelNameOrNullAsync(ChannelAttributes channelAttributes)
        {
            if (channelAttributes.ChannelId.HasValue)
            {
                var channel = await _channelRepository.GetByIdAsync(channelAttributes.ChannelId.Value);
                if (!string.IsNullOrWhiteSpace(channel.Name))
                {
                    return channel.Name;
                }
            }

            return null;
        }

        private class AttributionDataParser
        {
            private static JsonSerializerSettings Settings => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            
            private readonly IUrlReferrerRepository _urlReferrerRepository;
            private readonly IChannelRepository _channelRepository;
            
            public AttributionDataParser(
                IUrlReferrerRepository urlReferrerRepository, 
                IChannelRepository channelRepository)
            {
                _urlReferrerRepository = urlReferrerRepository;
                _channelRepository = channelRepository;
            }
            
            public async Task<ChannelAttributes> TryParseAsync(long hash, string attributionData)
            {
                var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                JsonConvert.PopulateObject(attributionData, attributes, Settings);
                
                var utmParameters = attributes
                    .Where(a => a.Key.StartsWith("utm_", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(x => x.Key, x => x.Value);
                
                if (utmParameters.Any())
                {
                    return await ParseUtmParameters(hash, utmParameters);                    
                }

                if (attributes.TryGetValue("urlReferrer", out var urlReferrerHost))
                {
                    return await ParseUrlReferrer(hash, urlReferrerHost);
                }

                throw new ArgumentException($"{nameof(attributionData)}has no utm_parameters and no UrlReferrer" +
                                            $"\r\n{attributionData}");
            }

            private async Task<ChannelAttributes> ParseUtmParameters(long hash, Dictionary<string, string> utmParameters)
            {
                var channelAttributes = new ChannelAttributes(hash)
                {
                    UtmSource = utmParameters.TryPopValue("utm_source"),
                    UtmMedium = utmParameters.TryPopValue("utm_medium"),
                    UtmCampaign = utmParameters.TryPopValue("utm_campaign"),
                    UtmTerm = utmParameters.TryPopValue("utm_term"),
                    UtmContent = utmParameters.TryPopValue("utm_content"),
                    UtmAgency = utmParameters.TryPopValue("utm_agency"),
                    UtmPartnerId = utmParameters.TryPopValue("utm_partner_id"),
                    UtmCampaignId = utmParameters.TryPopValue("utm_campaign_id"),
                    UtmAdType = utmParameters.TryPopValue("utm_ad_type"),
                    ChannelId = await ParseChannelId(utmParameters),
                    UnparsedData = utmParameters.Any() ? JsonConvert.SerializeObject(utmParameters, Formatting.None, Settings) : null
                };

                return channelAttributes;
            }

            private async Task<int?> ParseChannelId(Dictionary<string, string> attributes)
            {
                var utmChannelId = attributes.TryPopValue("utm_channel_id");
                if (!string.IsNullOrWhiteSpace(utmChannelId)
                    && int.TryParse(utmChannelId, NumberStyles.None, CultureInfo.InvariantCulture,out var parsedChannelId)
                    && await _channelRepository.IsChannelIdExistsAsync(parsedChannelId))
                {
                    return parsedChannelId;
                }

                return null;
            }

            private async Task<ChannelAttributes> ParseUrlReferrer(long hash, string urlReferrerHost)
            {
                var channelAttributes = new ChannelAttributes(hash)
                {
                    Hash = hash
                };

                var urlReferrer = await _urlReferrerRepository.GetByHostAsync(urlReferrerHost);
                channelAttributes.UrlReferrerId = urlReferrer?.Id;
                channelAttributes.ChannelId = urlReferrer != null 
                    ? urlReferrer.ChannelId 
                    : _channelRepository.GetDirectChannelId();

                return channelAttributes;
            }
        }
    }
}