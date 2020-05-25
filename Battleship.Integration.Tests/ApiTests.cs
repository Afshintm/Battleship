using System.Threading.Tasks;
using BattleShip.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Battleship.Integration.Tests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ApiTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }
        
        [Theory]
        [InlineData("/game")]
        [InlineData("/board")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());
        }

    }
}