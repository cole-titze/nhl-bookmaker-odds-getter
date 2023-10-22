using Entities.DbModels;
using FuzzySharp;

namespace Services.NhlData.Mappers
{
	public static class MapFutureGameOddsResponseToGameOdds
	{
        public static DbGameOdds Map(DbGame game, dynamic message)
        {
            var gameResponse = GetGameFromResponse(game, message);

            if(gameResponse == null)
                return new DbGameOdds();

            var gameOdds = new DbGameOdds()
            {
                gameId = game.id,
                draftKingsHomeOdds = (double)gameResponse.bookmakers[0].markets[0].outcomes[0].price,
                draftKingsAwayOdds = (double)gameResponse.bookmakers[0].markets[0].outcomes[1].price,
                betMgmHomeOdds = (double)gameResponse.bookmakers[1].markets[0].outcomes[0].price,
                betMgmAwayOdds = (double)gameResponse.bookmakers[1].markets[0].outcomes[1].price,
                bovadaHomeOdds = (double)gameResponse.bookmakers[2].markets[0].outcomes[0].price,
                bovadaAwayOdds = (double)gameResponse.bookmakers[2].markets[0].outcomes[1].price,
                barstoolHomeOdds = (double)gameResponse.bookmakers[3].markets[0].outcomes[0].price,
                barstoolAwayOdds = (double)gameResponse.bookmakers[3].markets[0].outcomes[1].price,
            };

            return gameOdds;
        }

        private static dynamic? GetGameFromResponse(DbGame game, dynamic message)
        {
            foreach(var gameResponse in message)
            {
                // Game is live or old
                if (DateTime.Parse((string)gameResponse.commence_time) < DateTime.UtcNow)
                    continue;

                if (Fuzz.Ratio(game.homeTeam.GetFullTeamName(), (string)message.home_team) >= .9 && Fuzz.Ratio(game.awayTeam.GetFullTeamName(), message.away_team) >= .9)
                    return gameResponse;
            }

            return null;
        }
    }
}

