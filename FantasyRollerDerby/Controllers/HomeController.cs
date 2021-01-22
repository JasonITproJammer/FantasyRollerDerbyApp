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
    [RequireHttps]
    public class HomeController : Controller
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
        public ActionResult Index(string msg)
        {
            ViewBag.HomeError = msg;
            _Home model = new _Home();
            dbconAPP = GetConnection();
            try
            {
                dbconAPP.Open();
                if (msg == "joinaleague")
                {
                    model.GetNonLeagueMemberData(dbconAPP, model);
                    return View("NonMember", model);
                }

                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                if (user == null)
                {
                    model.GetNonLeagueMemberData(dbconAPP, model);
                    return View("NonMember", model);
                }
                else
                {
                    model.cUser = new User();
                    model.cUser.UserID = model.cUser.GetUserID(dbconAPP, user.Id);
                    model.GetLeagueMemberData(dbconAPP, model);
                    if(model.list_cLeaguePrivate.Count < 1 && model.list_cLeaguePublic.Count < 1)
                    {
                        model.GetNonLeagueMemberData(dbconAPP, model);
                        return View("NonMember", model);
                    }
                    return View("Member", model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.HomeError += "ERROR: " + ex.Message + " (Home.Index[GET]).";
                return View("NonMember", model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
        }

        public ActionResult Leagues()
        {
            _Home model = new _Home();
            dbconAPP = GetConnection();
            try
            {
                dbconAPP.Open();
                model.GetNonLeagueMemberData(dbconAPP, model);
                return View("NonMember", model);
            }
            catch (Exception ex)
            {
                ViewBag.HomeError += "ERROR: " + ex.Message + " (Home.League[GET]).";
                return View("NonMember", model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
        }

        public ActionResult Teams()
        {
            _Home model = new _Home();
            dbconAPP = GetConnection();
            try
            {
                dbconAPP.Open();
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                if (user == null)
                {
                    model.GetNonLeagueMemberData(dbconAPP, model);
                    return View("NonMember", model);
                }
                else
                {
                    model.cUser = new User();
                    model.cUser.UserID = model.cUser.GetUserID(dbconAPP, user.Id);
                    model.GetLeagueMemberData(dbconAPP, model);
                    return View("Member", model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.HomeError += "ERROR: " + ex.Message + " (Home.League[GET]).";
                return View("NonMember", model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult JoinPublicLeague(int leagueID, int useriD)
        {
            _Home model = new _Home();
            dbconAPP = GetConnection();
            try
            {
                dbconAPP.Open();
                model.list_cLeaguePublic = new List<League>();
                model.cUser = new User();
                model.cLeague = new League();
                model.cUser.UserID = useriD;
                model.cLeague.LeagueID = leagueID;
                model.GetNonLeagueMemberData(dbconAPP, model);
                List<League> tempPublic = new List<League>();
                tempPublic = model.list_cLeaguePublic.Where(n => n.LeagueID == leagueID).ToList<League>();
                model.cLeague = tempPublic[0];
                ViewBag.LeagueType = "Public";
            }
            catch (Exception ex)
            {
                ViewBag.HomeError += "ERROR: " + ex.Message + " (Home.JoinPublicLeague[GET]).";
                return View("JoinLeague", model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return View("JoinLeague", model);
        }

        public ActionResult JoinPrivateLeague(int leagueID, int useriD)
        {
            _Home model = new _Home();
            dbconAPP = GetConnection();
            try
            {
                dbconAPP.Open();
                model.list_cLeaguePrivate = new List<League>();
                model.cUser = new User();
                model.cLeague = new League();
                model.cUser.UserID = useriD;
                model.cLeague.LeagueID = leagueID;
                model.GetNonLeagueMemberData(dbconAPP, model);
                List<League> tempPublic = new List<League>();
                tempPublic = model.list_cLeaguePrivate.Where(n => n.LeagueID == leagueID).ToList<League>();
                model.cLeague = tempPublic[0];
                ViewBag.LeagueType = "Private";
            }
            catch (Exception ex)
            {
                ViewBag.HomeError += "ERROR: " + ex.Message + " (Home.JoinPrivateLeague[GET]).";
                return View("JoinLeague", model);
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return View("JoinLeague", model);
        }

        public ActionResult SubmitPassword(string buttonAction, string pwd, string LeagueID)
        {
            string result = "fail";
            dbconAPP = GetConnection();
            try
            {
                //Verify password
                dbconAPP.Open();
                string strsql = "SELECT * FROM tblLeague WHERE Password = '" + pwd + "' AND LeagueID = " + Convert.ToInt32(LeagueID);
                var cmd = new SqlCommand(strsql, dbconAPP);
                SqlDataReader myReader;
                myReader = cmd.ExecuteReader();
                int count = 0;
                while (myReader.Read())
                {
                    count++;
                }
                myReader.Close();
                result = count > 0 ? "pass" : "fail";
            }
            catch (Exception ex)
            {
                result += "ERROR: " + ex.Message + " (Home.SubmitPassword[POST]).";
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return Content(result);
        }
        #endregion

        #region POSTS
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Index(_Home model, string buttonAction, string leagueID)
        {
            FantasyTeam modelFantasyTeam = new FantasyTeam();
            string errType = "League";
            string errMsg = "";
            try
            {
                //Get connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Verify user isn't already apart of the league
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.cLeague = new League();
                model.cUser = new User();
                model.cUser.UserID = model.cUser.GetUserID(dbconAPP, user.Id);
                model.cLeague.LeagueID = Convert.ToInt32(leagueID);
                if (model.cLeague.VerifyLeagueUsers(dbconAPP, model.cLeague, model.cUser.UserID))
                {
                    return Content("You cannot join this league because you are already a member.");
                }

                //Process request based on buttonAction value
                switch (buttonAction.ToUpper())
                {
                    case "PUBLIC":
                        //TempData["model"] = model;
                        List<League> tempPublic = new List<League>();
                        tempPublic = model.list_cLeaguePublic.Where(n => n.LeagueID == model.cLeague.LeagueID).ToList<League>();
                        model.cLeague = tempPublic[0];
                        ViewBag.LeagueType = "Public";
                        //return View("JoinLeague", model);
                        //return RedirectToAction("JoinPublicLeague");
                        break;

                    case "PRIVATE":
                        //TempData["model"] = model;
                        List<League> tempPrivate = new List<League>();
                        tempPrivate = model.list_cLeaguePrivate.Where(n => n.LeagueID == model.cLeague.LeagueID).ToList<League>();
                        model.cLeague = tempPrivate[0];
                        ViewBag.LeagueType = "Private";
                        //return View("JoinLeague", model);
                        //return RedirectToAction("JoinPrivateLeague");
                        break;
                }

                //Get Fantasy Team info
                errType = "FantasyTeam";
                modelFantasyTeam.UserID = model.cUser.UserID;
                modelFantasyTeam.LeagueID = model.cLeague.LeagueID;
                modelFantasyTeam.LeagueName = model.cLeague.LeagueName;
                return PartialView("_JoinLeague", modelFantasyTeam);
            }
            catch (Exception ex)
            {
                if (errType == "League")
                {
                    errMsg += "ERROR: " + ex.Message + " (Home.Index[POST]).";
                    return Content(errMsg);
                }
                else
                {
                    errMsg += "ERROR: " + ex.Message + " (Home.Index[POST]).";
                    ViewBag.JoinLeagueError = errMsg;
                    return PartialView("_JoinLeague", modelFantasyTeam);
                }
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
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult GoToLeague(_Home model, string buttonAction, string LeagueID, string SeasonID, string FantasyTeamID)
        {
            //if (buttonAction == "public")
            //{
            //    TempData["LeagueList"] = model.list_cLeaguePublic;
            //}
            //else
            //{
            //    TempData["LeagueList"] = model.list_cLeaguePrivate;
            //}
            return RedirectToAction("Index", "League", new { leagueType = buttonAction, leagueID = LeagueID, seasonID = SeasonID, fantasyTeamID = FantasyTeamID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult JoinPublicLeague(_Home model, string buttonAction, HttpPostedFileBase uploadfile)
        {
            //model = (_Home)TempData["model"];
            if (uploadfile != null && uploadfile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(uploadfile.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/TeamLogos"), fileName);
                uploadfile.SaveAs(path);
                model.cFantasyTeam.Logo = fileName;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult JoinPrivateLeague(_Home model, string buttonAction, HttpPostedFileBase uploadfile)
        {
            //model = (_Home)TempData["model"];
            if (uploadfile != null && uploadfile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(uploadfile.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/TeamLogos"), fileName);
                uploadfile.SaveAs(path);
                model.cFantasyTeam.Logo = fileName;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult JoinLeague(_Home model, string buttonAction, HttpPostedFileBase uploadfile)
        {
            dbconAPP = GetConnection();
            try
            {
                //save logo
                if (uploadfile != null && uploadfile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(uploadfile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/TeamLogos"), fileName);
                    uploadfile.SaveAs(path);
                    model.cFantasyTeam.Logo = fileName;
                }
                else
                {
                    model.cFantasyTeam.Logo = "defaultTeamLogo.png";
                }

                //Add fantasy team
                dbconAPP.Open();
                model.cFantasyTeam.UserID = model.cUser.UserID;
                model.cFantasyTeam.LeagueID = model.cLeague.LeagueID;
                model.cFantasyTeam.CreateNewFantasyTeam(dbconAPP, model.cFantasyTeam);
            }
            catch (Exception ex)
            {
                ViewBag.HomeError += "ERROR: " + ex.Message + " (Home.JoinLeague[POST]).";
                return RedirectToAction("Index", "Home", new { msg = ViewBag.HomeError });
            }
            finally
            {
                if (dbconAPP != null && dbconAPP.State == System.Data.ConnectionState.Open)
                {
                    dbconAPP.Close();
                }
            }
            return RedirectToAction("Index", "Home", new { msg = ViewBag.HomeError });
        }
        #endregion
    }
}