using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//added
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;

namespace FantasyRollerDerby.Models
{
    public class _League
    {
        #region VARS
        #endregion

        #region CLASSES
        public User cUser { get; set; }
        public League cLeague { get; set; }
        public Season cSeason { get; set; }
        public StatType cStatType { get; set; }
        public FantasyTeam cFantasyTeam { get; set; }
        public Skater cSkater { get; set; }
        public GameStat cGameStat { get; set; }
        #endregion

        #region LISTS
        public List<League> list_cLeague { get; set; }
        public List<Season> list_cSeason { get; set; }
        public List<StatType> list_cStatType { get; set; }
        public List<StatType> list_DefaultStats { get; set; }
        public List<FantasyTeam> list_cFantasyTeam { get; set; }
        public List<FantasyTeam> list_LeagueStanding { get; set; }
        public List<FantasyTeam> list_SeasonFantasyTeams { get; set; }
        public List<FantasyTeam> list_DraftOrderTeams { get; set; }
        public List<Skater> list_Skater { get; set; }
        public List<Skater> list_SkaterGameStats { get; set; }
        #endregion

        #region METHODS
        public Return CreateNewLeague(SqlConnection dbconAPP, _League model)
        {
            Return cR = new Return();

            //Create the league
            using (SqlCommand cmd = new SqlCommand("wboiLeague", dbconAPP))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", model.cUser.UserID);
                cmd.Parameters.AddWithValue("@LeagueName", model.cLeague.LeagueName);
                cmd.Parameters.AddWithValue("@isPublic", model.cLeague.isPublic);
                cmd.Parameters.AddWithValue("@Password", model.cLeague.Password);
                cmd.Parameters.AddWithValue("@CreateByUserID", model.cUser.UserID);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                cmd.ExecuteScalar();
            }

            //Get the league's id
            model.cLeague.LeagueID = model.cLeague.GetLeagueID(dbconAPP, model.cLeague.LeagueName, model.cUser.UserID);
            if (model.cLeague.LeagueID == 0)
            {
                cR.ReturnMsg += "More than one League ID was returned (League.GetLeagueID). ";
                cR.ReturnFlag = false;
                return cR;
            }

            //Create the league's season
            using (SqlCommand cmd = new SqlCommand("wboiSeason", dbconAPP))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LeagueID", model.cLeague.LeagueID);
                cmd.Parameters.AddWithValue("@MaxTeams", model.cLeague.MaxTeams);
                cmd.Parameters.AddWithValue("@DraftTimePerTeam", model.cLeague.DraftTimePerTeam);
                cmd.Parameters.AddWithValue("@RosterSize", model.cLeague.RosterSize);
                cmd.Parameters.AddWithValue("@DraftDate", model.cLeague.DraftDate);
                cmd.Parameters.AddWithValue("@StartDate", model.cLeague.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", model.cLeague.EndDate);
                cmd.Parameters.AddWithValue("@CreateByUserID", model.cUser.UserID);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                cmd.ExecuteScalar();
            }

            //Get the league's season id
            model.cSeason = new Season();
            model.cSeason.SeasonID = model.cSeason.GetSeasonID(dbconAPP, model.cLeague);
            if (model.cSeason.SeasonID == 0)
            {
                cR.ReturnMsg += "More than one Season ID was returned (League.GetSeasonID). ";
                cR.ReturnFlag = false;
                return cR;
            }

            //Create the League's Stat Types, Get league stat types ids and Create stat type values
            foreach (var item in model.list_cStatType)
            {
                if(item.StatTypeID < 1)
                {
                    //Create league stat types
                    using (SqlCommand cmd = new SqlCommand("wboiStatType", dbconAPP))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StatName", item.Name);
                        cmd.Parameters.AddWithValue("@StatNameCode", item.NameCode);
                        cmd.Parameters.AddWithValue("@StatDescription", item.Description);
                        cmd.Parameters.AddWithValue("@CreateByUserID", model.cUser.UserID);
                        cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                        cmd.ExecuteScalar();
                    }

                    //Get league stat type id
                    item.StatTypeID = item.GetStatTypeID(dbconAPP, item);
                    if (item.StatTypeID == 0)
                    {
                        cR.ReturnMsg += "More than one StatType ID was returned (League.GetStatTypeID). ";
                        cR.ReturnFlag = false;
                        return cR;
                    }
                }
                
                //Create stat type value xref
                using (SqlCommand cmd = new SqlCommand("wboiLeagueStatTypeValueXref", dbconAPP))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StatTypeID", item.StatTypeID);
                    cmd.Parameters.AddWithValue("@LeagueID", model.cLeague.LeagueID);
                    cmd.Parameters.AddWithValue("@StatTypeValue", item.cStatTypeValue.StatValue);
                    cmd.Parameters.AddWithValue("@CreateByUserID", model.cUser.UserID);
                    cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                    cmd.ExecuteScalar();
                }
            }

            cR.ReturnFlag = true;
            return cR;
        }
        #endregion

    }
}