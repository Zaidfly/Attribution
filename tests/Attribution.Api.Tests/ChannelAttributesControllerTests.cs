using System.Net.Http;
using System.Threading.Tasks;
using Attribution.Api.Dtos;
using Attribution.Api.Tests.Migrations;
using Xunit;

namespace Attribution.Api.Tests
{
    [Trait("Category", "Integration")]
    public class ChannelAttributesControllerTests: IClassFixture<AttributionApiTestsFixture>
    {
        private readonly HttpClient _client;
        private const string BaseAddress = "/api/v1/channelattributes/title";
        
        public ChannelAttributesControllerTests(AttributionApiTestsFixture attributionApiTestsFixture)
        {
            _client = attributionApiTestsFixture.TestServer.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Source", "IntergationTests");
        }
        
        [Fact]
        public async Task GetChannelAttributesTitle_FromUtmParameters()
        {
            const long hash = Migration005_ChannelAttributes_Add.UtmParametersHash;
            const string expectedTitle = Migration005_ChannelAttributes_Add.UtmSource
                                         + "_" + Migration005_ChannelAttributes_Add.UtmMedium
                                         + "_" + Migration005_ChannelAttributes_Add.UtmCampaign;
            const string expectedChannelName = Migration004_Channels_Add.Name;
            
            var response = await _client.GetAsync($"{BaseAddress}?hash={hash}");

            response.EnsureSuccessStatusCode();
            var titleInfo = response.ReadResponseContent<ChannelAttributesTitleDto>();
            Assert.Equal(expectedTitle, titleInfo.Title);
            Assert.Equal(expectedChannelName, titleInfo.ChannelName);
        }
        
        [Fact]
        public async Task GetChannelAttributesTitle_FromUrlReferrer()
        {
            const long hash = Migration005_ChannelAttributes_Add.UrlReferrerHash;
            const string expectedTitle = Migration001_UrlReferrers_Add.SomeTestHost;
            
            var response = await _client.GetAsync($"{BaseAddress}?hash={hash}");

            response.EnsureSuccessStatusCode();
            var titleInfo = response.ReadResponseContent<ChannelAttributesTitleDto>();
            Assert.Equal(expectedTitle, titleInfo.Title);
        }
        
        [Fact]
        public async Task GetChannelAttributesTitle_NotExistentHash()
        {
            const long hash = -1L;
            
            var response = await _client.GetAsync($"{BaseAddress}?hash={hash}");

            response.EnsureSuccessStatusCode();
            var titleInfo = response.ReadResponseContent<ChannelAttributesTitleDto>();
            Assert.Null(titleInfo);
        }
    }
}