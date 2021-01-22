using FantasyRollerDerby.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FantasyRollerDerby.Controllers
{
    public class FantasyTeamController : Controller
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

        // GET: FantasyTeam
        [Authorize]
        public ActionResult Index(int fantasyTeamID, int seasonID)
        {
            _FantasyTeam model = new _FantasyTeam();
            try
            {
                //Get and open connection
                dbconAPP = GetConnection();
                dbconAPP.Open();

                //Get app user info
                model.cUser = new User();
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                model.cUser.AspNetUserID = user.Id;
                model.cUser.GetUserID(dbconAPP, model.cUser.AspNetUserID);

                //Set Season info
                model.cSeason = new Season();
                model.cSeason.SeasonID = seasonID;
                model.cSeason.GetSeasonInfo(dbconAPP, model.cSeason);

                //Get Fantasy Team basic info
                model.cFantasyTeam = new FantasyTeam();
                model.cFantasyTeam.FantasyTeamID = fantasyTeamID;
                model.cFantasyTeam.GetFantasyTeamInfo(dbconAPP, model.cFantasyTeam);

                //Get Fantasy Team skater list
                model.cSkater = new Skater();
                model.list_cSkater = new List<Skater>();
                model.list_cSkater = model.cSkater.GetFantasyTeamSkaterList(dbconAPP, seasonID, fantasyTeamID);

            }
            catch (Exception ex)
            {
                ViewBag.FantasyTeamError += "ERROR: " + ex.Message + " (FantasyTeam.Index[GET]).";
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

        // GET: FantasyTeam/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FantasyTeam/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FantasyTeam/Create
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

        // GET: FantasyTeam/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FantasyTeam/Edit/5
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

        // GET: FantasyTeam/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FantasyTeam/Delete/5
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
