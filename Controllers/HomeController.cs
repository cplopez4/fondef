using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fondef.DAL;
using Fondef.Models;

namespace Fondef.Controllers
{
    public class HomeController : Controller
    {
        private FondefContext db = new FondefContext();
        public ActionResult Index()
        {
            ViewData["Shapes"] = db.Cuencas.ToList();
            return View();
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
    }
}