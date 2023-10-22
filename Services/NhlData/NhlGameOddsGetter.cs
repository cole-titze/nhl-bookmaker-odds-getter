using System.Reflection.Metadata.Ecma335;
using Entities.DbModels;
using Entities.Models;
using Microsoft.Extensions.Logging;
using Services.NhlData.Mappers;
using Services.RequestMaker;
namespace Services.NhlData
{
	public class NhlGameOddsGetter
	{
        private readonly IRequestMaker _requestMaker;
        private readonly ApiSettings _settings;
        private readonly ILogger<NhlGameOddsGetter> _logger;
        private dynamic? _futureGameResponseCache;

        public NhlGameOddsGetter(IRequestMaker reqMaker, ApiSettings apiSettings, ILoggerFactory loggerFactory)
		{
            _requestMaker = reqMaker;
            _logger = loggerFactory.CreateLogger<NhlGameOddsGetter>();
            _settings = apiSettings;
        }

        /// <summary>
        /// Calls the Odds api and gets the odds for a single game
        /// </summary>
        /// <param name="gameId">The game to get</param>
        /// <returns>A game odds object corresponding to the id passed in</returns>
        /// Example Request: 
        public async Task<DbGameOdds> GetGameOdds(DbGame game)
        {
            // TODO:
            // If the game date is in the past use the historical odds api
            if (game.gameDate < DateTime.UtcNow || game.gameDate > DateTime.UtcNow.AddDays(2))
                return new DbGameOdds();

            var gameOdds = await GetFutureGameOdds(game);

            return gameOdds;
        }
        // Example query: https://api.the-odds-api.com/v4/sports/icehockey_nhl/odds/?bookmakers=draftkings,betmgm,bovada,barstool&apiKey=
        private async Task<DbGameOdds> GetFutureGameOdds(DbGame game)
        {
            string url = "https://api.the-odds-api.com/v4/sports/icehockey_nhl/odds/?";
            string query = GetGameQuery(game);

            if(_futureGameResponseCache != null)
                return MapFutureGameOddsResponseToGameOdds.Map(game, _futureGameResponseCache);

            var gameResponse = await _requestMaker.MakeRequest(url, query);
            if (gameResponse == null)
            {
                _logger.LogWarning("Failed to get game with id: " + game.id.ToString());
                return new DbGameOdds();
            }
            _futureGameResponseCache = gameResponse;

            return MapFutureGameOddsResponseToGameOdds.Map(game, gameResponse);
        }
        /// <summary>
        /// Creates the game query
        /// </summary>
        /// <param name="game">The game to make the query for</param>
        /// <returns>Game query string</returns>
        private string GetGameQuery(DbGame game)
        {
            string urlParameters = $"&bookmakers=draftkings,betmgm,bovada,barstool&apiKey=" + _settings.OddsApiKey;

            return urlParameters;
        }
    }
}
