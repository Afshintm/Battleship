using System;
using System.Collections.Generic;
using BattleShip.Api.Models;
using Xunit;

namespace BattleShip.Unit.Tests
{
    public class ShipTests
    {
        [Fact]
        public void Create_A_Ship_Should_Assign_Positions()
        {
            
            var shipVewModel = new ShipViewModel
            {
                StartingPosition = new BoardPosition { X=1 , Y=1 },
                Length = 5,
                Alignment = Alignment.Horizontal
            };
            
            var ship = Ship.Create(shipVewModel);
            
            Assert.NotNull(ship);
            var boardPositions = ship.BoardPositions;
            
            Assert.Equal(shipVewModel.Length,ship.BoardPositions.Count);
            
            for (var i = 0; i < shipVewModel.Length; i++)
            {
                Assert.Contains(boardPositions, (n) =>
                    (shipVewModel.StartingPosition.Y == n.Y && shipVewModel.StartingPosition.X+i == n.X) 
                );
            }
        }

        [Fact]
        public void ValidateShipPositions_Should_Not_Allow_Crossing_Ships()
        {
            var source = new List<BoardPosition>
            {
                new BoardPosition{X=3,Y=3,Value = Guid.Empty},
                new BoardPosition{X=3,Y=4,Value = Guid.Empty},
                new BoardPosition{X=3,Y=5,Value = Guid.Empty},
            };
            var boardPositions = new List<BoardPosition>
            {
                new BoardPosition{X=3,Y=3,Value = Guid.Empty},
                new BoardPosition{X=4,Y=4,Value = Guid.Empty},
                new BoardPosition{X=5,Y=5,Value = Guid.Empty},
            };
        }

        [Fact]
        public void ValidateShipPositions_Should_Not_Allow_Crossing_Board_Border()
        {
        }
    }
}