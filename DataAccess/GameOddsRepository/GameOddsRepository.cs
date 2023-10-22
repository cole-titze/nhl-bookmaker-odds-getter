using Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.GameOddsRepository
{
    public class GameOddsRepository : IGameOddsRepository
    {
        private readonly GameDbContext _dbContext;
        public GameOddsRepository(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Updates the game odds
        /// </summary>
        /// <param name="games">The game odds to update</param>
        /// <returns></returns>
        public async Task UpdateGameOdds(IEnumerable<DbGameOdds> games)
        {
            var gamesToUpdate = new List<DbGameOdds>();
            foreach(var game in games)
            {
                var dbGame = _dbContext.GameOdds.First(x => x.gameId == game.gameId);
                dbGame.Clone(game);
                gamesToUpdate.Add(dbGame);
            }

            _dbContext.GameOdds.UpdateRange(gamesToUpdate);
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Gets all game odds
        /// </summary>
        /// <returns>The season's game odds</returns>
        public async Task<IEnumerable<DbGameOdds>> GetGamesOdds()
        {
            return await _dbContext.GameOdds.Include(x => x.game)
                                            .ThenInclude( g => g.homeTeam)
                                            .Include(x => x.game)
                                            .ThenInclude(g => g.awayTeam)
                                            .ToListAsync();
        }
    }
}