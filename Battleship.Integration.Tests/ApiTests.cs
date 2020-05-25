using System.Threading.Tasks;
using BattleShip.Api;
using BattleShip.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Battleship.Integration.Tests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        public const string Get_Board_Url = "/board";
        public const string Get_Game_Url = "/game";

        public ApiTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }
        
        [Theory]
        [InlineData(Get_Game_Url)]
        [InlineData(Get_Board_Url)]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); 
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var apiResult = await response.ParseResponse();
            if (url == Get_Board_Url)
                Assert.Equal(GameStatus.NotStarted.ToString() , apiResult.Response.status);
        }

    }
}