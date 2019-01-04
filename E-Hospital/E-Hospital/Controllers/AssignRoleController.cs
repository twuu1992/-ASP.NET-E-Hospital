using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Hospital.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace E_Hospital.Controllers
{
    public class AssignRoleController : Controller
    {
        private ApplicationDbContext context;

        public AssignRoleController()
        {
            context = new ApplicationDbContext();
            
        }

        

        // GET: AssignRole
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var Roles = context.Roles.ToList();
            return View(Roles);
        }

        // GET: Create a new role
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        //POST: Create a role
        [HttpPost]
        public ActionResult Create(IdentityRole Role)
        {
            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}