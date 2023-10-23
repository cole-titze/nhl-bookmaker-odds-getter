﻿using Entities.DbModels;
using FuzzySharp;

namespace Services.NhlData.Mappers
{
	public static class MapGameOddsResponseToGameOdds
	{
        public static DbGameOdds MapFutureGame(DbGame game, dynamic message)
        {
            var gameResponse = GetGameFromResponse(game, message);

            if(gameResponse == null)
                return new DbGameOdds();

            var gameOdds = new DbGameOdds()
            {
                gameId = game.id,
            };
            foreach(var bookmaker in gameResponse.bookmakers)
            {
                switch ((string)bookmaker.key)
                {
                    case "betmgm":
                        gameOdds.betMgmHomeOdds = (double)bookmaker.markets[0].outcomes[0].price;
                        gameOdds.betMgmAwayOdds = (double)bookmaker.markets[0].outcomes[1].price;
                        break;
                    case "bovada":
                        gameOdds.bovadaHomeOdds = (double)bookmaker.markets[0].outcomes[0].price;
                        gameOdds.bovadaAwayOdds = (double)bookmaker.markets[0].outcomes[1].price;
                        break;
                    case "barstool":
                        gameOdds.barstoolHomeOdds = (double)bookmaker.markets[0].outcomes[0].price;
                        gameOdds.barstoolAwayOdds = (double)bookmaker.markets[0].outcomes[1].price;
                        break;
                    case "draftkings":
                        gameOdds.draftKingsHomeOdds = (double)bookmaker.markets[0].outcomes[0].price;
                        gameOdds.draftKingsAwayOdds = (double)bookmaker.markets[0].outcomes[1].price;
                        break;
                }
            }

            return gameOdds;
        }

        public static DbGameOdds MapPastGame(DbGame game, Dictionary<DateTime, dynamic?> message)
        {
            var gameResponse = GetGameFromResponse(game, message);

            if (gameResponse == null)
                return new DbGameOdds();


            // TODO: Make nullable and continue if missing bookmaker
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

                if (hasSameTeams(game, gameResponse) && hasSameDate(game.gameDate, gameResponse))
                    return gameResponse;
            }

            return null;
        }

        private static bool hasSameDate(DateTime gameDate, dynamic gameResponse)
        {
            var utcVegasGameDate = DateTime.Parse((string)gameResponse.commence_time);
            return utcVegasGameDate >= gameDate.AddHours(-2) && utcVegasGameDate <= gameDate.AddHours(2) ? true : false;
        }

        private static bool hasSameTeams(DbGame game, dynamic gameResponse)
        {
            return (Fuzz.Ratio(game.homeTeam.GetFullTeamName(), (string)gameResponse.home_team) >= .9 && Fuzz.Ratio(game.awayTeam.GetFullTeamName(), (string)gameResponse.away_team) >= .9) ? true : false;
        }
    }
}

