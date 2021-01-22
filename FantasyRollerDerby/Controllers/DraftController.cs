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
using FantasyRollerDerby.Hubs;

namespace FantasyRollerDerby.Controllers
{
    public class DraftController : Controller
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

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            //string fantasyTeamName, string leagueName, int seasonID, int teamID
            _Draft model = new _Draft();
            model.cUser = new User();
            model.cFantasyTeam = new FantasyTeam();
            model.cLeague = new League();
            model.cSeason = new Season();
            model.cSkater = new Skater();
            model.cDerbyTeam = new DerbyTeam();
            model.list_cDerbyTeam = new List<DerbyTeam>();
            model.list_cFantasyTeam = new List<FantasyTeam>();
            model.list_cLeague = new List<League>();
            model.list_cSeason = new List<Season>();
            model.list_cSkater = new List<Skater>();
            try
            {
                //Get session vars
                string fantasyTeamName = Session["fantasyTeamName"].ToString();
                string leagueName = Session["leagueName"].ToString();
                int seasonID = Convert.ToInt32(Session["seasonID"].ToString());
                int teamID = Convert.ToInt32(Session["teamID"].ToString());

                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Get UserID
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.cUser.AspNetUserID = user.Id;
                model.cUser.UserID = model.cUser.GetUserID(dbconAPP, model.cUser.AspNetUserID);

                //Get Draft Info
                Session["FantasyTeamName"] = fantasyTeamName;
                Session["LeagueName"] = leagueName;
                model.cFantasyTeam.Name = fantasyTeamName;
                model.cLeague.LeagueName = leagueName;
                model.cSeason.SeasonID = Convert.ToInt32(seasonID);
                model.cFantasyTeam.SeasonID = Convert.ToInt32(seasonID);
                model.cFantasyTeam.FantasyTeamID = Convert.ToInt32(teamID);
                model.GetDraftInfo(dbconAPP, model);
                foreach (var item in model.list_cSkater)
                {
                    List<DerbyTeam> tempList = new List<DerbyTeam>();
                    tempList = model.list_cDerbyTeam.Where(n => n.DerbyTeamID == item.DerbyTeamID).ToList<DerbyTeam>();
                    item.DerbyTeamName = tempList[0].Name;
                }

                //Calculate total draft time in seconds and convert to minutes
                int totalDraftSeconds = model.list_SeasonFantasyTeams.Count * model.cSeason.RosterSize * model.cSeason.DraftTimePerTeam;
                ViewBag.TotalDraftTime = totalDraftSeconds / 60;
                ViewBag.TotalPicks = model.list_SeasonFantasyTeams.Count * model.cSeason.RosterSize;

                //Create Draft Order if the draft order hasn't been created yet
                if (model.cSeason.DraftOrder == null || model.cSeason.DraftOrder == "")
                {
                    Dictionary<int, int> dictDraftOrder = new Dictionary<int, int>();
                    Random randoNum = new Random();
                    int numTeams = model.list_SeasonFantasyTeams.Count;
                    int min = 0;
                    int max = numTeams;

                    for (int i = 1; i <= numTeams; i++)
                    {
                        int intTemp = randoNum.Next(min, max);
                        while (dictDraftOrder.ContainsValue(model.list_SeasonFantasyTeams[intTemp].FantasyTeamID))
                        {
                            if (intTemp == min)
                                min++;
                            else if (intTemp == max)
                                max--;
                            intTemp = randoNum.Next(min, max);
                        }
                        if (intTemp == min)
                            min++;
                        else if (intTemp == max)
                            max--;
                        dictDraftOrder.Add(i, model.list_SeasonFantasyTeams[intTemp].FantasyTeamID);
                    }

                    for (int i = 1; i <= dictDraftOrder.Count; i++)
                    {
                        if (i != dictDraftOrder.Count)
                            model.cSeason.DraftOrder += dictDraftOrder[i].ToString() + "|";
                        else
                            model.cSeason.DraftOrder += dictDraftOrder[i].ToString();
                    }

                    model.cSeason.UpdateDraftOrder(dbconAPP, model.cSeason.SeasonID, model.cSeason.DraftOrder);
                    model.cSeason.UpdateCurrentPick(dbconAPP, model.cSeason.SeasonID, Convert.ToInt32(dictDraftOrder[1].ToString()));
                }

                //Get current pick
                model.cSeason.CurrentPickFantasyTeamID = model.cSeason.GetCurrentPickTeamID(dbconAPP, model.cSeason.SeasonID);
                FantasyTeam tempFTM = new FantasyTeam();
                tempFTM = model.list_SeasonFantasyTeams.Single(s => s.FantasyTeamID == model.cSeason.CurrentPickFantasyTeamID);
                model.cSeason.CurrentPickFantasyTeamName = tempFTM.Name;

                //Order fantasy teams into list
                string draftOrderNames = "";
                string draftOrderTeamLogos = "";
                model.list_DraftOrderTeams = new List<FantasyTeam>();
                List<string> tempOrderList = new List<string>();
                tempOrderList = model.cSeason.DraftOrder.Split('|').ToList();
                foreach (var item in tempOrderList)
                {
                    FantasyTeam tempModelFT = new FantasyTeam();
                    tempModelFT = model.list_SeasonFantasyTeams.Single(s => s.FantasyTeamID == Convert.ToInt32(item));
                    model.list_DraftOrderTeams.Add(tempModelFT);
                    draftOrderNames += tempModelFT.Name + "|";
                    draftOrderTeamLogos += tempModelFT.Logo + "|";
                }
                draftOrderNames = draftOrderNames.Substring(0, draftOrderNames.Length - 1);
                draftOrderTeamLogos = draftOrderTeamLogos.Substring(0, draftOrderTeamLogos.Length - 1);
                ViewBag.DraftOrderNames = draftOrderNames;
                ViewBag.DraftOrderTeamLogos = draftOrderTeamLogos;

                //Create Generic Draft Order
                int totalTeams = model.list_SeasonFantasyTeams.Count;
                int totalRounds = model.cSeason.RosterSize;
                string genericDraftOrder = "";
                for (int i = 0; i < totalRounds; i++)
                {
                    if ((i % 2) == 0)
                    {
                        //Walk UP
                        for (int j = 0; j < totalTeams; j++)
                        {
                            genericDraftOrder += j + "|";
                        }

                    }
                    else
                    {
                        //Walk DOWN
                        for (int j = (totalTeams - 1); j > -1; j--)
                        {
                            genericDraftOrder += j + "|";
                        }
                    }
                }
                //Delete last "|"
                genericDraftOrder = genericDraftOrder.Substring(0, genericDraftOrder.Length - 1);

                //Split out genericDraftOrder
                ViewBag.GenericDraftOrder = genericDraftOrder;


                //**************************TESTING**********************************************
                //Determine what draft picks should have already taken place and draft them
                var test1 = model.cSeason.DraftDate.ToBinary();
                var test2 = model.cSeason.DraftDate.ToFileTime(); //console log this
                //ViewBag.test2Date = model.cSeason.DraftDate;
                //ViewBag.Test2 = test2;
                var test3 = model.cSeason.DraftDate.ToFileTimeUtc();
                var nowFileTime = DateTime.Now.ToFileTime(); //console log this
                var nowDate = DateTime.UtcNow;
                //ViewBag.nowDateUTC = nowDate;
                //ViewBag.nowFileTime = nowFileTime;
                var nowFileTimeUTC = DateTime.Now.ToFileTimeUtc();

                //convert time to file time and append date to front when comparing

                //get draft time plus first pick
                DateTime testDraft1 = model.cSeason.DraftDate.AddSeconds(model.cSeason.DraftTimePerTeam);
                DateTime testDraft5 = testDraft1.ToUniversalTime();
                string testDraft2 = testDraft5.ToString("yyyyMMdd");
                string testDraft3 = testDraft5.ToFileTimeUtc().ToString();
                testDraft2 = testDraft2 + testDraft3;
                Double testDraft4 = Convert.ToDouble(testDraft2);

                //get current time
                DateTime nowDraft1 = DateTime.UtcNow;
                string nowDraft2 = nowDraft1.ToString("yyyyMMdd");
                string nowDraft3 = nowDraft1.ToFileTimeUtc().ToString();
                nowDraft2 = nowDraft2 + nowDraft3;
                Double nowDraft4 = Convert.ToDouble(nowDraft2);

                //if (model.cSeason.DraftDate.AddSeconds(model.cSeason.DraftTimePerTeam) < DateTime.Now)
                //if(testDraft4 < nowDraft4)
                if (1 == 2)
                {//if true, that means the draft has already started and is already past the first pick
                    double diffSeconds = (DateTime.Now - model.cSeason.DraftDate).TotalSeconds;
                    int estPicks = Convert.ToInt32(diffSeconds) / model.cSeason.DraftTimePerTeam;
                    for (int i = 0; i < estPicks; i++)
                    {
                        List<string> listGenericDraftOrder = new List<string>();
                        listGenericDraftOrder = genericDraftOrder.Split('|').ToList();
                        var pick = Convert.ToInt32(listGenericDraftOrder[i].ToString());
                        var checkTeamID = model.list_DraftOrderTeams[pick].FantasyTeamID;
                        bool draftCheck = model.TeamDraftedCount(dbconAPP, model.cSeason.SeasonID, checkTeamID, i + 1);
                        if (!draftCheck)
                        {
                            //draft highest ranked player left to team
                            tempFTM = new FantasyTeam();
                            tempFTM.SkaterID = model.cSkater.GetNextRankedSkaterID(dbconAPP, Convert.ToInt32(seasonID));
                            tempFTM.FantasyTeamID = checkTeamID;
                            tempFTM.SeasonID = model.cSeason.SeasonID;
                            tempFTM.DraftPickNumber = i + 1;
                            tempFTM.UserID = model.cUser.UserID;
                            //SELECT * FROM tblFantasyTeamRoster WHERE SeasonID = 4 
                            //SELECT* FROM tblSeason WHERE SeasonID = 4
                            //Draft is not snaking here, still snaking when logging in before the draft starts
                            model.cFantasyTeam.AddSkaterToFantasyTeam(dbconAPP, tempFTM);
                        }
                    }

                    //refresh list of available skaters
                    model.list_cSkater = new List<Skater>();
                    model.list_cSkater = model.cSkater.GetSkaterList(dbconAPP, model.cFantasyTeam.SeasonID);
                    foreach (var item in model.list_cSkater)
                    {
                        List<DerbyTeam> tempList = new List<DerbyTeam>();
                        tempList = model.list_cDerbyTeam.Where(n => n.DerbyTeamID == item.DerbyTeamID).ToList<DerbyTeam>();
                        item.DerbyTeamName = tempList[0].Name;
                    }
                }
                //**************************TESTING**********************************************


                //Generate a random number for different users
                Random rnd = new Random();
                ViewBag.Random = rnd.Next(1, 100);
            }
            catch (Exception ex)
            {
                ViewBag.LeagueError += "ERROR: " + ex.Message + " (Draft.Index[GET]).";
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

        // GET: Draft/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Draft/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Draft/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Draft/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddSkaterToFantasyTeam(string teamID, string seasonID,
            string skaterID, string draftPickNumber, string userID, string userTeamID, string random)
        {
            try
            {
                //Assign parameters
                int intTeamID = Convert.ToInt32(teamID);
                int intSkaterID = Convert.ToInt32(skaterID);
                int intSeasonID = Convert.ToInt32(seasonID);
                int intDraftPickNumber = Convert.ToInt32(draftPickNumber);
                int intUserID = Convert.ToInt32(userID);
                int intUserTeamID = Convert.ToInt32(userTeamID);
                int intRandom = Convert.ToInt32(random);

                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Instantiate objects
                FantasyTeam model = new FantasyTeam();
                User modelUser = new User();

                //Pause for a random number of milliseconds
                //Random rnd = new Random();
                //int pause = rnd.Next(1, 1000);
                int pause = intUserTeamID * intUserID;
                while (pause > 2000)
                    pause = pause / intRandom;
                System.Threading.Thread.Sleep(pause);
                if (model.IsSkaterDrafted(dbconAPP, intTeamID, intSeasonID, intSkaterID))
                {
                    //Skater was already drafted by another client
                    //return success because skater is already drafted to the team
                    return Content("Draft.AddSkaterToFantasyTeam[POST] Skater already drafted to this team.");
                }
                else
                {
                    //Insert record into draft table
                    //ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    //modelUser.AspNetUserID = user.Id;
                    //modelUser.UserID = modelUser.GetUserID(dbconAPP, modelUser.AspNetUserID);
                    model.FantasyTeamID = intTeamID;
                    model.SeasonID = intSeasonID;
                    model.SkaterID = intSkaterID;
                    model.UserID = intUserID;
                    model.DraftPickNumber = intDraftPickNumber;
                    model.AddSkaterToFantasyTeam(dbconAPP, model);
                    return Content("Draft.AddSkaterToFantasyTeam[POST] Successfully ADDED!");
                }

            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message + " (Draft.AddSkaterToFantasyTeam[POST]).");
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateCurrentTeamDrafting(string teamID, string seasonID)
        {
            try
            {
                //convert string to int for SPROC
                int intTeamID = Convert.ToInt32(teamID);
                int intSeasonID = Convert.ToInt32(seasonID);

                //Update record in tblSeason
                dbconAPP = GetConnection();
                dbconAPP.Open();
                Season model = new Season();
                model.UpdateCurrentPick(dbconAPP, intSeasonID, intTeamID);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message + " (Draft.UpdateCurrentTeamDrafting[POST]).");
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return Content("Draft.UpdateCurrentTeamDrafting[POST] Success!");
        }

        //GetNextRankedSkaterID
        [HttpGet]
        public ActionResult GetNextRankedSkaterID(int seasonID)
        {
            Skater model = new Skater();
            try
            {
                dbconAPP = GetConnection();
                dbconAPP.Open();
                model.SkaterID = model.GetNextRankedSkaterID(dbconAPP, seasonID);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message + " (Draft.GetNextRankedSkaterID[GET]).");
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return Content(model.SkaterID.ToString());
        }


        // POST: Draft/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Draft/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Draft/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
