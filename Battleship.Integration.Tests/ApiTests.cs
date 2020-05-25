using System.Net.Http;
using System.Text;
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
        public const string ApplicationJson_ContentType = "application/json; charset=utf-8";
        public const string Create_Board_Url = "/board/create";

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
            Assert.Equal(ApplicationJson_ContentType, response.Content.Headers.ContentType.ToString());
            var apiResult = await response.ParseResponse();
            if (url == Get_Board_Url)
                Assert.Equal(GameStatus.NotStarted.ToString() , apiResult.Response.status);
        }

        [Fact]
        public async Task Create_Board_Is_Successful()
        {
            
            var response = await CreateBoard();
            // Assert
            response.EnsureSuccessStatusCode(); 
            Assert.Equal(ApplicationJson_ContentType, response.Content.Headers.ContentType.ToString());
            
            var apiResult = await response.ParseResponse();
            
            Assert.Equal(GameStatus.Setup.ToString() , apiResult.Response.status);
        }

        public async Task<HttpResponseMessage> CreateBoard()
        {
            var client = _factory.CreateClient();
            var stringContent = new StringContent("" ,Encoding.UTF8,"application/json");
            var response = await client.PostAsync(Create_Board_Url, stringContent);
            return response;
        }
    }
}