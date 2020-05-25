using System;

namespace BattleShip.Api.Models
{
    public class Ship
    {
        public Guid Id { get; private set; }
        public int Length { get; private set; }
        public  Alignment Alignment { get; private set; }
    }
}