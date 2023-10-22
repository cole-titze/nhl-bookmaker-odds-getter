using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DbModels
{
    public class DbTeam
    {
        public int id { get; set; }
        public string teamName { get; set; } = String.Empty;
        public string locationName { get; set; } = String.Empty;

        public string GetFullTeamName()
        {
            return locationName + " " + teamName;
        }
    }
}
