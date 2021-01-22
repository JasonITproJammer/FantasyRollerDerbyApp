using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FantasyRollerDerby.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.IO;

namespace FantasyRollerDerby.Controllers
{
    public class LeagueController : Controller
    {
        #region dbKahn
        //SQL Connection
        SqlConnection dbconAPP;
        public SqlConnection GetConnection(string conType = "WebApp")
        {
            switch (conType.ToUpper())
            {
                case "WEBAPP":
                    return new SqlConnection(ConfigurationManager.ConnectionStrings["JND_FantasyRollerDerby"].ConnectionString.ToString());
                default:
                    return new SqlConnection(ConfigurationManager.ConnectionStrings["JND_FantasyRollerDerby"].ConnectionString.ToString());
            }
        }
        #endregion

        #region GETS
        [Authorize]
        public ActionResult Index(string leagueType, string leagueID, int seasonID, int fantasyTeamID, string errMsg = "")
        {
            _League model = new _League();
            model.cLeague = new League();
            model.list_cLeague = new List<League>();
            model.cSeason = new Season();
            model.list_cFantasyTeam = new List<FantasyTeam>();
            model.cFantasyTeam = new FantasyTeam();
            model.list_LeagueStanding = new List<FantasyTeam>();
            model.cSkater = new Skater();
            model.list_Skater = new List<Skater>();

            try
            {
                //setup errMsg
                if(errMsg != "")
                {
                    ViewBag.LeagueError = errMsg;
                }

                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Get user info
                model.cUser = new User();
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.cUser.AspNetUserID = user.Id;
                model.cUser.UserID = model.cUser.GetUserID(dbconAPP, model.cUser.AspNetUserID);
                model.cUser.FullName = model.cUser.GetUserName(dbconAPP, model.cUser.UserID);
                if(model.cUser.FullName == "Jason Deutsch")
                {
                    ViewBag.Creator = true;
                }

                //Get League info
                model.cLeague = model.cLeague.GetLeagueInfo(dbconAPP, Convert.ToInt32(leagueID), seasonID);
                model.cFantasyTeam = new FantasyTeam();
                model.cFantasyTeam.Name = model.cLeague.TeamName;
                model.cFantasyTeam.FantasyTeamID = fantasyTeamID;

                //Enable or disable the commish edit league button
                if(model.cLeague.CommissionerUserID == model.cUser.UserID)
                {
                    ViewBag.IsCommish = true;
                }

                ////Enable or disable the draft room button based on when the draft time is
                //int beforeDraft = (30 * 60) * -1; //allowed into the draft 30 minutes before
                //int afterDraft = (model.cLeague.DraftTimePerTeam * model.cLeague.RosterSize * model.cLeague.MaxTeams) + (15 * 60); //allowed into draft 15 minutes after the end of the draft
                //var test1 = model.cLeague.DraftDate.ToFileTimeUtc();
                //var test2 = model.cLeague.DraftDate.ToBinary();
                //var test3 = model.cLeague.DraftDate.ToUniversalTime();
                //var test4 = model.cLeague.DraftDate.ToFileTime();
                //var testNow = DateTime.Now.ToBinary();
                //var testNowUTC = DateTime.UtcNow;
                //var testNowUtcBinary = DateTime.UtcNow.ToBinary();
                //var testNowFileTimeUTC = DateTime.Now.ToFileTimeUtc();
                //var testNowFileTime = DateTime.Now.ToFileTime();

                //var inactiveDraftTime = model.cLeague.DraftDate.AddSeconds(afterDraft).ToFileTime();
                //var activeDraftTime = model.cLeague.DraftDate.AddSeconds(beforeDraft).ToFileTime();
                //var currentTime = DateTime.Now.ToFileTime();
                ////Having trouble getting this date comparison to work on Azure web hosting solutions
                ////double intInactiveDate = Convert.ToDouble(inactiveDraftTime.ToString("yyyyMMddhhmmss"));
                ////double intActiveDate = Convert.ToDouble(activeDraftTime.ToString("yyyyMMddhhmmss"));
                ////double intDateTimeNow = Convert.ToDouble(DateTime.Now.ToString("yyyyMMddhhmmss"));
                ////Azure web hosting service compares to the tick so this won't work 
                //if (inactiveDraftTime > currentTime && activeDraftTime < currentTime)
                //{
                //    //Enable the draft button
                //    ViewBag.IsDraft = true;
                //}
                ////ViewBag.IsDraft = true;

                //get list of teams first by total team points, then by draft pick number
                model.cSeason = new Season();
                model.list_cFantasyTeam = new List<FantasyTeam>();
                model.list_cFantasyTeam = model.cFantasyTeam.GetFantasyTeamStandings(dbconAPP, model.cLeague.SeasonID, model.cLeague.LeagueID);

                //if list is empty then sort teams by draft order
                model.list_SeasonFantasyTeams = model.cFantasyTeam.GetSeasonFantasyTeamList(dbconAPP, model.cLeague.SeasonID);
                if (model.list_cFantasyTeam.Count < 1)
                {
                    //Get season draft order
                    model.cSeason.SeasonID = model.cLeague.SeasonID;
                    model.cSeason.GetSeasonInfo(dbconAPP, model.cSeason);

                    if(model.cSeason.DraftOrder != "")
                    {
                        //Order teams according to the draft order
                        foreach (var item in model.cSeason.DraftOrder.Split('|'))
                        {
                            FantasyTeam tempFT = new FantasyTeam();
                            tempFT = model.list_SeasonFantasyTeams.Find(x => x.FantasyTeamID == Convert.ToInt32(item));
                            model.list_cFantasyTeam.Add(tempFT);
                        }
                    }
                    else
                    {
                        model.list_cFantasyTeam = model.list_SeasonFantasyTeams;
                    }
                }

                //Create standings list
                //First, verify that all teams are included in the standings list
                model.list_LeagueStanding = new List<FantasyTeam>();
                if (model.list_cFantasyTeam.Count != model.list_SeasonFantasyTeams.Count)
                {
                    model.list_LeagueStanding = model.list_cFantasyTeam.OrderByDescending(x => x.TotalTeamPoints).ToList();
                    foreach (var item in model.list_SeasonFantasyTeams)
                    {
                        var test = model.list_LeagueStanding.Find(m => m.FantasyTeamID == item.FantasyTeamID);
                        if (test == null)
                        {
                            item.TotalTeamPoints = 0.00;
                            model.list_LeagueStanding.Add(item);
                        }
                    }
                }
                else
                {
                    model.list_LeagueStanding = model.list_cFantasyTeam.OrderByDescending(x => x.TotalTeamPoints).ToList();
                }

                //set fantasy team id and get fantasy team info
                model.cFantasyTeam.FantasyTeamID = fantasyTeamID;
                model.cFantasyTeam.GetFantasyTeamInfo(dbconAPP, model.cFantasyTeam);

                //Get Fantasy Team skater list
                model.cSkater = new Skater();
                model.list_Skater = new List<Skater>();
                model.list_Skater = model.cSkater.GetFantasyTeamSkaterList(dbconAPP, seasonID, fantasyTeamID);

                //Determine if the season is over
                if(DateTime.Now > model.cLeague.EndDate)
                {
                    ViewBag.SeasonEnded = true;
                    //need to change so users can't select a start date before the draft date, etc.
                    ViewBag.MinDate = DateTime.Now.ToString("yyyy-MM-dd");
                    ViewBag.MaxDate = DateTime.Now.AddYears(10).ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                ViewBag.LeagueError += "ERROR: " + ex.Message + " (League.Index[GET]).";
                return View(model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            //string fantasyTeamName, string leagueName, int seasonID, int teamID
            Session["fantasyTeamName"] = model.cFantasyTeam.Name;
            Session["leagueName"] = model.cLeague.LeagueName;
            Session["seasonID"] = model.cLeague.SeasonID;
            Session["teamID"] = model.cFantasyTeam.FantasyTeamID;
            return View(model);
        }

        // GET: League/Create
        [Authorize]
        public ActionResult CreateLeague(string leagueType, string errMsg)
        {
            _League model = new _League();
            try
            {
                //Set errMsg
                ViewBag.LeagueError = errMsg;

                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Get user info
                model.cUser = new User();
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext
                    .Current.User.Identity.GetUserId());
                model.cUser.AspNetUserID = user.Id;
                model.cUser.UserID = model.cUser.GetUserID(dbconAPP, model.cUser.AspNetUserID);
                model.cUser.FullName = model.cUser.GetUserName(dbconAPP, model.cUser.UserID);

                //Get default stat type list
                model.cStatType = new StatType();
                model.list_DefaultStats = new List<StatType>();
                model.list_DefaultStats = model.cStatType.GetStatTypeDefaultList(dbconAPP);

                //Determine if the league is public or private
                ViewBag.LeaguePrivate = "false";
                if (leagueType.ToLower() == "public")
                {
                    ViewBag.LeaguePrivate = "false";
                }
                else if (leagueType.ToLower() == "private")
                {
                    ViewBag.LeaguePrivate = "true";
                }
            }
            catch (Exception ex)
            {
                ViewBag.LeagueError += "ERROR: " + ex.Message + " (League.CreateLeague[GET]).";
                return View(model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return View(model);
        }

        [Authorize]
        public ActionResult EditLeague(int leagueID, int seasonID, int userID, int teamID)
        {
            League model = new League();
            User user = new User();
            try
            {
                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //get league info
                model = model.GetLeagueInfo(dbconAPP, leagueID, seasonID);
                model.FantasyTeamID = teamID;

                //Disable editing league settings if past season end date
                if (DateTime.Now > model.EndDate)
                {
                    ViewBag.SeasonEnded = true;
                }

                //Get user info
                string userName = "";
                userName = user.GetUserName(dbconAPP, userID);
                //Keep editing league settings enabled always for Jason Deutsch
                if(userName.Contains("Jason Deutsch"))
                {
                    ViewBag.IsDeutsch = true;
                }
                
                //Do not build the Password element if league is public
                ViewBag.IsPrivate = false;
                if (!model.isPublic)
                {
                    ViewBag.IsPrivate = true;
                }
            }
            catch (Exception ex)
            {
                ViewBag.LeagueError += "ERROR: " + ex.Message + " (League.EditLeague[GET]).";
                return View(model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return View(model);
        }

        [Authorize]
        public ActionResult EditStats(int leagueID, int seasonID, int userID, int teamID)
        {
            _League model = new _League();
            try
            {
                //Initialize sub classes and lists
                model.cUser = new User();
                model.cLeague = new League();
                model.cSkater = new Skater();
                model.cStatType = new StatType();
                model.cFantasyTeam = new FantasyTeam();
                model.list_cStatType = new List<StatType>();
                model.list_Skater = new List<Skater>();
                model.list_SkaterGameStats = new List<Skater>();

                //set values
                model.cLeague.LeagueID = leagueID;
                model.cLeague.SeasonID = seasonID;
                model.cUser.UserID = userID;

                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Get league info
                model.cLeague = model.cLeague.GetLeagueInfo(dbconAPP, leagueID, seasonID);

                //Get stat type list specific to the league
                model.list_cStatType = model.cStatType.GetStatTypeLeagueList(dbconAPP, leagueID);

                //Get all skaters drafted as a list for the drop down lists
                model.list_Skater = model.cSkater.GetSkaterList(dbconAPP, seasonID, 1);

                //Get skaters and stats as list
                model.list_SkaterGameStats = model.cSkater.GetSkaterGameStats(dbconAPP, seasonID, leagueID,
                    model.cLeague.StartDate.ToString("MM/dd/yyyy"), model.cLeague.EndDate.ToString("MM/dd/yyyy"));

                //Set TeamID
                model.cLeague.FantasyTeamID = teamID;
                model.cFantasyTeam.FantasyTeamID = teamID;

                //Disable Adding Skater stats if past season end date
                if(DateTime.Now > model.cLeague.EndDate)
                {
                    ViewBag.SeasonEnded = true;
                }

                //Set Min and Max dates
                ViewBag.MinDate = model.cLeague.StartDate.ToString("yyyy-MM-dd");
                ViewBag.MaxDate = model.cLeague.EndDate.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                ViewBag.LeagueError += "ERROR: " + ex.Message + " (League.EditStats[GET]).";
                return View(model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return View(model);
        }

        [Authorize]
        public ActionResult NewSeason(int leagueID, int seasonID, int userID, int teamID)
        {
            League model = new League();
            string leagueType = "";
            try
            {
                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Get league info to create season
                model = model.GetLeagueInfo(dbconAPP, leagueID, seasonID);
                model.FantasyTeamID = teamID;

                //Create new season using existing league info
                model.CreateSeason(dbconAPP, model);

                //Get league info for the new season
                model = model.GetLeagueInfo(dbconAPP, leagueID, seasonID);
                model.FantasyTeamID = teamID;
                if (model.isPublic)
                    leagueType = "public";
                else
                    leagueType = "private";

                //Disable editing league settings if past season end date
                if (DateTime.Now > model.EndDate)
                {
                    ViewBag.SeasonEnded = true;
                }
            }
            catch (Exception ex)
            {
                ViewBag.LeagueError += "ERROR: " + ex.Message + " (League.NewSeason[GET]).";
                return View(model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return RedirectToAction("Index", new { leagueType = leagueType, leagueID = model.LeagueID, seasonID = model.SeasonID, fantasyTeamID = model.FantasyTeamID });
        }
        #endregion

        #region POSTS
        // POST: League/Create
        [HttpPost]
        public ActionResult Create(_League model, FormCollection fc)
        {
            Return cR = new Return();
            FantasyTeam modelFantasyTeam = new FantasyTeam();
            string errMsg = "";
            string leagueType = "public";
            string errType = "League";
            int rowNum = 0;
            try
            {
                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Get league public or private type
                ViewBag.LeaguePrivate = "false";
                var test = fc["isPrivate"].ToString();
                if (fc["isPrivate"].ToString() == "false")
                {
                    ViewBag.LeaguePrivate = "false";
                    model.cLeague.isPublic = true;
                    leagueType = "public";
                }
                else
                {
                    ViewBag.LeaguePrivate = "true";
                    model.cLeague.isPublic = false;
                    leagueType = "private";
                }

                //Assign password
                model.cLeague.Password = fc["lPwd"].ToString();

                //Get all league stat types and their values
                //need to think of a solution for when a user enters
                //a comma within the input value
                List<string> listStatName = new List<string>();
                listStatName = fc["StatTypeName"].Split(',').ToList();
                List<string> listStatCode = new List<string>();
                listStatCode = fc["StatTypeCode"].Split(',').ToList();
                List<string> listStatID = new List<string>();
                listStatID = fc["StatTypeID"].Split(',').ToList();
                List<string> listStatDesc = new List<string>();
                listStatDesc = fc["StatTypeDesc"].Split(',').ToList();
                List<string> listStatValue = new List<string>();
                listStatValue = fc["StatTypeValue"].Split(',').ToList();
                rowNum = listStatID.Count;
                model.list_cStatType = new List<StatType>();
                for (int i = 0; i < rowNum; i++)
                {
                    StatType temp = new StatType();
                    temp.cStatTypeValue = new StatTypeValue();
                    temp.StatTypeID = Convert.ToInt32(listStatID[i]);
                    temp.Name = listStatName[i];
                    temp.NameCode = listStatCode[i];
                    temp.Description = listStatDesc[i];
                    temp.cStatTypeValue.StatValue = Convert.ToDouble(listStatValue[i]);
                    model.list_cStatType.Add(temp);
                }

                //Create the league
                cR = model.CreateNewLeague(dbconAPP, model);
                if (cR.ReturnFlag == false)
                {
                    errMsg += cR.ReturnMsg;
                    return RedirectToAction("CreateLeague", new { leagueType = leagueType, errMsg = errMsg });
                }

                //Get Fantasy Team info
                errType = "FantasyTeam";
                modelFantasyTeam.UserID = model.cUser.UserID;
                modelFantasyTeam.LeagueID = model.cLeague.LeagueID;
                modelFantasyTeam.LeagueName = model.cLeague.LeagueName;
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message + " (League.Create[POST]).");
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }

            //if (model.cLeague.isPublic)
            //{
            //    //return RedirectToAction("JoinPublicLeague", "Home", new { leagueID = model.cLeague.LeagueID, userID = model.cUser.UserID });
            //    ViewBag.LeagueType = "public";
            //}
            //else
            //{
            //    //return RedirectToAction("JoinPrivateLeague", "Home", new { leagueID = model.cLeague.LeagueID, userID = model.cUser.UserID });
            //    ViewBag.LeagueType = "private";
            //}
            return PartialView("_JoinLeague", modelFantasyTeam);
        }

        [HttpPost]
        public ActionResult CreateTeam(FormCollection fc, HttpPostedFileBase FantasyTeamLogo)
        {
            try
            {
                //Initialize classes
                FantasyTeam model = new FantasyTeam();

                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //save logo
                if (FantasyTeamLogo != null && FantasyTeamLogo.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(FantasyTeamLogo.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/TeamLogos"), fileName);
                    FantasyTeamLogo.SaveAs(path);
                    model.Logo = fileName;
                }
                else
                {
                    model.Logo = "defaultTeamLogo.png";
                }

                //Add fantasy team
                model.UserID = Convert.ToInt32(fc["FantasyTeamUserID"].ToString());
                model.LeagueID = Convert.ToInt32(fc["FantasyTeamLeagueID"].ToString());
                model.Name = fc["FantasyTeamName"].ToString();
                model.Slogan = fc["FantasyTeamSlogan"].ToString();
                model.CreateNewFantasyTeam(dbconAPP, model);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message + " (League.CreateTeam[POST]).");
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult UpdateDraftDateTime(string draftDateTime, string seasonID)
        {
            try
            {
                //Update draft date time
                dbconAPP = GetConnection();
                dbconAPP.Open();
                string strsql = "UPDATE tblSeason " +
                    "SET DraftDate = '" + draftDateTime + "' WHERE SeasonID = " + seasonID;
                var cmd = new SqlCommand(strsql, dbconAPP);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message + " (League.UpdateDraftDateTime[POST]).");
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return Content("League.UpdateDraftDateTime[POST] Success!");
        }

        [HttpPost]
        public ActionResult UpdateDraftTimePerTeam(string draftTimePerTeam, string seasonID)
        {
            try
            {
                //Update DraftTimePerTeam
                dbconAPP = GetConnection();
                dbconAPP.Open();
                string strsql = "UPDATE tblSeason " +
                    "SET DraftTimePerTeam = '" + draftTimePerTeam + "' WHERE SeasonID = " + seasonID;
                var cmd = new SqlCommand(strsql, dbconAPP);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message + " (League.UpdateDraftTimePerTeam[POST]).");
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return Content("League.UpdateDraftTimePerTeam[POST] Success!");
        }

        [HttpPost]
        public ActionResult ResetSeasonRosters(string seasonID)
        {
            try
            {
                //Delete all records from  tblFantasyTeamRoster 
                //Where the SeasonID matches the method argument
                dbconAPP = GetConnection();
                dbconAPP.Open();
                string strsql = "DELETE FROM tblFantasyTeamRoster " +
                    " WHERE SeasonID = " + seasonID;
                var cmd = new SqlCommand(strsql, dbconAPP);
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message + " (League.ResetSeasonRosters[POST]).");
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return Content("League.ResetSeasonRosters[POST] Success!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditLeague(League model)
        {
            try
            {
                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //set team ID
                int teamID = model.FantasyTeamID;

                //Edit League info
                model.EditLeagueInfo(dbconAPP, model);

                //get league info
                model = model.GetLeagueInfo(dbconAPP, model.LeagueID, model.SeasonID);
                model.FantasyTeamID = teamID;
            }
            catch (Exception ex)
            {
                ViewBag.LeagueError += "ERROR: " + ex.Message + " (League.EditLeague[GET]).";
                return View(model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditStats(_League model, FormCollection fc)
        {
            try
            {
                //Initialize classes
                model.cGameStat = new GameStat();

                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Assign values from form collection
                var skaterIDs = fc["cSkater.SkaterID"].Split(',').ToList();
                var statTypeIDs = fc["cStatType.StatTypeID"].Split(',').ToList();
                var gameDates = fc["GameDate"].Split(',').ToList();
                var statValues = fc["StatValue"].Split(',').ToList();

                //Add skater stats
                for(int i = 0; i < skaterIDs.Count(); i++ )
                {
                    model.cGameStat.AddGameStat(dbconAPP, Convert.ToInt32(skaterIDs[i]), Convert.ToInt32(statTypeIDs[i]), Convert.ToDouble(statValues[i]), Convert.ToDateTime(gameDates[i]), model.cUser.UserID);
                }

                //get league info
                //model.cLeague = model.cLeague.GetLeagueInfo(dbconAPP, model.cLeague.LeagueID, model.cLeague.SeasonID);
            }
            catch (Exception ex)
            {
                ViewBag.LeagueError += "ERROR: " + ex.Message + " (League.EditStats[POST]).";
                return View(model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return RedirectToAction("EditStats", new { leagueID = model.cLeague.LeagueID, seasonID = model.cLeague.SeasonID, userID = model.cUser.UserID, teamID = model.cFantasyTeam.FantasyTeamID });
        }

        [HttpPost]
        public ActionResult SaveTeamInfo(string teamName, string teamSlogan, int teamID, HttpPostedFileBase TeamImage)
        {
            string saveFile = "";
            try
            {
                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Save logo file
                if (TeamImage != null && TeamImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(TeamImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/TeamLogos"), fileName);
                    TeamImage.SaveAs(path);
                    saveFile = fileName;
                }

                //Save fantasy team info
                FantasyTeam model = new FantasyTeam();
                model.UpdateFantasyTeamInfo(dbconAPP, teamID, teamName, teamSlogan, saveFile);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message + " (League.SaveTeamInfo[POST]).");
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return Content("/Images/TeamLogos/" + saveFile);
        }

        [HttpPost]
        [Authorize]
        public ActionResult NewSeason(int leagueID, int seasonID, int userID, int teamID, DateTime draftDate, DateTime startDate, DateTime endDate)
        {
            League model = new League();
            string leagueType = "";
            try
            {
                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Get league info to create season
                model = model.GetLeagueInfo(dbconAPP, leagueID, seasonID);
                model.FantasyTeamID = teamID;
                model.DraftDate = draftDate;
                model.StartDate = startDate;
                model.EndDate = endDate;

                //Create new season using existing league info
                model.CreateSeason(dbconAPP, model);

                //Get new season id
                Season modelSeason = new Season();
                model.SeasonID = modelSeason.GetSeasonID(dbconAPP, model);

                //Get league info for the new season
                model = model.GetLeagueInfo(dbconAPP, model.LeagueID, model.SeasonID);
                model.FantasyTeamID = teamID;
                if (model.isPublic)
                    leagueType = "public";
                else
                    leagueType = "private";

                //Disable editing league settings if past season end date
                if (DateTime.Now > model.EndDate)
                {
                    ViewBag.SeasonEnded = true;
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { leagueType = leagueType, leagueID = model.LeagueID,
                    seasonID = model.SeasonID, fantasyTeamID = model.FantasyTeamID, errMsg = "ERROR: " + ex.Message + " (League.NewSeason[POST])."
                });
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return RedirectToAction("Index", new { leagueType = leagueType, leagueID = model.LeagueID, seasonID = model.SeasonID, fantasyTeamID = model.FantasyTeamID });
        }
        #endregion

    }
}
