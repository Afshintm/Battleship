using BattleShip.Api.Models;
using Xunit;

namespace BattleShip.Unit.Tests
{
    public class ShipTests
    {
        [Fact]
        public void Create_A_Ship_Should_Assign_Positions()
        {
            
            var ship = new Ship();
            Assert.NotNull(ship.BoardPositions);
        }
    }
}