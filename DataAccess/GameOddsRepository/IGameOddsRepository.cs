using Entities.DbModels;

namespace DataAccess.GameOddsRepository
{
	public interface IGameOddsRepository
	{
        Task UpdateGameOdds(IEnumerable<DbGameOdds> games);
        Task<IEnumerable<DbGameOdds>> GetGamesOdds();
    }
}
