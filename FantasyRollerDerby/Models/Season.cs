using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace FantasyRollerDerby.Models
{
    public class Season
    {
        #region VARS
        public int SeasonID { get; set; }
        public int LeagueID { get; set; }
        public int MaxTeams { get; set; }
        public int DraftTimePerTeam { get; set; }
        public int RosterSize { get; set; }
        public string DraftOrder { get; set; }
        public int CurrentPickFantasyTeamID { get; set; }
        public string CurrentPickFantasyTeamName { get; set; }
        public DateTime DraftDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CreateByUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int RetireByUserID { get; set; }
        public DateTime RetireDate { get; set; }
        #endregion

        #region METHODS
        public int GetSeasonID(SqlConnection dbconAPP, League model)
        {
            int seasonID = 0;
            string strsql = "SELECT * FROM tblSeason WHERE LeagueID = " + model.LeagueID + 
                " AND MaxTeams = " + model.MaxTeams + " AND DraftDate = '" + model.DraftDate + 
                "' AND StartDate = '" + model.StartDate + "' AND EndDate = '" + model.EndDate + "' AND RetireDate IS NULL";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            while (myReader.Read())
            {
                seasonID = Convert.ToInt32(myReader["SeasonID"].ToString());
                count++;
            }
            myReader.Close();
            return count == 1 ? seasonID : 0;
        }

        public Return GetSeasonInfo(SqlConnection dbconAPP, Season model)
        {
            Return cR = new Return();

            string strsql = "SELECT * FROM tblSeason WHERE SeasonID = " + model.SeasonID +
                " AND RetireDate IS NULL";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            while (myReader.Read())
            {
                model.LeagueID = Convert.ToInt32(myReader["LeagueID"].ToString());
                model.MaxTeams = Convert.ToInt32(myReader["MaxTeams"].ToString());
                model.DraftTimePerTeam = Convert.ToInt32(myReader["DraftTimePerTeam"].ToString());
                model.RosterSize = Convert.ToInt32(myReader["RosterSize"].ToString());
                model.DraftOrder = myReader["DraftOrder"].ToString();
                model.DraftDate = Convert.ToDateTime(myReader["DraftDate"].ToString());
                model.StartDate = Convert.ToDateTime(myReader["StartDate"].ToString());
                model.EndDate = Convert.ToDateTime(myReader["EndDate"].ToString());
                count++;
            }
            myReader.Close();

            cR.ReturnFlag = true;
            if (count > 1)
            {
                cR.ReturnFlag = false;
                cR.ReturnMsg = "ERROR: More than one seasonID matched.";
            }
            else
            {
                cR.ReturnFlag = true;
                cR.ReturnMsg = "Success!";
            }
            return cR;
        }

        public Return UpdateDraftOrder(SqlConnection dbconAPP, int seasonID, string draftOrder)
        {
            Return cR = new Return();

            //Update draft order for season
            using (SqlCommand cmd = new SqlCommand("wbouDraftOrder", dbconAPP))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeasonID", seasonID);
                cmd.Parameters.AddWithValue("@DraftOrder", draftOrder);
                cmd.ExecuteScalar();
            }

            cR.ReturnFlag = true;
            return cR;
        }

        public Return UpdateCurrentPick(SqlConnection dbconAPP, int seasonID, int FantasyTeamID)
        {
            Return cR = new Return();

            //Update draft order for season
            using (SqlCommand cmd = new SqlCommand("wbouCurrentPick", dbconAPP))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeasonID", seasonID);
                cmd.Parameters.AddWithValue("@CurrentPickTeamID", FantasyTeamID);
                cmd.ExecuteScalar();
            }

            cR.ReturnFlag = true;
            return cR;
        }

        public int GetCurrentPickTeamID(SqlConnection dbconAPP, int seasonID)
        {
            int CurrentPickTeamID = 0;
            string strsql = "SELECT CurrentPickFantasyTeamID FROM tblSeason " + 
                "WHERE SeasonID = " + seasonID + " AND RetireDate IS NULL";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            while (myReader.Read())
            {
                CurrentPickTeamID = Convert.ToInt32(myReader["CurrentPickFantasyTeamID"].ToString());
                count++;
            }
            myReader.Close();
            return count == 1 ? CurrentPickTeamID : 0;
        }
        #endregion
    }
}