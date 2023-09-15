using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XRayHub.Models;

namespace XRayHub.Controllers
{
    public class MedicalPractitionerController : Controller
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

        public ActionResult UploadXray()
        {
            return View();
        }
        // GET: Appointments/ReviewAppointments
        [Authorize(Roles = "MedicalPractitioner")]
        public ActionResult ReviewAppointments()
        {
            var practitionerId = User.Identity.GetUserId();
            var practitioner = dbContext.MedicalPractitioners.FirstOrDefault(p => p.IdentityUserID == practitionerId);

            // Ensure you are actually fetching the appointments.
            var appointments = dbContext.Appointments.Where(a => a.PractitionerID == practitioner.PractitionerID).ToList();

            // Ensure that appointments is not null before returning
            if (appointments == null)
            {
                appointments = new List<Appointment>();  // initialize with an empty list
            }
            return View("~/Views/Appointments/ReviewAppointments.cshtml", appointments);


        }
    }

}