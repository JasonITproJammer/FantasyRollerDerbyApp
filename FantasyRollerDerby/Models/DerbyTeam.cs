using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyRollerDerby.Models
{
    public class DerbyTeam
    {
        public int DerbyTeamID { get; set; }
        public string Name { get; set; }
        public string NameCode { get; set; }
        public string Logo { get; set; }

        public List<DerbyTeam> GetDerbyTeamList()
        {
            List<DerbyTeam> tempList = new List<DerbyTeam>();
            DerbyTeam tempModel = new DerbyTeam();

            tempModel.DerbyTeamID = 1;
            tempModel.Name = "Saint Louis Gatekeepers";
            tempModel.NameCode = "SLGK";
            tempModel.Logo = "degault.jpg";
            tempList.Add(tempModel);
            tempModel = new DerbyTeam();

            tempModel.DerbyTeamID = 2;
            tempModel.Name = "Southern Discomfort";
            tempModel.NameCode = "SOCO";
            tempModel.Logo = "degault.jpg";
            tempList.Add(tempModel);
            tempModel = new DerbyTeam();

            tempModel.DerbyTeamID = 3;
            tempModel.Name = "Bridgetown Menace";
            tempModel.NameCode = "BTRD";
            tempModel.Logo = "degault.jpg";
            tempList.Add(tempModel);
            tempModel = new DerbyTeam();

            tempModel.DerbyTeamID = 4;
            tempModel.Name = "Texas Mens Roller Derby";
            tempModel.NameCode = "TXMD";
            tempModel.Logo = "degault.jpg";
            tempList.Add(tempModel);
            tempModel = new DerbyTeam();

            tempModel.DerbyTeamID = 5;
            tempModel.Name = "New York Shock Exchange";
            tempModel.NameCode = "NYSE";
            tempModel.Logo = "degault.jpg";
            tempList.Add(tempModel);
            tempModel = new DerbyTeam();

            tempModel.DerbyTeamID = 16;
            tempModel.Name = "Wisconsin Mens Roller Derby";
            tempModel.NameCode = "WMRD";
            tempModel.Logo = "degault.jpg";
            tempList.Add(tempModel);
            tempModel = new DerbyTeam();

            return tempList;
        }
    }
}