using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.Pkcs;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using E_Hospital.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace E_Hospital.Controllers
{
    public class ReservationsController : Controller
    {
        private System_Models db = new System_Models();

        // GET: Reservations
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var reservations = db.Reservations.Where(s => s.PatientId == userId);
            
            return View(reservations.ToList());
        }

        // GET: Reservations/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        //Select the doctor from the list when Create()
        public ActionResult SelectDoctor()
        {
            return View(db.Doctors.ToList());
        }

        // GET: Reservations/Create
        public ActionResult Create(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Doctor doctor = db.Doctors.Find(id);

            if (doctor == null)
            {
                return HttpNotFound();
            }

            ViewBag.PatientId = User.Identity.GetUserId();
            ViewBag.DoctorId = id;
            ViewBag.DoctorTitle = doctor.FirstName + " " + doctor.LastName;
            
            // Conflict Dates
            List<String> dateList = new List<String>();
            var reservations = db.Reservations.Where(m => m.DoctorId == id);
            
            foreach (Reservation item in reservations)
            {

                dateList.Add(item.Date.ToString("dd/M/yyyy"));
            }

            ViewBag.ConflictDates = dateList;
            
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Date,Status,Description,DoctorId,PatientId")] Reservation reservation)
        {
            
            if (ModelState.IsValid)
            {
                // Set the value of the new reservation
                reservation.Id = RandomId(128);
                // Send Email
                var subject = "E-Hospital: You have make a new reservation successfully!";
                var content = "<br/><br/>You have created a new reservation at " + DateTime.Now +
                              ". Please wait for doctor's confirmation. <br/><br/>Thank you for using E-Hospital";
                if (!User.IsInRole("Admin"))
                {
                    var userId = User.Identity.GetUserId();
                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                        .GetUserManager<ApplicationUserManager>().FindById(userId);

                    SendEmail(user.Email, subject, content);
                    Debug.WriteLine("Successfully send email");
                }
                
                db.Reservations.Add(reservation);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
            }

            
            return View(reservation);
        }

        // Get: Load the hospital view
        public ActionResult ShowHospital(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            
            if (doctor == null)
            {
                return HttpNotFound();
            }

            Hospital hospital = db.Hospitals.Find(doctor.HospitalId);
            var tuple = new Tuple<Doctor, Hospital>(doctor, hospital);
            return View(tuple);
        }

        // Reschedule the reservation time
        // GET: Reservations/Edit/5
        [Authorize]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }

            ViewBag.PreviousDate = reservation.Date.ToString("dd/M/yyyy");

            // Conflict Dates
            List<String> dateList = new List<String>();
            var userId = User.Identity.GetUserId();
            var reservations = db.Reservations.Where(m => m.PatientId == userId);

            foreach (Reservation item in reservations)
            {

                dateList.Add(item.Date.ToString("dd/M/yyyy"));
            }

            ViewBag.ConflictDates = dateList;
            ViewBag.PatientId = userId;
            ViewBag.DoctorId = reservation.DoctorId;
            ViewBag.Id = id;

            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Status,Description,PatientId,DoctorId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
               
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
            }



            return View(reservation);
        }

        // Get the list of unconfirmed reservations
        // GET: Reservations/ConfirmList
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmList()
        {
            var reservations = db.Reservations.Where(r => r.Status == "Not Confirmed");

            return View(reservations.ToList());
        }

        // Confirm the reservation
        // GET: Reservations/ConfirmStatus
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmStatus(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            
            if (reservation == null)
            {
                return HttpNotFound();
            }

            ViewBag.Id = reservation.Id;
            ViewBag.Date = reservation.Date;
            ViewBag.Description = reservation.Description;
            ViewBag.PatientId = reservation.PatientId;
            ViewBag.DoctorId = reservation.DoctorId;

            return View(reservation);
        }

        // POST: Reservation/ConfirmStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmStatus([Bind(Include = "Id,Date,Status,Description,PatientId,DoctorId")] Reservation reservation)
        {
            

            if (ModelState.IsValid)
            {
                // Send Email
                var subject = "E-Hospital: Your Reservation has been confirmed by the doctor!";
                var content = "<br/><br/>Your reservation has been confirmed at " + DateTime.Now +
                              ". Please meet your doctor on time! <br/><br/>Thank you for using E-Hospital";
                db.Entry(reservation).State = EntityState.Modified;

                db.SaveChanges();
                var userId = reservation.PatientId;
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext()
                       .GetUserManager<ApplicationUserManager>().FindById(userId);

                SendEmail(user.Email, subject, content);
                Debug.WriteLine("Successfully send email");

                return RedirectToAction("ConfirmList");

            }
            else
            {

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
            }



            return View(reservation);

        }


        // GET: Reservations/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Create random string
        private static Random random = new Random();
        [NonAction]
        public static string RandomId(int len)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars,len).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Send Email
        [NonAction]
        public void SendEmail(string email, string subject, string content)
        {

            var fromEmail = new MailAddress("twuu0012@student.monash.edu", "E-Hospital");
            var toEmail = new MailAddress(email);
            var fromEmailPasswd = "Caoshanshiwonvpengyou";
            
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPasswd)
               
                
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            })
            smtp.Send(message);

        }

        // GET: Calendar page
        public ActionResult ShowCalendar()
        {
            return View();
        }

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
