using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;

namespace FantasyRollerDerby.Models
{
    public class Skater
    {
        public int SkaterID { get; set; }
        public int DerbyTeamID { get; set; }
        public string DerbyTeamName { get; set; }
        public string DerbyTeamNameCode { get; set; }
        public string DerbyTeamLogo { get; set; }
        public string DerbyName { get; set; }
        public int DerbyNumber { get; set; }
        public string Position { get; set; }
        public string Photo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DraftPickNumber { get; set; }
        public int GameStatID { get; set; }
        public double GameStatValue { get; set; }
        public string StatTypeName { get; set; }
        public string StatTypeNameCode { get; set; }
        public double StatTypeValue { get; set; }
        public double TotalSkaterGameStatValue { get; set; }
        public double TotalSkaterValue { get; set; }
        public int StatTypeID { get; set; }
        public DateTime GameDate { get; set; }
        public int FantasyTeamID { get; set; }
        public int FantasyTeamRosterID { get; set; }
        public int SeasonID { get; set; }
        public int LeagueID { get; set; }

        public List<Skater> GetSkaterList(SqlConnection dbconAPP, int seasonID, int isDrafted = 0)
        {
            List<Skater> tempList = new List<Skater>();
            using (SqlCommand cmd = new SqlCommand("wbogSkaterList", dbconAPP))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeasonID", seasonID);
                cmd.Parameters.AddWithValue("@isDrafted", isDrafted);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Skater tempModel = new Skater();
                            tempModel.SkaterID = Convert.ToInt32(dr["SkaterID"].ToString());
                            tempModel.DerbyTeamID = Convert.ToInt32(dr["DerbyTeamID"].ToString());
                            tempModel.DerbyName = dr["DerbyName"].ToString();
                            tempModel.DerbyNumber = Convert.ToInt32(dr["DerbyNumber"].ToString());
                            tempModel.Position = dr["Position"].ToString();
                            tempModel.Photo = dr["Photo"].ToString();
                            tempModel.FirstName = dr["FirstName"].ToString();
                            tempModel.LastName = dr["LastName"].ToString();
                            tempList.Add(tempModel);
                        }
                    }
                }
            }
            return tempList;
        }

        public int GetNextRankedSkaterID(SqlConnection dbconAPP, int seasonID)
        {
            int skaterID = 0;
            string strsql = "SELECT TOP (1) SkaterID FROM tblSkater WHERE SkaterID NOT IN " +
                "(SELECT SkaterID FROM tblFantasyTeamRoster WHERE SeasonID = '" + seasonID + "') " +
                "ORDER BY Ranking";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            while (myReader.Read())
            {
                skaterID = Convert.ToInt32(myReader["SkaterID"].ToString());
                count++;
            }
            myReader.Close();
            return count == 1 ? skaterID : 0;
        }

        public List<Skater> GetFantasyTeamSkaterList(SqlConnection dbconAPP, int seasonID, int fantasyTeamID)
        {
            List<Skater> tempList = new List<Skater>();
            using (SqlCommand cmd = new SqlCommand("wbogFantasyTeamSkaterList", dbconAPP))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeasonID", seasonID);
                cmd.Parameters.AddWithValue("@FantasyTeamID", fantasyTeamID);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Skater tempModel = new Skater();
                            tempModel.DerbyTeamID = Convert.ToInt32(dr["DerbyTeamID"].ToString());
                            tempModel.DerbyTeamName = dr["DerbyTeamName"].ToString();
                            tempModel.DerbyTeamNameCode = dr["DerbyTeamNameCode"].ToString();
                            tempModel.DerbyTeamLogo = dr["DerbyTeamLogo"].ToString();
                            tempModel.SkaterID = Convert.ToInt32(dr["SkaterID"].ToString());
                            tempModel.DerbyName = dr["SkaterDerbyName"].ToString();
                            tempModel.DerbyNumber = Convert.ToInt32(dr["SkaterDerbyNumber"].ToString());
                            tempModel.Photo = dr["SkaterDerbyPhoto"].ToString();
                            tempModel.DraftPickNumber = Convert.ToInt32(dr["DraftPickNumber"].ToString());
                            tempList.Add(tempModel);
                        }
                    }
                }
            }
            return tempList;
        }

        public List<Skater> GetSkaterGameStats(SqlConnection dbconAPP, int seasonID, int leagueID, 
            string startDate, string endDate)
        {
            List<Skater> returnList = new List<Skater>();
            using (SqlCommand cmd = new SqlCommand("wbogSkaterGameStats", dbconAPP))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeasonID", seasonID);
                cmd.Parameters.AddWithValue("@LeagueID", leagueID);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Skater tempModel = new Skater();
                            tempModel.GameStatID = Convert.ToInt32(dr["GameStatID"].ToString());
                            tempModel.SkaterID = Convert.ToInt32(dr["SkaterID"].ToString());
                            tempModel.DerbyName = dr["DerbyName"].ToString();
                            tempModel.Photo = dr["Photo"].ToString();
                            tempModel.GameStatValue = Convert.ToDouble(dr["GameStatValue"].ToString());
                            tempModel.StatTypeValue = Convert.ToDouble(dr["StatTypeValue"].ToString());
                            tempModel.TotalSkaterGameStatValue = Convert.ToDouble(dr["TotalSkaterGameStatValue"].ToString());
                            tempModel.StatTypeID = Convert.ToInt32(dr["StatTypeID"].ToString());
                            tempModel.StatTypeName = dr["Name"].ToString();
                            tempModel.StatTypeNameCode = dr["NameCode"].ToString();
                            tempModel.GameDate = Convert.ToDateTime(dr["GameDate"].ToString());
                            tempModel.FantasyTeamID = Convert.ToInt32(dr["FantasyTeamID"].ToString());
                            tempModel.FantasyTeamRosterID = Convert.ToInt32(dr["FantasyTeamRosterID"].ToString());
                            returnList.Add(tempModel);
                        }
                    }
                }
            }
            return returnList;
        }
    }
}