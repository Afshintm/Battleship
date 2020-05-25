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
            result.startingPosition = new
            {
                ship.StartingPosition.X, ship.StartingPosition.Y
            };
            return result;
        }
    }
}