using System.Dynamic;
using BattleShip.Api.Services;

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
    }
}