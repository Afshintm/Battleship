using System;
using System.Collections.Generic;
using BattleShip.Api.Extensions;
using BattleShip.Api.Models;

namespace BattleShip.Api.Services
{
    public interface IGameTrackerService
    {
        void CreateBoard(int dimensions = 10);
        void AddShip();
        GameStatus Status { get; }
        void SetStatus(GameStatus status);
        dynamic GetGameStatus();
        List<Ship> Ships { get; }
    }

    public class GameTrackerService: IGameTrackerService
    {
        
        private int _dimensions = 10;
        private GameStatus _status;
        public List<Ship> Ships { get; set; }

        public GameTrackerService()
        {
            _status = GameStatus.NotStarted;
        }

        public void CreateBoard(int dimensions = 10)
        {
            _dimensions = dimensions;
            if (_status == GameStatus.Playing)
                throw new  ApplicationException("Cannot setup board while playing.");
            
            if(Ships== null)
                Ships = new List<Ship>();

            _status = GameStatus.Setup;
        }

        public void AddShip()
        {
            throw new System.NotImplementedException();
        }

        public GameStatus Status { get; }
        public void SetStatus(GameStatus status)
        {
            throw new System.NotImplementedException();
        }

        public dynamic GetGameStatus()
        {
            return this.ToViewModel();
        }
    }
}