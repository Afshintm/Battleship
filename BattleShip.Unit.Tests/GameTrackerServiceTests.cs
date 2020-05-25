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
            Action act = () => gameTracker.CreateBoard();
            Assert.Throws<HttpStatusCodeException>(act);
        }

        [Theory]
        [InlineData(GameStatus.NotStarted, GameStatus.Setup, null)]
        [InlineData(GameStatus.Setup, GameStatus.Playing, typeof(HttpStatusCodeException))]
        [InlineData(GameStatus.NotStarted, GameStatus.Playing, typeof(HttpStatusCodeException))]
        void CanMove_GameStatue_From_To(GameStatus from, GameStatus to, Type type = null)
        {
            var tracker = new GameTrackerService();
            tracker.SetStatus(from);
            Action act = () => tracker.SetStatus(to);
            if (type != null)
            {
                var exception = Assert.Throws<HttpStatusCodeException>(act);
            }
            else
            {
                act();
                Assert.Equal(to, tracker.Status);
            }
        }
    }
}