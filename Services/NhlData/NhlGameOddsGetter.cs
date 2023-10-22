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
        private readonly Dictionary<DateTime, dynamic> _gameResponseCache;

        public NhlGameOddsGetter(IRequestMaker reqMaker, ApiSettings apiSettings, ILoggerFactory loggerFactory)
		{
            _requestMaker = reqMaker;
            _logger = loggerFactory.CreateLogger<NhlGameOddsGetter>();
            _settings = apiSettings;
            _gameResponseCache = new Dictionary<DateTime, dynamic>();
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
            // If the game date is today or two days from today, get the odds once and cache for future games
            // If the game date is later than two days from now, skip
            // Parse game data and match by teams playing and date?

            if (game.gameDate < DateTime.UtcNow)
                return new DbGameOdds();

            var gameOdds = await GetFutureGameOdds(game);

            return gameOdds;
        }
        // Example query: https://api.the-odds-api.com/v4/sports/icehockey_nhl/odds/?bookmakers=draftkings,betmgm,bovada,barstool&commenceTimeFrom=2023-10-21T17:00:00Z&commenceTimeTo=2023-10-22T17:00:00Z&apiKey=
        private async Task<DbGameOdds> GetFutureGameOdds(DbGame game)
        {
            string url = "https://api.the-odds-api.com/v4/sports/icehockey_nhl/odds/?regions=us";
            string query = GetGameQuery(game);

            if(_gameResponseCache.ContainsKey(game.gameDate.Date))
                return MapFutureGameOddsResponseToGameOdds.Map(game, _gameResponseCache[game.gameDate.Date]);

            var gameResponse = await _requestMaker.MakeRequest(url, query);
            if (gameResponse == null)
            {
                _logger.LogWarning("Failed to get game with id: " + game.id.ToString());
                return new DbGameOdds();
            }
            _gameResponseCache.Add(game.gameDate.Date, gameResponse);

            return MapFutureGameOddsResponseToGameOdds.Map(game, gameResponse);
        }
        /// <summary>
        /// Creates the game query
        /// </summary>
        /// <param name="id">Id of the game to get</param>
        /// <returns>Game query string</returns>
        private string GetGameQuery(DbGame game)
        {
            string startDate = game.gameDate.AddHours(-12).ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z";
            string endDate = game.gameDate.AddHours(+12).ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z";
            string urlParameters = $"&commenceTimeFrom={startDate}&commenceTimeTo={endDate}&bookmakers=draftkings,betmgm,bovada,barstool&apiKey=" + _settings.OddsApiKey;

            return urlParameters;
        }
    }
}
