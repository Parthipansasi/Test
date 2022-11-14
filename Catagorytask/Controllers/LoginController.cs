using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Catagorytask.Models;

namespace Catagorytask.Controllers
{
    public class LoginController : Controller
    {
        private sapEntities2 db = new sapEntities2();
        // GET: Login
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult register(Register_Table Registers)
        {
            using (sapEntities2 db = new sapEntities2())
            {
                if (Registers.Id == 0)
                {
                    db.Register_Tables.Add(Registers);
                    db.SaveChanges();
                    return Json(new { success = true, message = "User register successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(Registers).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Register_Table model, string returnUrl)
        {
            {
                bool userExits = db.Register_Tables.Any(x => x.Username == model.Username && x.PassWord == model.PassWord);
                Register_Table s = db.Register_Tables.FirstOrDefault(x => x.Username == model.Username && x.PassWord == model.PassWord);
                if (userExits)
                {
                    FormsAuthentication.SetAuthCookie(s.Username, false);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Username or Password is wrong");
                return View(model);
            }
        }
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return View("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}