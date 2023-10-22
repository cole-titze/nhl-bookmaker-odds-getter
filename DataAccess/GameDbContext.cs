using Entities.DbModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public partial class GameDbContext : DbContext
    {
        private readonly string _connectionString;
        public GameDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public virtual DbSet<DbGame> Game { get; set; } = null!;
        public virtual DbSet<DbTeam> Team { get; set; } = null!;
        public virtual DbSet<DbGameOdds> GameOdds { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}