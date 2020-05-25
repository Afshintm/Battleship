using System;
using System.Collections.Generic;
using System.Dynamic;
using BattleShip.Api.ExceptionHandling;
using BattleShip.Api.Models;
using BattleShip.Api.Services;
using Microsoft.AspNetCore.Builder;

namespace BattleShip.Api.Extensions
{
    public static class ExtensionsClass
    {
        public static dynamic ToViewModel(this IGameTrackerService gameTrackerService)
        {
            if (gameTrackerService == null) return null;
            dynamic result = new ExpandoObject();
            result.status = gameTrackerService.Status.ToString();
            if(gameTrackerService.Ships!=null)
                result.ships = gameTrackerService.Ships;
            return result;
        }
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
        
        public static dynamic ToViewModel(this Ship ship)
        {
            if (ship == null) return null;
            dynamic result = new ExpandoObject();
            result.alignment = ship.Alignment.ToString();
            result.length = ship.Length;
            // result.startingPosition = new
            // {
            //     ship.StartingPosition.X, ship.StartingPosition.Y
            // };
            return result;
        }

        public static List<BoardPosition> BuildShipFrom(this BoardPosition startingPosition,Guid Id ,int length, Alignment alignment)
        {
            var boardPositions = new List<BoardPosition>();
            for (var i = 1; i < length; i++)
            {
                if (alignment == Alignment.Horizontal)
                {
                    boardPositions.Add(new BoardPosition
                    {
                        X= startingPosition.X+i,
                        Y= startingPosition.Y,
                        Value = Id
                    });
                }
                else 
                {
                    boardPositions.Add(new BoardPosition
                    {
                        X= startingPosition.X,
                        Y= startingPosition.Y+i,
                        Value = Id
                    });
                }
            }
            return boardPositions;
        }
        
        public static List<BoardPosition> BuildShipFrom(this BoardPosition startingPosition,int length, Alignment alignment)
        {
            if (startingPosition == null) return null;
            var boardPositions = new List<BoardPosition>{startingPosition};
            for (var i = 1; i < length; i++)
            {
                if (alignment == Alignment.Horizontal)
                {
                    boardPositions.Add(new BoardPosition
                    {
                        X= startingPosition.X+i,
                        Y= startingPosition.Y,
                        Value = startingPosition.Value
                    });
                }
                else 
                {
                    boardPositions.Add(new BoardPosition
                    {
                        X= startingPosition.X,
                        Y= startingPosition.Y+i,
                        Value = startingPosition.Value
                    });
                }
            }
            return boardPositions;
        }
        // private  IEnumerable<BoardPosition> SetupPositions(BoardPosition position, int length, Alignment alignment)
        // {
        //     var boardPositions = new List<BoardPosition>();
        //     for (var i = 0; i < length; i++)
        //     {
        //         if (alignment == Alignment.Horizontal)
        //         {
        //             boardPositions.Add(new BoardPosition
        //             {
        //                 X= position.X+i,
        //                 Y= position.Y,
        //                 Value = Id
        //             });
        //         }
        //         else 
        //         {
        //             boardPositions.Add(new BoardPosition
        //             {
        //                 X= position.X,
        //                 Y= position.Y+i,
        //                 Value = Id
        //             });
        //         }
        //     }
        //     return boardPositions;
        // }
    }
}