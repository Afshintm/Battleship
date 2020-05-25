namespace BattleShip.Api.Models
{
    public class BoardPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"Board position X:{X}, Y:{Y}.";
        }
    }
}