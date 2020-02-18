using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Attribution.Api.Dtos;
using Attribution.Api.Tests.Migrations;
using Newtonsoft.Json;
using Xunit;

namespace Attribution.Api.Tests
{
    [Trait("Category", "Integration")]
    public class UrlReferrerControllerTests : IClassFixture<AttributionApiTestsFixture>
    {
        private readonly HttpClient _client;
        
        public UrlReferrerControllerTests(AttributionApiTestsFixture attributionApiTestsFixture)
        {
            _client = attributionApiTestsFixture.TestServer.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Source", "IntergationTests");
        }

        [Fact]
        public async Task GetAll_ReturnsExistingUrlReferrers()
        {
            var response = await _client.GetAsync("/api/v1/urlreferrers");

            response.EnsureSuccessStatusCode();
            var urlReferrerDtos = response.ReadResponseContent<IReadOnlyCollection<UrlReferrerDto>>();
            Assert.True(urlReferrerDtos.Count > 1);
        }

        [Fact]
        public async Task GetAllDeleted_ReturnsDeletedUrlReferrers()
        {
            var response = await _client.GetAsync("/api/v1/urlreferrers/deleted");

            response.EnsureSuccessStatusCode();
            var urlReferrerDtos = response.ReadResponseContent<IReadOnlyCollection<UrlReferrerDto>>();
            var deletedReferrer = urlReferrerDtos
                .FirstOrDefault(r => r.Host == Migration002_UrlReferrers_AddDeleted.Sometestdomen);
            
            Assert.NotNull(deletedReferrer);
        }

        [Fact]
        public async Task Get_GetsUrlReferrer()
        {
            const string expectedHost = Migration001_UrlReferrers_Add.SomeTestHost;
            var response = await _client.GetAsync($"/api/v1/urlreferrers/{expectedHost}");

            response.EnsureSuccessStatusCode();
            
            var urlReferrerDto = response.ReadResponseContent<UrlReferrerDto>();
            
            Assert.NotNull(urlReferrerDto);
            Assert.Equal(expectedHost, urlReferrerDto.Host);
        }
        
        [Fact]
        public async Task Add_AddsNewUrlReferrer()
        {
            var newUrlReferer = new UrlReferrerDto
            {
                Host = "someaddedhost.test"
            };
            
            //Create new urlReferrer
            var postResponse = await _client.PostAsync($"/api/v1/urlreferrers/", newUrlReferer.ToJsonContent());
            postResponse.EnsureSuccessStatusCode();
            
            //Get newly created urlReferrer
            var getResponse = await _client.GetAsync($"/api/v1/urlreferrers/{newUrlReferer.Host}");
            getResponse.EnsureSuccessStatusCode();
            
            var createdUrlReferrer = getResponse.ReadResponseContent<UrlReferrerDto>();
            
            Assert.NotNull(createdUrlReferrer);
            Assert.Equal(newUrlReferer.Host, createdUrlReferrer.Host);
        }
        
        [Fact]
        public async Task Remove_DeletesUrlReferrer()
        {
            const string deletingHost = Migration003_UrlReferrers_AddForDeletion.Sometestdomen;
            
            //delete urlReferrer
            var postResponse = await _client.PostAsync($"/api/v1/urlreferrers/deleted/{deletingHost}", null);
            postResponse.EnsureSuccessStatusCode();
            
            //Try get deleted urlReferrer
            var getResponse = await _client.GetAsync($"/api/v1/urlreferrers/{deletingHost}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
    
    public static class Helper
    {
        public static StringContent ToJsonContent(this object @object) =>
            new StringContent(JsonConvert.SerializeObject(@object), Encoding.UTF8, "application/json");

        public static T ReadResponseContent<T>(this HttpResponseMessage response)
        {
            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}