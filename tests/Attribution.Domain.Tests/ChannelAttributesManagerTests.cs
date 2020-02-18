using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attribution.Domain.Dal;
using Attribution.Domain.Managers;
using Attribution.Domain.Models;
using Moq;
using Xunit;

namespace Attribution.Domain.Tests
{
    [Trait("Category", "Unit")]
    public class ChannelAttributesManagerTests
    {
        private const long DirectChannelAttributesId = 42L;
        private const int DirectChannelId = 24;
        
        private const long ExpectedChannelAttributesId = 123L;
        private const int ExpectedUrlReferrerId = 55;
        
        private readonly ChannelAttributesManager _manager;
        private readonly Mock<IChannelAttributesRepository> _channelAttributesRepositoryMock = new Mock<IChannelAttributesRepository>();
        private readonly Mock<IUrlReferrerRepository> _urlReferrerRepositoryMock = new Mock<IUrlReferrerRepository>();
        private readonly Mock<IChannelRepository> _channelRepositoryMock = new Mock<IChannelRepository>();
        
        private ChannelAttributes _savedAttributes;
        
        public ChannelAttributesManagerTests()
        {
            _channelAttributesRepositoryMock
                .Setup(m => m.GetOrAddChannelAttributesIdAsync(It.IsAny<long>()))
                .ReturnsAsync(ExpectedChannelAttributesId);
            
            _channelAttributesRepositoryMock
                .Setup(m => m.GetDirectChannelAttributesIdAsync())
                .ReturnsAsync(DirectChannelAttributesId);

            _channelAttributesRepositoryMock
                .Setup(m => m.SaveChannelAttributesAsync(It.IsAny<ChannelAttributes>()))
                .Callback<ChannelAttributes>(ca => _savedAttributes = ca);

            _channelRepositoryMock
                .Setup(m => m.GetDirectChannelId())
                .Returns(DirectChannelId);

            _manager = new ChannelAttributesManager(
                _channelAttributesRepositoryMock.Object, 
                _urlReferrerRepositoryMock.Object,
                _channelRepositoryMock.Object);
        }

        [Fact]
        public async Task GetChannelAttributesId_HashIsNull_ReturnsDirect()
        {
            var channelAttributesId = await _manager.GetChannelAttributesIdAsync(null);
            
            Assert.Equal(DirectChannelAttributesId, channelAttributesId);
        }

        [Fact]
        public async Task GetChannelAttributesId_HashIsZero_ReturnsDirect()
        {
            var channelAttributesId = await _manager.GetChannelAttributesIdAsync(0L);
            
            Assert.Equal(DirectChannelAttributesId, channelAttributesId);
        }
        
        [Fact]
        public async Task GetChannelAttributesId_HashIsSomeNumber_ReturnsIdForHash()
        {
            var channelAttributesId = await _manager.GetChannelAttributesIdAsync(123L);
            
            Assert.Equal(ExpectedChannelAttributesId, channelAttributesId);
        }

