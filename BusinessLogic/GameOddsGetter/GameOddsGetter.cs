using DataAccess.GameOddsRepository;
using Microsoft.Extensions.Logging;
using Services.NhlData;
using Entities.DbModels;

namespace BusinessLogic.GameOddsGetter
{
    public class GameOddsGetter
    {
        private readonly IGameOddsRepository _gameOddsRepo;
        private readonly ILogger<GameOddsGetter> _logger;
        private readonly NhlGameOddsGetter _nhlGameOddsGetter;
        public GameOddsGetter(IGameOddsRepository gameOddsRepository, NhlGameOddsGetter nhlGameOddsGetter, ILoggerFactory loggerFactory)
        {
            _gameOddsRepo = gameOddsRepository;
            _logger = loggerFactory.CreateLogger<GameOddsGetter>();
            _nhlGameOddsGetter = nhlGameOddsGetter;
        }
        /// <summary>
        /// Gets all nhl games within the season range. If the game is already in the database, it is skipped.
        /// </summary>
        public async Task<IEnumerable<DbGameOdds>> GetGameOdds()
        {
            var gamesOdds = await _gameOddsRepo.GetGamesOdds();

            foreach(var gameOdds in gamesOdds)
            {
                if (gameOdds.IsCalculated())
                    continue;

                var vegasGameOdds = await _nhlGameOddsGetter.GetGameOdds(gameOdds.game);
                gameOdds.draftKingsHomeOdds = vegasGameOdds.draftKingsHomeOdds;
                gameOdds.draftKingsAwayOdds = vegasGameOdds.draftKingsAwayOdds;
            }

            return gamesOdds;
        }
    }
}