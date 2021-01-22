using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FantasyRollerDerby.Models
{
    public class FantasyTeam
    {
        #region VARS
        public int FantasyTeamID { get; set; }  
        public int LeagueID { get; set; }
        public string LeagueName { get; set; }
        public int SeasonID { get; set; }
        public int SkaterID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Slogan { get; set; }
        public string Logo { get; set; }
        public int CreateByUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int? RetireByUserID { get; set; }
        public DateTime? RetireDate { get; set; }
        public int DraftPickNumber { get; set; }
        public double TotalTeamPoints { get; set; }
        public string errMsg { get; set; }
        #endregion

        #region METHODS
        public Return CreateNewFantasyTeam(SqlConnection dbconAPP, FantasyTeam model)
        {
            Return cR = new Return();

            //Create New Fantasy Team
            using (SqlCommand cmd = new SqlCommand("wboiFantasyTeam", dbconAPP))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", model.UserID);
                cmd.Parameters.AddWithValue("@LeagueID", model.LeagueID);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Slogan", model.Slogan);
                cmd.Parameters.AddWithValue("@Logo", model.Logo);
                cmd.Parameters.AddWithValue("@CreateByUserID", model.UserID);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                cmd.ExecuteScalar();
            }

            cR.ReturnFlag = true;
            return cR;
        }

        public List<FantasyTeam> GetSeasonFantasyTeamList(SqlConnection dbconAPP, int seasonID)
        {
            List<FantasyTeam> tempList = new List<FantasyTeam>();
            using (SqlCommand cmd = new SqlCommand("wbogLeagueSeasonFantasyTeamList", dbconAPP))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeasonID", seasonID);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            FantasyTeam tempModel = new FantasyTeam();
                            tempModel.FantasyTeamID = Convert.ToInt32(dr["FantasyTeamID"].ToString());
                            tempModel.Name = dr["Name"].ToString();
                            tempModel.UserID = Convert.ToInt32(dr["UserID"].ToString());
                            tempModel.Logo = dr["Logo"].ToString();
                            tempList.Add(tempModel);
                        }
                    }
                }
            }
            return tempList;
        }

        public Return AddSkaterToFantasyTeam(SqlConnection dbconAPP, FantasyTeam model)
        {
            Return cR = new Return();

            //Add skater to fantasy team for seasonID
            using (SqlCommand cmd = new SqlCommand("wboiFantasyTeamRoster", dbconAPP))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FantasyTeamID", model.FantasyTeamID);
                cmd.Parameters.AddWithValue("@SeasonID", model.SeasonID);
                cmd.Parameters.AddWithValue("@SkaterID", model.SkaterID);
                cmd.Parameters.AddWithValue("@DraftPickNumber", model.DraftPickNumber);
                cmd.Parameters.AddWithValue("@CreateByUserID", model.UserID);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                cmd.ExecuteScalar();
            }

            cR.ReturnFlag = true;
            return cR;
        }

        public Return GetFantasyTeamInfo(SqlConnection dbconAPP, FantasyTeam model)
        {
            Return cR = new Return();

            string strsql = "SELECT * FROM tblFantasyTeam WHERE FantasyTeamID = " + model.FantasyTeamID.ToString() +
                " AND RetireDate IS NULL";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            while (myReader.Read())
            {
                model.LeagueID = Convert.ToInt32(myReader["LeagueID"].ToString());
                model.Name = myReader["Name"].ToString();
                model.Slogan = myReader["Slogan"].ToString();
                model.Logo = myReader["Logo"].ToString();
                model.CreateByUserID = Convert.ToInt32(myReader["CreateByUserID"].ToString());
                model.CreateDate = Convert.ToDateTime(myReader["CreateDate"].ToString());
                count++;
            }
            myReader.Close();

            cR.ReturnFlag = true;
            if (count > 1)
            {
                cR.ReturnFlag = false;
                cR.ReturnMsg = "ERROR: More than one FantasyTeamID matched.";
            }
            else
            {
                cR.ReturnFlag = true;
                cR.ReturnMsg = "Success!";
            }
            return cR;
        }

        public List<FantasyTeam> GetFantasyTeamStandings(SqlConnection dbconAPP, int seasonID, 
            int leagueID)
        {
            List<FantasyTeam> tempList = new List<FantasyTeam>();

            //Get game stat data
            using (SqlCommand cmd = new SqlCommand("wbogSeasonStandings", dbconAPP))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeasonID", seasonID);
                cmd.Parameters.AddWithValue("@LeagueID", leagueID);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            FantasyTeam tempModel = new FantasyTeam();
                            tempModel.LeagueID = Convert.ToInt32(dr["LeagueID"].ToString());
                            tempModel.SeasonID = Convert.ToInt32(dr["SeasonID"].ToString());
                            tempModel.Name = dr["Name"].ToString();
                            tempModel.Logo = dr["Logo"].ToString();
                            tempModel.FantasyTeamID = Convert.ToInt32(dr["FantasyTeamID"].ToString());
                            tempModel.TotalTeamPoints = Convert.ToDouble(dr["TotalTeamFantasyPoints"].ToString());
                            tempList.Add(tempModel);
                        }
                    }
                }
            }
            return tempList;
        }
        
        public void UpdateFantasyTeamInfo(SqlConnection dbconAPP, int teamID, string teamName, 
            string teamSlogan, string teamLogo)
        {
            //Update Fantasy Team Info
            string sql = "UPDATE tblFantasyTeam SET Name = '" + teamName + "', Slogan = '" + teamSlogan + "', " +
                "Logo = '" + teamLogo + "' WHERE FantasyTeamID = " + teamID + "";
            var cmd = new SqlCommand(sql, dbconAPP);
            cmd.ExecuteScalar();
        }
        
        public bool IsSkaterDrafted(SqlConnection dbconAPP, int teamID, int seasonID, int skaterID)
        {
            bool result = false;
            string strsql = "SELECT * FROM tblFantasyTeamRoster WHERE FantasyTeamID = " + teamID +
                " AND SeasonID = " + seasonID + " AND SkaterID = " + skaterID + " AND RetireDate IS NULL";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            while (myReader.Read())
            {
                result = true;
            }
            myReader.Close();
            return result;
        }
        #endregion
    }
}