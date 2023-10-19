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
            _dbContext.GameOdds.UpdateRange(games);
            await _dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Gets all game odds
        /// </summary>
        /// <returns>The season's game odds</returns>
        public async Task<IEnumerable<DbGameOdds>> GetGamesOdds()
        {
            return await _dbContext.GameOdds.Include(x => x.game).ToListAsync();
        }
    }
}