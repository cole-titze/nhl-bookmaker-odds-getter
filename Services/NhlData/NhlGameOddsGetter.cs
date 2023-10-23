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
        private readonly Dictionary<DateTime, dynamic?> _pastGameResponseCache;

        public NhlGameOddsGetter(IRequestMaker reqMaker, ApiSettings apiSettings, ILoggerFactory loggerFactory)
		{
            _requestMaker = reqMaker;
            _logger = loggerFactory.CreateLogger<NhlGameOddsGetter>();
            _settings = apiSettings;
            _pastGameResponseCache = new Dictionary<DateTime, dynamic?>();
        }

        /// <summary>
        /// Calls the Odds api and gets the odds for a single game
        /// </summary>
        /// <param name="gameId">The game to get</param>
        /// <returns>A game odds object corresponding to the id passed in</returns>
        /// Example Request: 
        public async Task<DbGameOdds> GetGameOdds(DbGame game)
        {
            DbGameOdds gameOdds;
            if (game.gameDate > DateTime.UtcNow.AddDays(2))
                return new DbGameOdds();

            if(game.gameDate < DateTime.UtcNow)
            {
                return new DbGameOdds();
                // Until I upgrade api, skip past game odds
                // gameOdds = await GetPastGameOdds(game);
            }
            else
            {
                gameOdds = await GetFutureGameOdds(game);
            }

            return gameOdds;
        }

        // Example query: https://api.the-odds-api.com/v4/sports/icehockey_nhl/odds-history/?bookmakers=draftkings,betmgm,bovada,barstool&date=2021-10-18T12:00:00Z&apiKey=
        /// <summary>
        /// Gets the game odds for a game that happened in the past
        /// </summary>
        /// <param name="game">The game to get the odds of</param>
        /// <returns>Game odds for the given game</returns>
        private async Task<DbGameOdds> GetPastGameOdds(DbGame game)
        {
            string url = "https://api.the-odds-api.com/v4/sports/icehockey_nhl/odds-history/?";
            string query = GetPastGameQuery(game);

            if (_pastGameResponseCache.ContainsKey(game.gameDate.Date))
                return MapGameOddsResponseToGameOdds.MapPastGame(game, _pastGameResponseCache);

            var gameResponse = await _requestMaker.MakeRequest(url, query);
            if (gameResponse == null)
            {
                _logger.LogWarning("Failed to get game with id: " + game.id.ToString());
                return new DbGameOdds();
            }
            _pastGameResponseCache.Add(game.gameDate.Date, gameResponse);

            return MapGameOddsResponseToGameOdds.MapPastGame(game, _pastGameResponseCache);
        }

        // Example query: https://api.the-odds-api.com/v4/sports/icehockey_nhl/odds/?bookmakers=draftkings,betmgm,bovada,barstool&apiKey=
        /// <summary>
        /// Gets the game odds for a game in the future
        /// </summary>
        /// <param name="game">The game to get the odds for</param>
        /// <returns>The game odds for the given game</returns>
        private async Task<DbGameOdds> GetFutureGameOdds(DbGame game)
        {
            string url = "https://api.the-odds-api.com/v4/sports/icehockey_nhl/odds/?";
            string query = GetFutureGameQuery(game);

            if(_futureGameResponseCache != null)
                return MapGameOddsResponseToGameOdds.MapFutureGame(game, _futureGameResponseCache);

            var gameResponse = await _requestMaker.MakeRequest(url, query);
            if (gameResponse == null)
            {
                _logger.LogWarning("Failed to get game with id: " + game.id.ToString());
                return new DbGameOdds();
            }
            _futureGameResponseCache = gameResponse;

            return MapGameOddsResponseToGameOdds.MapFutureGame(game, gameResponse);
        }
        /// <summary>
        /// Creates the game query for a game in the future
        /// </summary>
        /// <param name="game">The game to make the query for</param>
        /// <returns>Game query string</returns>
        private string GetFutureGameQuery(DbGame game)
        {
            string urlParameters = $"&bookmakers=draftkings,betmgm,bovada,barstool&apiKey=" + _settings.OddsApiKey;

            return urlParameters;
        }
        /// <summary>
        /// Creates the game query for a game in the future
        /// </summary>
        /// <param name="game">The game to make the query for</param>
        /// <returns>Game query string</returns>
        private string GetPastGameQuery(DbGame game)
        {
            string date = game.gameDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z";
            string urlParameters = $"&bookmakers=draftkings,betmgm,bovada,barstool&date={date}&apiKey={_settings.OddsApiKey}";

            return urlParameters;
        }
    }
}
