using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DbModels
{
    public class DbGame
    {
        public int id { get; set; }
        public int seasonStartYear { get; set; }
        public bool hasBeenPlayed { get; set; }
        public int homeTeamId { get; set; }
        public int awayTeamId { get; set; }
        public DateTime gameDate { get; set; }
        public TEAM winner { get; set; }
        [ForeignKey("homeTeamId")]
        public DbTeam homeTeam { get; set; } = null!;
        [ForeignKey("awayTeamId")]
        public DbTeam awayTeam { get; set; } = null!;
    }
    public enum TEAM
    {
        home = 0,
        away = 1
    }
}