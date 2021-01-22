using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FantasyRollerDerby.Models
{
    public class GameStat
    {
        #region VARS
        public int GameStatID { get; set; }
        public int SkaterID { get; set; }
        public int StatTypeID { get; set; }
        public double GameStatValue { get; set; }
        public DateTime GameDate { get; set; }
        public int CreateByUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int RetireByUserID { get; set; }
        public DateTime RetireDate { get; set; }
        #endregion

        #region METHODS
        public void AddGameStat(SqlConnection dbconAPP, int skaterID, int statID, double value, DateTime gameDate, int userID)
        {
            //Insert Game Stat
            string sql = "INSERT INTO tblGameStat (SkaterID, StatTypeID, GameStatValue, GameDate, CreateByUserID, CreateDate) " +
                    "VALUES (" + skaterID + ", " + statID + ", " + value + ", '" + gameDate.ToString("MM/dd/yyyy") + "', " + userID + ", '" + DateTime.Now + "')";
            var cmd = new SqlCommand(sql, dbconAPP);
            cmd.ExecuteScalar();
        }
        #endregion
    }
}