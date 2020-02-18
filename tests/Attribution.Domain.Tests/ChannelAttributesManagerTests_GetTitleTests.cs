using System.Threading.Tasks;
using Attribution.Domain.Dal;
using Attribution.Domain.Managers;
using Attribution.Domain.Models;
using Attribution.TestUtils;
using Moq;
using Xunit;

namespace Attribution.Domain.Tests
{
    [Trait("Category", "Unit")]
    public class ChannelAttributesManagerTests_GetTitleTests
    {
        private readonly ChannelAttributesManager _manager;
        private readonly Mock<IChannelAttributesRepository> _channelAttributesRepositoryMock = new Mock<IChannelAttributesRepository>();
        private readonly Mock<IUrlReferrerRepository> _urlReferrerRepositoryMock = new Mock<IUrlReferrerRepository>();
        private readonly Mock<IChannelRepository> _channelRepositoryMock = new Mock<IChannelRepository>();

        private const long AttributionDataHash = 17L;
        private readonly UrlReferrer _urlReferrer;
        
        public ChannelAttributesManagerTests_GetTitleTests()
        {
            _urlReferrer = new UrlReferrer
            {
                Id = 4,
                Host = "SomeHost.some"
            };

            _urlReferrerRepositoryMock
                .Setup(m => m.GetByIdAsync(_urlReferrer.Id))
                .ReturnsAsync(_urlReferrer);
            
            _manager = new ChannelAttributesManager(
                _channelAttributesRepositoryMock.Object,
                _urlReferrerRepositoryMock.Object,
                _channelRepositoryMock.Object);
        }

        [Fact]
        public async Task GetTitle_NoSuchHash_Null()
        {
            SetupChannelAttributesRepository(null);
            
            var title = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Null(title);
        }

        [Fact]
        public async Task GetTitle_HashExists_TitleIsNotEmptyString()
        {
            SetupChannelAttributesRepository(new ChannelAttributesBuilder().WithUtmSource("SomeSource").Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.False(string.IsNullOrWhiteSpace(titleInfo.Title), "title must not be an empty string");
        }

        [Fact]
        public async Task GetTitle_WithUtmSource_UtmSource()
        {
            SetupChannelAttributesRepository(new ChannelAttributesBuilder().WithUtmSource("SomeSource").Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Equal("SomeSource", titleInfo.Title);
        }
        
        [Fact]
        public async Task GetTitle_WithUtmMedium_UtmMedium()
        {
            SetupChannelAttributesRepository(new ChannelAttributesBuilder().WithUtmMedium("SomeMedium").Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Equal("SomeMedium", titleInfo.Title);
        }
        
        [Fact]
        public async Task GetTitle_WithUtmSourceAndUtmMedium_UtmSourceAndUtmMedium()
        {
            SetupChannelAttributesRepository(
                new ChannelAttributesBuilder()
                    .WithUtmSource("SomeSource")
                    .WithUtmMedium("SomeMedium")
                    .Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Equal("SomeSource_SomeMedium", titleInfo.Title);
        }
        
        [Fact]
        public async Task GetTitle_WithUtmSourceAndUtmMediumAndUtmCampaign_UtmSourceAndUtmMediumAndUtmCampaign()
        {
            SetupChannelAttributesRepository(
                new ChannelAttributesBuilder()
                    .WithUtmSource("SomeSource")
                    .WithUtmMedium("SomeMedium")
                    .WithUtmCampaign("SomeCampaign")
                    .Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Equal("SomeSource_SomeMedium_SomeCampaign", titleInfo.Title);
        }
        
        [Fact]
        public async Task GetTitle_WithUrlReferrer_UrlReferrerHost()
        {
            SetupChannelAttributesRepository(
                new ChannelAttributesBuilder()
                    .WithUrlReferrer(_urlReferrer.Id)
                    .Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Equal(_urlReferrer.Host, titleInfo.Title);
        }
        
        [Fact]
        public async Task GetTitle_WithoutAnyUtmsAndUrlReferrer_Untitled()
        {
            SetupChannelAttributesRepository(new ChannelAttributesBuilder().Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Equal("untitled", titleInfo.Title);
        }
        
        [Fact]
        public async Task GetTitle_WithUtmTerm_Untitled()
        {
            SetupChannelAttributesRepository(new ChannelAttributesBuilder().WithUtmTerm("SomeTerm").Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Equal("untitled", titleInfo.Title);
        }
        
        [Fact]
        public async Task GetTitle_WithChannelName_ReturnsChannelName()
        {
            var channel = new ChannelBuilder().WithName("SomeChannel").Build();
            SetupChannelRepository(channel);
            SetupChannelAttributesRepository(
                new ChannelAttributesBuilder()
                    .WithUtmTerm("SomeTerm")
                    .WithChannelId(channel.Id)
                    .Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Equal("SomeChannel", titleInfo.ChannelName);
        }
        
        [Fact]
        public async Task GetTitle_ChannelAttributesWithoutChannel_ChannelNameIsNull()
        {
            SetupChannelAttributesRepository(
                new ChannelAttributesBuilder()
                    .WithUtmTerm("SomeTerm")
                    .Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Null(titleInfo.ChannelName);
        }
        
        [Fact]
        public async Task GetTitle_ChannelWithoutName_ChannelNameIsNull()
        {
            var channel = new ChannelBuilder().Build();
            SetupChannelRepository(channel);
            SetupChannelAttributesRepository(
                new ChannelAttributesBuilder()
                    .WithUtmTerm("SomeTerm")
                    .WithChannelId(channel.Id)
                    .Build());
            
            var titleInfo = await _manager.GetTitleAsync(AttributionDataHash);

            Assert.Null(titleInfo.ChannelName);
        }

        private void SetupChannelAttributesRepository(ChannelAttributes channelAttributes)
        {
            _channelAttributesRepositoryMock
                .Setup(m => m.GetByHashAsync(It.IsAny<long>()))
                .ReturnsAsync(channelAttributes);
        }
        
        private void SetupChannelRepository(Channel channel)
        {
            _channelRepositoryMock
                .Setup(m => m.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(channel);
        }
    }
}