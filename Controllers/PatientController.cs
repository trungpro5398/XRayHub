using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XRayHub.Models;

namespace XRayHub.Controllers
{

    public class PatientController : Controller
    {
        private XRayHub_Models dbContext = new XRayHub_Models();

        public ActionResult Dashboard()
        {
            return View();
        }

        public new ActionResult Profile()
        {
            return View();
        }
        public ActionResult Appointments()
        {
            ViewBag.Practitioners = dbContext.MedicalPractitioners.ToList();
            return View("~/Views/Appointments/ScheduleAppointment.cshtml");
        }


        public ActionResult ReviewRecords()
        {
            return View();
        }
    }

}