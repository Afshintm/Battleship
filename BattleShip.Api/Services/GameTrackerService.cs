using BattleShip.Api.Models;

namespace BattleShip.Api.Services
{
    public interface IGameTrackerService
    {
        void AddShip();
        GameStatus Status { get; }
        void SetStatus(GameStatus status);
    }

    public class GameTrackerService: IGameTrackerService
    {
        public void AddShip()
        {
            throw new System.NotImplementedException();
        }

        public GameStatus Status { get; }
        public void SetStatus(GameStatus status)
        {
            throw new System.NotImplementedException();
        }
    }
}