        [Fact]
        public async Task CreateChannelAttributesData_HashIsZero_ArgumentException()
        {
            Task Action() => _manager.CreateChannelAttributesDataAsync(0L, null);

            await Assert.ThrowsAsync<ArgumentException>(Action);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_AttributionDataIsNull_ArgumentNullException()
        {
            Task Action() => _manager.CreateChannelAttributesDataAsync(1L, null);

            await Assert.ThrowsAsync<ArgumentException>(Action);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public async Task CreateChannelAttributesData_AttributionDataIsEmptyString_ArgumentException(string attributionData)
        {
            Task Action() => _manager.CreateChannelAttributesDataAsync(1L, attributionData);

            await Assert.ThrowsAsync<ArgumentException>(Action);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_ValidAttributionData_SavesChannelAttributes()
        {
            const long hash = 1L;
            const string attributionData = @"{""utm_source"":""abc""}";
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            _channelAttributesRepositoryMock.Verify(
                m => m.SaveChannelAttributesAsync(It.IsAny<ChannelAttributes>()),
                Times.Once);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_ValidAttributionData_SavesWithHash()
        {
            const long hash = 1L;
            const string attributionData = @"{""utm_source"":""abc""}";
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal(hash, _savedAttributes.Hash);
        }

        [Theory]
        [MemberData(nameof(UtmParamsData))]
        public async Task CreateChannelAttributesData_UtmParams_Save(
            string utmParamName, 
            string utmParamValue, 
            Func<ChannelAttributes, string> propertyGetter)
        {
            const long hash = 1L;
            var attributionData = $@"{{""{utmParamName}"":""{utmParamValue}""}}";
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal(utmParamValue, propertyGetter(_savedAttributes));
        }

        [Fact]
        public async Task CreateChannelAttributesData_SeveralParameters_Save()
        {
            const long hash = 1L;
            const string attributionData = @"{""utm_medium"":""abc"",""utm_source"":""xyz""}";
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal("abc", _savedAttributes.UtmMedium);
            Assert.Equal("xyz", _savedAttributes.UtmSource);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_SomeUnkownUtmParameters_SavesInUnparsedData()
        {
            const long hash = 1L;
            const string attributionData = @"{""utm_unknown_param"":""fff"",""utm_source"":""xyz""}";
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal(@"{""utm_unknown_param"":""fff""}", _savedAttributes.UnparsedData);
        }
            
        [Fact]
        public async Task CreateChannelAttributesData_NoneUnkownUtmParameters_UnparsedDataIsNull()
        {
            const long hash = 1L;
            const string attributionData = @"{""utm_medium"":""fff"",""utm_source"":""xyz""}";
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Null(_savedAttributes.UnparsedData);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_UrlReferrerIsInWhitelist_SavesUrlReferrer()
        {
            const long hash = 1L;
            const string attributionData = @"{""urlReferrer"":""google.com""}";

            SetupUrlReffererWhitelist(ExpectedUrlReferrerId);
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal(ExpectedUrlReferrerId, _savedAttributes.UrlReferrerId);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_UtmParametersAndUrlReferrer_DoesNotCheckUrlReferrer()
        {
            const long hash = 1L;
            const string attributionData = @"{""utm_unknown_param"":""fff"", ""urlReferrer"":""google.com""}";

            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            _urlReferrerRepositoryMock.Verify(m => m.GetByHostAsync(It.IsAny<string>()), Times.Never);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_UrlReferrerHasChannelId_SavesUrlReferrerAndChannelId()
        {
            const long hash = 1L;
            const string attributionData = @"{""urlReferrer"":""google.com""}";

            const int bindedChannelId = 234;
            SetupUrlReffererWhitelist(ExpectedUrlReferrerId, bindedChannelId);
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal(ExpectedUrlReferrerId, _savedAttributes.UrlReferrerId);
            Assert.Equal(bindedChannelId, _savedAttributes.ChannelId);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_UrlReferrerDoesNotHaveChannelId_SavesUrlReferrerWithoutChannel()
        {
            const long hash = 1L;
            const string attributionData = @"{""urlReferrer"":""google.com""}";

            SetupUrlReffererWhitelist(ExpectedUrlReferrerId, null);
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal(ExpectedUrlReferrerId, _savedAttributes.UrlReferrerId);
            Assert.Null(_savedAttributes.ChannelId);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_UrlReferrerIsNotInWhitelist_UrlReferrerIsNull()
        {
            const long hash = 1L;
            const string attributionData = @"{""urlReferrer"":""google.com""}";

            SetupUrlReffererWhitelist(null);
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Null(_savedAttributes.UrlReferrerId);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_UrlReferrerIsNotInWhitelist_ChannelIdIsDirect()
        {
            const long hash = 1L;
            const string attributionData = @"{""urlReferrer"":""google.com""}";

            SetupUrlReffererWhitelist(null);
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal((int?)DirectChannelId, _savedAttributes.ChannelId.Value);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_UrlReferrerIsNotInWhitelist_DoesNotCheck()
        {
            const long hash = 1L;
            const string attributionData = @"{""urlReferrer"":""google.com""}";

            SetupUrlReffererWhitelist(null);
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal((int?)DirectChannelId, _savedAttributes.ChannelId.Value);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_UtmChannelIdExists_SavesChannelId()
        {
            const int channelId = 153;
            const long hash = 1L;
            var attributionData = $@"{{""utm_channel_id"":""{channelId}""}}";
            
            SetupChannelIdExists(true);
            
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Equal(channelId, _savedAttributes.ChannelId);
        }
        
        [Fact]
        public async Task CreateChannelAttributesData_UtmChannelIdDoesNotExist_ChannelIdIsNull()
        {
            const int channelId = 144;
            const long hash = 1L;
            var attributionData = $@"{{""utm_channel_id"":""{channelId}""}}";

            SetupChannelIdExists(false);
                
            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            Assert.Null(_savedAttributes.UrlReferrerId);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateChannelAttributesData_UtmChannelIdDoesNotExist_DoesNotCallChannelRepository(string channelId)
        {
            const long hash = 1L;
            var attributionData = $@"{{""utm_channel_id"":""{channelId}""}}";

            await _manager.CreateChannelAttributesDataAsync(hash, attributionData);

            _channelRepositoryMock.Verify(m => m.IsChannelIdExistsAsync(It.IsAny<int>()), Times.Never);
            Assert.Null(_savedAttributes.UrlReferrerId);
        }

        public static IEnumerable<object[]> UtmParamsData => 
         new List<object[]>
            {
                new object[] { "utm_source", "abc1", (Func<ChannelAttributes, string>)(ca => ca.UtmSource) },
                new object[] { "utm_medium", "abc2", (Func<ChannelAttributes, string>)(ca => ca.UtmMedium) },
                new object[] { "utm_campaign", "abc3", (Func<ChannelAttributes, string>)(ca => ca.UtmCampaign) },
                new object[] { "utm_term", "abc4", (Func<ChannelAttributes, string>)(ca => ca.UtmTerm) },
                new object[] { "utm_content", "abc5", (Func<ChannelAttributes, string>)(ca => ca.UtmContent) },
                new object[] { "utm_agency", "xyz1", (Func<ChannelAttributes, string>)(ca => ca.UtmAgency) },
                new object[] { "utm_partner_id", "xyz2", (Func<ChannelAttributes, string>)(ca => ca.UtmPartnerId) },
                new object[] { "utm_campaign_id", "xyz3", (Func<ChannelAttributes, string>)(ca => ca.UtmCampaignId) },
                new object[] { "utm_ad_type", "xyz4", (Func<ChannelAttributes, string>)(ca => ca.UtmAdType) },
                
            };

        private void SetupChannelIdExists(bool exists)
        {
            _channelRepositoryMock
                .Setup(m => m.IsChannelIdExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(exists);
        }
        
        private void SetupUrlReffererWhitelist(int? existingId, int? bindedChannelId = null)
        {
            var urlReferrer = existingId.HasValue
                ? new UrlReferrer {Id = existingId.Value, ChannelId = bindedChannelId}
                : null;
            
            _urlReferrerRepositoryMock
                .Setup(m => m.GetByHostAsync(It.IsAny<string>()))
                .ReturnsAsync(urlReferrer);
        }
    }
}