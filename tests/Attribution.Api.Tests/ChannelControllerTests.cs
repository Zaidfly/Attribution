using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Attribution.Api.Dtos;
using Attribution.Api.Tests.Migrations;
using Xunit;

namespace Attribution.Api.Tests
{
    [Trait("Category", "Integration")]
    public class ChannelControllerTests: IClassFixture<AttributionApiTestsFixture>
    {
        private const string BaseAddress = "/api/v1/channels";
        private readonly HttpClient _client;
        
        public ChannelControllerTests(AttributionApiTestsFixture attributionApiTestsFixture)
        {
            _client = attributionApiTestsFixture.TestServer.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Source", "IntergationTests");
        }
        
        [Fact]
        public async Task GetAll_ReturnsExistingChannels()
        {
            var response = await _client.GetAsync(BaseAddress);

            response.EnsureSuccessStatusCode();
            var channelDtos = response.ReadResponseContent<IReadOnlyCollection<ChannelDto>>();
            Assert.True(channelDtos.Count > 1);
        }
        
        [Fact]
        public async Task Get_ReturnsExistingChannels()
        {
            var response = await _client.GetAsync($"{BaseAddress}/{Migration004_Channels_Add.Id}");

            response.EnsureSuccessStatusCode();
            var channelDto = response.ReadResponseContent<ChannelWithIdDto>();
            Assert.NotNull(channelDto);
            Assert.Equal(Migration004_Channels_Add.Id, channelDto.Id);
            Assert.Equal(Migration004_Channels_Add.Name, channelDto.Name);
            Assert.Equal(Migration004_Channels_Add.Owner, channelDto.Owner);
            Assert.Equal(Migration004_Channels_Add.Contract, channelDto.Contract);
            Assert.Equal(Migration004_Channels_Add.PriceTerms, channelDto.PriceTerms);
            Assert.Equal(Migration004_Channels_Add.PartnerName, channelDto.PartnerName);
            Assert.Equal(Migration004_Channels_Add.ContactsEmail, channelDto.ContactsEmail);
            Assert.Equal(Migration004_Channels_Add.ContactsPhone, channelDto.ContactsPhone);
            Assert.Equal(Migration004_Channels_Add.ContactsWebsite, channelDto.ContactsWebsite);
            Assert.Equal(Migration004_Channels_Add.Description, channelDto.Description);
            Assert.Equal(Migration004_Channels_Add.Comments, channelDto.Comments);
        }
        
        [Fact]
        public async Task Add_AddedNewChannel()
        {
            var channelWithIdDto = new ChannelWithIdDto
            {
                Name = "add-name",
                Owner = "add-owner",
                Contract = "add-contract",
                PriceTerms = "add-PriceTerms",
                PartnerName = "add-PartnerName",
                ContactsEmail = "add-ContactsEmail",
                ContactsPhone = "add-ContactsPhone",
                ContactsWebsite = "add-ContactsWebsite",
                Description = "add-Description",
                Comments = "add-comments",
            };
            
            var response = await _client.PostAsync(BaseAddress, channelWithIdDto.ToJsonContent());

            response.EnsureSuccessStatusCode();
            var returnedChannelDto = response.ReadResponseContent<ChannelWithIdDto>();
            Assert.NotNull(returnedChannelDto);
            Assert.Equal(channelWithIdDto.Name, returnedChannelDto.Name);
            Assert.Equal(channelWithIdDto.Owner, returnedChannelDto.Owner);
            Assert.Equal(channelWithIdDto.Contract, returnedChannelDto.Contract);
            Assert.Equal(channelWithIdDto.PriceTerms, returnedChannelDto.PriceTerms);
            Assert.Equal(channelWithIdDto.PartnerName, returnedChannelDto.PartnerName);
            Assert.Equal(channelWithIdDto.ContactsEmail, returnedChannelDto.ContactsEmail);
            Assert.Equal(channelWithIdDto.ContactsPhone, returnedChannelDto.ContactsPhone);
            Assert.Equal(channelWithIdDto.ContactsWebsite, returnedChannelDto.ContactsWebsite);
            Assert.Equal(channelWithIdDto.Description, returnedChannelDto.Description);
            Assert.Equal(channelWithIdDto.Comments, returnedChannelDto.Comments);
            
            //Get newly created channel
            var getResponse = await _client.GetAsync($"{BaseAddress}/{returnedChannelDto.Id}");
            getResponse.EnsureSuccessStatusCode();
            
            var createdChannelDto = getResponse.ReadResponseContent<ChannelWithIdDto>();
            Assert.NotNull(returnedChannelDto);
            Assert.Equal(returnedChannelDto.Id, createdChannelDto.Id);
            Assert.Equal(channelWithIdDto.Name, createdChannelDto.Name);
            Assert.Equal(channelWithIdDto.Owner, createdChannelDto.Owner);
            Assert.Equal(channelWithIdDto.Contract, createdChannelDto.Contract);
            Assert.Equal(channelWithIdDto.PriceTerms, createdChannelDto.PriceTerms);
            Assert.Equal(channelWithIdDto.PartnerName, createdChannelDto.PartnerName);
            Assert.Equal(channelWithIdDto.ContactsEmail, createdChannelDto.ContactsEmail);
            Assert.Equal(channelWithIdDto.ContactsPhone, createdChannelDto.ContactsPhone);
            Assert.Equal(channelWithIdDto.ContactsWebsite, createdChannelDto.ContactsWebsite);
            Assert.Equal(channelWithIdDto.Description, createdChannelDto.Description);
            Assert.Equal(channelWithIdDto.Comments, createdChannelDto.Comments);
        }
        
        [Fact]
        public async Task Update_ReturnsExistingChannels()
        {
            var channelDto = new ChannelWithIdDto
            {
                Id = 1,
                Name = "update-direct",
                Owner = "update-owner"
            };
            
            var response = await _client.PutAsync(BaseAddress, channelDto.ToJsonContent());
            response.EnsureSuccessStatusCode();
            
            //Get updated channel
            var getResponse = await _client.GetAsync($"{BaseAddress}/{channelDto.Id}");
            getResponse.EnsureSuccessStatusCode();
            
            var updatedChannelDto = getResponse.ReadResponseContent<ChannelWithIdDto>();
            Assert.NotNull(updatedChannelDto);
            Assert.Equal(channelDto.Id, updatedChannelDto.Id);
            Assert.Equal(channelDto.Name, updatedChannelDto.Name);
        }
    }
}