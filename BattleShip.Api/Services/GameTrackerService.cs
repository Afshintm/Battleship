using System;
using System.Collections.Generic;
using System.Net;
using BattleShip.Api.ExceptionHandling;
using BattleShip.Api.Extensions;
using BattleShip.Api.Models;

namespace BattleShip.Api.Services
{
    public interface IGameTrackerService
    {
        void CreateBoard(int dimensions = 10);
        void AddShip();
        GameStatus Status { get; set; }
        void SetStatus(GameStatus status);
        dynamic GetGameStatus();
        List<Ship> Ships { get; }
    }

    public class GameTrackerService: IGameTrackerService
    {
        
        private int _dimensions = 10;
        public List<Ship> Ships { get; set; }

        public GameTrackerService()
        {
            _status = GameStatus.NotStarted;
        }

        private GameStatus _status;
        public GameStatus Status { 
            get => _status;
            set=> _status = value ; 
        }

        public void CreateBoard(int dimensions = 10)
        {
            _dimensions = dimensions;
            if (_status == GameStatus.Playing)
                throw new HttpStatusCodeException(HttpStatusCode.Forbidden,$"You cannot setup the board while game status is {_status.ToString()}.");

            
            if(Ships== null)
                Ships = new List<Ship>();

            _status = GameStatus.Setup;
        }

        public void AddShip()
        {
            throw new System.NotImplementedException();
        }

       
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