using System;
using BattleShip.Api.ExceptionHandling;
using BattleShip.Api.Models;
using BattleShip.Api.Services;
using Xunit;

namespace BattleShip.Unit.Tests
{
    public class GameTrackerServiceTests
    {
        [Fact]
        public void Cannot_Setup_Board_When_Its_in_Playing_Status()
        {
            var gameTracker = new GameTrackerService();
            gameTracker.Status = GameStatus.Playing;
            Action act = ()=> gameTracker.CreateBoard();
            Assert.Throws<HttpStatusCodeException>(act);
        }
    }
}