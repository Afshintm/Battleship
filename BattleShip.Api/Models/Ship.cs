using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BattleShip.Api.Models
{
    public class Ship
    {
        public Guid Id { get; private set; }
        public int Length { get; private set; }
        public  Alignment Alignment { get; private set; }
        
        public  BoardPosition StartingPosition { get; private set; }
        public List<BoardPosition> BoardPositions { get; private set; }
        
        private Ship(BoardPosition position, int length, Alignment alignment)
        {
            Id = Guid.NewGuid();
            StartingPosition = position;
            Length = length;
            Alignment = alignment;
            BoardPositions = new List<BoardPosition>();
            BoardPositions.AddRange(SetupPositions(position, length, alignment));
        }
        public static Ship Create(ShipViewModel shipViewModel)
        {
            return new Ship(shipViewModel.StartingPosition, shipViewModel.Length ,shipViewModel.Alignment);
        }
        private IEnumerable<BoardPosition> SetupPositions(BoardPosition position, int length, Alignment alignment)
        {
            var boardPositions = new List<BoardPosition>();
            for (var i = 0; i < length; i++)
            {
                if (alignment == Alignment.Horizontal)
                {
                    boardPositions.Add(new BoardPosition
                    {
                        X= position.X+i,
                        Y= position.Y,
                        Value = Id
                    });
                }
                else 
                {
                    boardPositions.Add(new BoardPosition
                    {
                        X= position.X,
                        Y= position.Y+i,
                        Value = Id
                    });
                }
            }
            return boardPositions;
        }
    }
    
    public class ShipViewModel
    {
        [Range(1, 10)]
        [Required]
        public int Length { get; set; }
        public  Alignment Alignment { get; set; }
        [Required]
        public  BoardPosition StartingPosition { get; set; }
    }
}