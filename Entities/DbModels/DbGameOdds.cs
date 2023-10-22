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
        public double bovadaHomeOdds { get; set; }
        public double bovadaAwayOdds { get; set; }
        public double betMgmHomeOdds { get; set; }
        public double betMgmAwayOdds { get; set; }
        public double barstoolHomeOdds { get; set; }
        public double barstoolAwayOdds { get; set; }
        public double modelHomeOdds { get; set; }
        public double modelAwayOdds { get; set; }
        [ForeignKey("gameId")]
        public DbGame game { get; set; } = null!;

        public void Clone(DbGameOdds gameOdds)
        {
            gameId = gameOdds.gameId;
            draftKingsHomeOdds = gameOdds.draftKingsHomeOdds;
            draftKingsAwayOdds = gameOdds.draftKingsAwayOdds;
            bovadaHomeOdds = gameOdds.bovadaHomeOdds;
            bovadaAwayOdds = gameOdds.bovadaAwayOdds;
            betMgmHomeOdds = gameOdds.betMgmHomeOdds;
            betMgmAwayOdds = gameOdds.betMgmAwayOdds;
            barstoolHomeOdds = gameOdds.barstoolHomeOdds;
            barstoolAwayOdds = gameOdds.barstoolAwayOdds;
            modelHomeOdds = gameOdds.modelHomeOdds;
            modelAwayOdds = gameOdds.modelAwayOdds;
        }

        public bool IsCalculated()
        {
            if(!draftKingsAwayOdds.Equals(0) && !draftKingsHomeOdds.Equals(0) && !bovadaAwayOdds.Equals(0) && !bovadaAwayOdds.Equals(0) && !betMgmAwayOdds.Equals(0) && !betMgmHomeOdds.Equals(0) && !barstoolAwayOdds.Equals(0) && !barstoolHomeOdds.Equals(0))
                return true;

            return false;
        }
    }
}
