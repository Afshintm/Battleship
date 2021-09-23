using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SampleTests.Components.Tests.Helpers;
using Xunit;
using SampleTests.Components.Tests.ApiObjects;
using SampleTests.Components.Tests.Extensions;

namespace SampleTests.Components.Tests.HttpClients
{
    public class ApiHttpClientTests
    {
        private readonly ApiHttpClient _apiClient;

        public ApiHttpClientTests()
        {
            _apiClient = new ApiHttpClient();

        }

        [Fact]
        public async Task Http_Client_With_Json_Body_Headers_Should_Return_Version()
        {
            // Arrange
            var url = $"{Constants.BaseUrl}/api/version";
            var method = HttpMethod.Get;
            var tenantId = Guid.NewGuid().ToString();
            var clientId = Guid.NewGuid().ToString();
            var body = "{}";
            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/vnd.api+json");
            headers.Add("x-Abcdapi-tenantid", tenantId);
            headers.Add("x-Abcdapi-clientid", clientId);

            // Act
            var response = await _apiClient.SendAsync(url, method, body, JsonConvert.SerializeObject(headers));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);
        }



    }
}
