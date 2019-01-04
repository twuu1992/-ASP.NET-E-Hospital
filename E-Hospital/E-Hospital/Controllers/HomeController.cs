using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Hospital.Models;
using Microsoft.AspNet.Identity;

namespace E_Hospital.Controllers
{
    public class HomeController : Controller
    {
        private System_Models db = new System_Models();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var reservations = db.Reservations.Where(r => r.PatientId == userId);
            return View(reservations.ToList());
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

        //// GET: Events/Create?date=YYYY-MM-DD
        //public ActionResult Create(String date)
        //{
        //    if (null == date)
        //        return RedirectToAction("Index");
        //    Event e = new Event();
        //    DateTime convertedDate = DateTime.Parse(date);
        //    e.Start = convertedDate;
        //    return View(e);
        //}

        public JsonResult GetEvents()
        {
            var userId = User.Identity.GetUserId();
            using (System_Models dc = new System_Models())
            {
                var reservations = dc.Reservations.Where(r => r.PatientId == userId && r.Status == "Confirmed").ToList();
                var events = new List<Event>();
                foreach (var r in reservations)
                {
                    Event e = new Event();
                    e.Start = r.Date;
                    e.Title = r.Description;
                    events.Add(e);
                }

                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

    }
}