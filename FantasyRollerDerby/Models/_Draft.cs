using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FantasyRollerDerby.Models
{
    public class _Draft
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
        public List<FantasyTeam> list_SeasonFantasyTeams { get; set; }
        public List<FantasyTeam> list_DraftOrderTeams { get; set;}
        #endregion

        public Return GetDraftInfo(SqlConnection dbconAPP, _Draft model)
        {
            Return cR = new Return();

            //get derby teams
            model.cDerbyTeam = new DerbyTeam();
            model.list_cDerbyTeam = new List<DerbyTeam>();
            model.list_cDerbyTeam = model.cDerbyTeam.GetDerbyTeamList();

            //get skaters
            model.cSkater = new Skater();
            model.list_cSkater = new List<Skater>();
            model.list_cSkater = model.cSkater.GetSkaterList(dbconAPP, model.cFantasyTeam.SeasonID);

            //get season draft date and time
            model.cSeason.SeasonID = model.cFantasyTeam.SeasonID;
            model.cSeason.GetSeasonInfo(dbconAPP, model.cSeason);

            //Get league info
            model.cLeague = model.cLeague.GetLeagueInfo(dbconAPP, model.cSeason.LeagueID, model.cSeason.SeasonID);

            //get list of the season's fantasy teams
            model.list_SeasonFantasyTeams = model.cFantasyTeam.GetSeasonFantasyTeamList(dbconAPP, model.cSeason.SeasonID);

            cR.ReturnFlag = true;
            return cR;
        }

        public bool TeamDraftedCount(SqlConnection dbconAPP, int seasonID, int teamID, int draftPickNumber)
        {
            int teamDrafted = 0;
            string strsql = "SELECT COUNT(*) FROM tblFantasyTeamRoster " +
                "WHERE SeasonID = " + seasonID + 
                " AND " + "FantasyTeamID = " + teamID +
                " AND " + "DraftPickNumber = " + draftPickNumber;
            var cmd = new SqlCommand(strsql, dbconAPP);
            teamDrafted = Convert.ToInt32(cmd.ExecuteScalar());
            return teamDrafted > 0 ? true : false;
        }
    }
}