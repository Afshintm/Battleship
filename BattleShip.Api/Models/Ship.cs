using System;
using System.Collections.Generic;

namespace BattleShip.Api.Models
{
    public class Ship
    {
        public Guid Id { get; private set; }
        public int Length { get; private set; }
        public  Alignment Alignment { get; private set; }
        
        public  BoardPosition StartingPosition { get; private set; }
        public List<BoardPosition> BoardPositions { get; private set; }

        public Ship()
        {
            Id = Guid.NewGuid();
            BoardPositions = new List<BoardPosition>();
            StartingPosition = new BoardPosition();
        }

        public Ship(BoardPosition position, int length, Alignment alignment)
        {
            Id = Guid.NewGuid();
            StartingPosition = position;
            Length = length;
            Alignment = alignment;
            BoardPositions = new List<BoardPosition>();
        }
    }
}