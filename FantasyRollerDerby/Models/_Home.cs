using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;

namespace FantasyRollerDerby.Models
{
    public class _Home
    {
        //All the classes for the home page feed into this class
        //All the lists needed for the home page are held here
        //Any methods specific to the home page are located here

        #region CLASSES
        public User cUser { get; set; }
        public League cLeague { get; set; }
        public FantasyTeam cFantasyTeam { get; set; }
        #endregion

        #region LISTS
        public List<League> list_cLeaguePublic { get; set; }
        public List<League> list_cLeaguePrivate { get; set; }
        public List<FantasyTeam> list_cFantasyTeam { get; set; }
        #endregion

        #region METHODS
        public Return GetLeagueMemberData(SqlConnection dbconAPP, _Home model)
        {
            Return cR = new Return();

            model.cLeague = new League();
            model.list_cLeaguePublic = new List<League>();
            model.list_cLeaguePrivate = new List<League>();
            model.list_cLeaguePublic = model.cLeague.GetLeagueList(dbconAPP, model.cLeague, true, model.cUser.UserID);
            model.list_cLeaguePrivate = model.cLeague.GetLeagueList(dbconAPP, model.cLeague, false, model.cUser.UserID);

            cR.ReturnFlag = true;
            return cR;
        }

        public Return GetNonLeagueMemberData(SqlConnection dbconAPP, _Home model)
        {
            Return cR = new Return();

            model.cLeague = new League();
            model.list_cLeaguePublic = new List<League>();
            model.list_cLeaguePrivate = new List<League>();
            model.list_cLeaguePublic = model.cLeague.GetLeagueList(dbconAPP, model.cLeague, true);
            model.list_cLeaguePrivate = model.cLeague.GetLeagueList(dbconAPP, model.cLeague, false);

            cR.ReturnFlag = true;
            return cR;
        }
        #endregion
    }
}