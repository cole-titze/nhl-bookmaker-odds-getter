using Entities.DbModels;

namespace Services.NhlData.Mappers
{
	public static class MapGameOddsResponseToGameOdds
	{
        public static DbGameOdds Map(dynamic message)
        {
            var game = new DbGameOdds()
            {
                gameId = (int)message.gamePk,
                draftKingsHomeOdds = (int)message,
                draftKingsAwayOdds = (int)message,
                myBookieHomeOdds = (int)message,
                myBookieAwayOdds = (int)message,
                betMgmHomeOdds = (int)message,
                betMgmAwayOdds = (int)message,
        };

            return game;
        }
    }
}

