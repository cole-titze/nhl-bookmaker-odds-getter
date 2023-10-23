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
            var currentGamesOdds = await _gameOddsRepo.GetGamesOdds();
            var updatedGameOdds = new List<DbGameOdds>();

            foreach(var gameOdds in currentGamesOdds)
            {
                if (gameOdds.IsCalculated())
                    continue;

                var vegasGameOdds = await _nhlGameOddsGetter.GetGameOdds(gameOdds.game);
                if (vegasGameOdds.gameId == 0)
                    continue;

                updatedGameOdds.Add(vegasGameOdds);
            }
            _logger.LogInformation("Number of Game Odds To Add: " + updatedGameOdds.Count.ToString());
            return updatedGameOdds;
        }
    }
}