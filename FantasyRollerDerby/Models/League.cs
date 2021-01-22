using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;

namespace FantasyRollerDerby.Models
{
    public class League
    {
        #region VARS
        public int LeagueID { get; set; }
        public int CommissionerUserID { get; set; }
        public int FantasyTeamID { get; set; }
        public string CommissionerUserName { get; set; }
        public string TeamName { get; set; }
        public string Logo { get; set; }
        public string Slogan { get; set; }
        [Required]
        public string LeagueName { get; set; }
        public int SeasonID { get; set; }
        [Required]
        public int MaxTeams { get; set; }
        public int Teams { get; set; }
        [Required]
        public DateTime DraftDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Required]
        public DateTime StartDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Required]
        public DateTime EndDate { get; set; }

        public int CreateByUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int? RetireByUserID { get; set; }
        public DateTime? RetireDate { get; set; }
        public Boolean isPublic { get; set; }
        public string Password { get; set; }
        public int DraftTimePerTeam { get; set; }
        public int RosterSize { get; set; }
        #endregion

        #region METHODS
        public List<League> GetLeagueList(SqlConnection dbconAPP, League model, bool isPublic)
        {
            List<League> listReturn = new List<League>();
            using (SqlCommand cmd = new SqlCommand("wbogLeagueList", dbconAPP))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@isPublic", isPublic);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            League tempModel = new League();
                            tempModel.LeagueID = Convert.ToInt32(dr["LeagueID"].ToString());
                            tempModel.CommissionerUserID = Convert.ToInt32(dr["CommissionerUserID"].ToString());
                            tempModel.CommissionerUserName = dr["CommissionerUserName"].ToString();
                            tempModel.LeagueName = dr["LeagueName"].ToString();
                            tempModel.SeasonID = Convert.ToInt32(dr["SeasonID"].ToString());
                            tempModel.MaxTeams = Convert.ToInt32(dr["MaxTeams"].ToString());
                            tempModel.Teams = Convert.ToInt32(dr["Teams"].ToString());
                            tempModel.DraftDate = Convert.ToDateTime(dr["DraftDate"].ToString());
                            tempModel.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                            tempModel.EndDate = Convert.ToDateTime(dr["EndDate"].ToString());
                            tempModel.CreateByUserID = Convert.ToInt32(dr["CreateByUserID"].ToString());
                            tempModel.CreateDate = Convert.ToDateTime(dr["CreateDate"].ToString());
                            tempModel.RetireByUserID = dr["RetireByUserID"].ToString() == "" ? default(Nullable<int>) : Convert.ToInt32(dr["RetireByUserID"].ToString());
                            tempModel.RetireDate = dr["RetireDate"].ToString() == "" ? default(Nullable<DateTime>) : Convert.ToDateTime(dr["RetireDate"].ToString());
                            tempModel.isPublic = Convert.ToBoolean(dr["isPublic"].ToString());
                            listReturn.Add(tempModel);
                        }
                    }
                }
            }
            return listReturn;
        }

        public List<League> GetLeagueList(SqlConnection dbconAPP, League model, bool isPublic, int userid)
        {
            List<League> listReturn = new List<League>();
            using (SqlCommand cmd = new SqlCommand("wbogMemberLeagueList", dbconAPP))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@isPublic", isPublic);
                cmd.Parameters.AddWithValue("@UserID", userid);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            League tempModel = new League();
                            tempModel.FantasyTeamID = Convert.ToInt32(dr["FantasyTeamID"].ToString());
                            tempModel.TeamName = dr["Name"].ToString();
                            tempModel.LeagueID = Convert.ToInt32(dr["LeagueID"].ToString());
                            tempModel.LeagueName = dr["LeagueName"].ToString();
                            tempModel.CommissionerUserID = Convert.ToInt32(dr["CommissionerUserID"].ToString());
                            tempModel.CommissionerUserName = dr["UserName"].ToString();
                            tempModel.Logo = dr["Logo"].ToString();
                            tempModel.Slogan = dr["Slogan"].ToString();
                            tempModel.SeasonID = Convert.ToInt32(dr["SeasonID"].ToString());
                            tempModel.DraftDate = Convert.ToDateTime(dr["DraftDate"].ToString());
                            tempModel.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                            tempModel.EndDate = Convert.ToDateTime(dr["EndDate"].ToString());
                            tempModel.isPublic = Convert.ToBoolean(dr["isPublic"].ToString());
                            listReturn.Add(tempModel);
                        }
                    }
                }
            }
            return listReturn;
        }

        public bool VerifyLeagueUsers(SqlConnection dbconAPP, League model, int userID)
        {
            string strsql = "SELECT * FROM tblFantasyTeam WHERE UserID = " + userID + " AND LeagueID = " + model.LeagueID;
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            while (myReader.Read())
            {
                count++;
            }
            myReader.Close();
            return count > 0 ? true : false;
        }

        public int GetLeagueID(SqlConnection dbconAPP, string leagueName, int userID)
        {
            int leagueID = 0;
            string strsql = "SELECT * FROM tblLeague WHERE CommissionerUserID = " + userID + 
                " AND LeagueName = '" + leagueName + "' AND RetireDate IS NULL";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            while (myReader.Read())
            {
                leagueID = Convert.ToInt32(myReader["LeagueID"].ToString());
                count++;
            }
            myReader.Close();
            return count == 1 ? leagueID : 0;
        }
        
        public League GetLeagueInfo(SqlConnection dbconAPP, int leagueID, int seasonID)
        {
            League model = new League();
            string strsql = "SELECT a.LeagueID, a.LeagueName, a.CommissionerUserID, a.isPublic, a.Password, b.SeasonID, " +
                "b.MaxTeams, b.DraftTimePerTeam, b.RosterSize, b.DraftOrder, b.DraftDate, b.StartDate, " +
                "b.EndDate, b.CreateByUserID, b.CreateDate, b.RetireByUserID, b.RetireDate " +
                "FROM tblLeague as a INNER JOIN tblSeason as b ON a.LeagueID = b.LeagueID " +
                "WHERE a.LeagueID = " + leagueID + " AND b.SeasonID = " + seasonID +" ";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader dr;
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                model.LeagueID = Convert.ToInt32(dr["LeagueID"].ToString());
                model.LeagueName = dr["LeagueName"].ToString();
                model.CommissionerUserID = Convert.ToInt32(dr["CommissionerUserID"].ToString());
                model.isPublic = Convert.ToBoolean(dr["isPublic"].ToString());
                model.Password = dr["Password"].ToString();
                model.SeasonID = Convert.ToInt32(dr["SeasonID"].ToString());
                model.MaxTeams = Convert.ToInt32(dr["MaxTeams"].ToString());
                model.DraftTimePerTeam = Convert.ToInt32(dr["DraftTimePerTeam"].ToString());
                model.RosterSize = Convert.ToInt32(dr["RosterSize"].ToString());
                model.Teams = dr["DraftOrder"].ToString().Split('|').Count();
                model.DraftDate = Convert.ToDateTime(dr["DraftDate"].ToString());
                model.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                model.EndDate = Convert.ToDateTime(dr["EndDate"].ToString());
                model.CreateByUserID = Convert.ToInt32(dr["CreateByUserID"].ToString());
                model.CreateDate = Convert.ToDateTime(dr["CreateDate"].ToString());
                model.RetireByUserID = dr["RetireByUserID"].ToString() == "" ? default(Nullable<int>) : Convert.ToInt32(dr["RetireByUserID"].ToString());
                model.RetireDate = dr["RetireDate"].ToString() == "" ? default(Nullable<DateTime>) : Convert.ToDateTime(dr["RetireDate"].ToString());
            }
            dr.Close();
            return model;
        }

        public void EditLeagueInfo(SqlConnection dbconAPP, League model)
        {
            //Update tblLeague
            string sqlLeague = "UPDATE tblLeague " +
                    "SET LeagueName = '" + model.LeagueName + "', Password = '" + model.Password + "' " +
                    "WHERE LeagueID = " + model.LeagueID;
            var cmdLeague = new SqlCommand(sqlLeague, dbconAPP);
            cmdLeague.ExecuteScalar();

            //Update tblSeason
            string sqlSeason = "UPDATE tblSeason SET MaxTeams = " + model.MaxTeams + ", " +
                "DraftTimePerTeam = " + model.DraftTimePerTeam + ", RosterSize = " + model.RosterSize + ", " +
                "DraftDate = '" + model.DraftDate + "', StartDate = '" + model.StartDate +"', " + 
                "EndDate = '" + model.EndDate + "' WHERE LeagueID = " + model.LeagueID + " AND SeasonID = " + model.SeasonID;
            var cmdSeason = new SqlCommand(sqlSeason, dbconAPP);
            cmdSeason.ExecuteScalar();
        }

        public void CreateSeason(SqlConnection dbconAPP, League model)
        {
            using (SqlCommand cmd = new SqlCommand("wboiSeason", dbconAPP))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LeagueID", model.LeagueID);
                cmd.Parameters.AddWithValue("@MaxTeams", model.MaxTeams);
                cmd.Parameters.AddWithValue("@DraftTimePerTeam", model.DraftTimePerTeam);
                cmd.Parameters.AddWithValue("@RosterSize", model.RosterSize);
                cmd.Parameters.AddWithValue("@DraftDate", model.DraftDate);
                cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
                cmd.Parameters.AddWithValue("@CreateByUserID", model.CreateByUserID);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                cmd.ExecuteScalar();
            }
        }
        #endregion
    }
}