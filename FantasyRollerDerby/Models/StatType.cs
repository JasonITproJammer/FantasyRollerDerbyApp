using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FantasyRollerDerby.Models
{
    public class StatType
    {
        #region VARS
        public int StatTypeID { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Name can only be 50 characters long")]
        public string Name { get; set; }
        [Required]
        [MaxLength(5, ErrorMessage = "Code can only be 5 characters long")]
        public string NameCode { get; set; }
        public string Description { get; set; }
        public int CreateByUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int RetireByUserID { get; set; }
        public DateTime RetireDate { get; set; }
        public bool DefaultStat { get; set; }
        public double DefaultValue { get; set; }
        public int StatTypeValueXrefID { get; set; }
        #endregion

        #region CLASSES
        public StatTypeValue cStatTypeValue = new StatTypeValue();
        public LeagueStatTypeValue cLeagueStatTypeValue = new LeagueStatTypeValue();
        #endregion

        #region LISTS
        List<StatTypeValue> list_StatTypeValue = new List<StatTypeValue>();
        List<LeagueStatTypeValue> list_LeagueStatTypeValue = new List<LeagueStatTypeValue>();
        #endregion

        #region METHODS
        public int GetStatTypeID(SqlConnection dbconAPP, StatType model)
        {
            int statTypeID = 0;
            string strsql = "SELECT * FROM tblStatType WHERE Name = '" + model.Name 
                + "' AND NameCode = '" + model.NameCode + "' AND RetireDate IS NULL";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            int count = 0;
            while (myReader.Read())
            {
                statTypeID = Convert.ToInt32(myReader["StatTypeID"].ToString());
                count++;
            }
            myReader.Close();
            return count == 1 ? statTypeID : 0;
        }

        public List<StatType> GetStatTypeDefaultList(SqlConnection dbconAPP)
        {
            List<StatType> tempList = new List<StatType>();
            string strsql = "SELECT * FROM tblStatType " +
                "WHERE DefaultStat = 1 AND RetireDate IS NULL";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            while (myReader.Read())
            {
                StatType tempModel = new StatType();
                tempModel.StatTypeID = Convert.ToInt32(myReader["StatTypeID"].ToString());
                tempModel.Name = myReader["Name"].ToString();
                tempModel.NameCode = myReader["NameCode"].ToString();
                tempModel.Description = myReader["Description"].ToString();
                tempModel.DefaultStat = Convert.ToBoolean(myReader["DefaultStat"].ToString());
                tempModel.DefaultValue = Convert.ToDouble(myReader["DefaultValue"].ToString());
                tempModel.CreateByUserID = Convert.ToInt32(myReader["CreateByUserID"].ToString());
                tempModel.CreateDate = Convert.ToDateTime(myReader["CreateDate"].ToString());
                tempList.Add(tempModel);
            }
            myReader.Close();
            return tempList;
        }

        public List<StatType> GetStatTypeLeagueList(SqlConnection dbconAPP, int leagueID)
        {
            List<StatType> tempList = new List<StatType>();
            string strsql = "SELECT DISTINCT a.StatTypeValueXrefID, a.StatTypeID, a.LeagueID, b.Name, b.NameCode, b.Description " +
                "FROM tblLeagueStatTypeValueXREF as a LEFT JOIN tblStatType as b ON a.StatTypeID = b.StatTypeID " +
                "WHERE a.LeagueID = " + leagueID + " ";
            var cmd = new SqlCommand(strsql, dbconAPP);
            SqlDataReader myReader;
            myReader = cmd.ExecuteReader();
            while (myReader.Read())
            {
                StatType tempModel = new StatType();
                tempModel.StatTypeID = Convert.ToInt32(myReader["StatTypeID"].ToString());
                tempModel.StatTypeValueXrefID = Convert.ToInt32(myReader["StatTypeValueXrefID"].ToString());
                tempModel.Name = myReader["Name"].ToString();
                tempModel.NameCode = myReader["NameCode"].ToString();
                tempModel.Description = myReader["Description"].ToString();
                tempList.Add(tempModel);
            }
            myReader.Close();
            return tempList;
        }
        #endregion
    }

    public class StatTypeValue
    {
        #region VARS
        public int StatTypeID { get; set; }
        public double StatValue { get; set; }
        public int CreateByUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int RetireByUserID { get; set; }
        public DateTime RetireDate { get; set; }
        #endregion
    }

    public class LeagueStatTypeValue
    {
        #region VARS
        public int StatTypeID { get; set; }
        public int LeagueID { get; set; }
        public double StatValue { get; set; }
        public int CreateByUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int RetireByUserID { get; set; }
        public DateTime RetireDate { get; set; }
        #endregion
    }
}