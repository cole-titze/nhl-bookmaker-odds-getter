using Entities.DbModels;
using Microsoft.Extensions.Logging;
using Services.NhlData.Mappers;
using Services.RequestMaker;
namespace Services.NhlData
{
	public class NhlGameOddsGetter
	{
        private readonly IRequestMaker _requestMaker;
        private readonly ILogger<NhlGameOddsGetter> _logger;

        public NhlGameOddsGetter(IRequestMaker reqMaker, ILoggerFactory loggerFactory)
		{
            _requestMaker = reqMaker;
            _logger = loggerFactory.CreateLogger<NhlGameOddsGetter>();
        }

        /// <summary>
        /// Calls the Odds api and gets the odds for a single game
        /// </summary>
        /// <param name="gameId">The game to get</param>
        /// <returns>A game odds object corresponding to the id passed in</returns>
        /// Example Request: 
        public async Task<DbGameOdds> GetGameOdds(int gameId)
        {
            string url = "https://api.the-odds-api.com/v4/sports/?apiKey=YOUR_API_KEY";
            string query = GetGameQuery(gameId);
            var gameResponse = await _requestMaker.MakeRequest(url, query);
            if (gameResponse == null)
            {
                _logger.LogWarning("Failed to get game with id: " + gameId.ToString());
                return new DbGameOdds();
            }

            return MapGameOddsResponseToGameOdds.Map(gameResponse);
        }
        /// <summary>
        /// Creates the game query
        /// </summary>
        /// <param name="seasonStartYear"></param>
        /// <param name="id"></param>
        /// <returns>Game query string</returns>
        private string GetGameQuery(int id)
        {
            string urlParameters = $"{id}/feed/live";

            return urlParameters;
        }
    }
}
