using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DbModels
{
    public class DbGameOdds
    {
        [Key]
        public int gameId { get; set; }
        public double draftKingsHomeOdds { get; set; }
        public double draftKingsAwayOdds { get; set; }
        public double myBookieHomeOdds { get; set; }
        public double myBookieAwayOdds { get; set; }
        public double betMgmHomeOdds { get; set; }
        public double betMgmAwayOdds { get; set; }
        public double modelHomeOdds { get; set; }
        public double modelAwayOdds { get; set; }
        [ForeignKey("gameId")]
        public DbGame game { get; set; } = null!;

        public void Clone(DbGameOdds gameOdds)
        {
            gameId = gameOdds.gameId;
            draftKingsHomeOdds = gameOdds.draftKingsHomeOdds;
            draftKingsAwayOdds = gameOdds.draftKingsAwayOdds;
            myBookieHomeOdds = gameOdds.myBookieHomeOdds;
            myBookieAwayOdds = gameOdds.myBookieAwayOdds;
            betMgmHomeOdds = gameOdds.betMgmHomeOdds;
            betMgmAwayOdds = gameOdds.betMgmAwayOdds;
            modelHomeOdds = gameOdds.modelHomeOdds;
            modelAwayOdds = gameOdds.modelAwayOdds;
        }
    }
}
