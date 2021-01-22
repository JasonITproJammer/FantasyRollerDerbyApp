using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyRollerDerby.Models
{
    public class _FantasyTeam
    {
        #region CLASSES
        public User cUser { get; set; }
        public League cLeague { get; set; }
        public Season cSeason { get; set; }
        public FantasyTeam cFantasyTeam { get; set; }
        public Skater cSkater { get; set; }
        public DerbyTeam cDerbyTeam { get; set; }
        #endregion

        #region LISTS
        public List<League> list_cLeague { get; set; }
        public List<Season> list_cSeason { get; set; }
        public List<FantasyTeam> list_cFantasyTeam { get; set; }
        public List<Skater> list_cSkater { get; set; }
        public List<DerbyTeam> list_cDerbyTeam { get; set; }
        #endregion

        #region METHODS

        #endregion
    }
}