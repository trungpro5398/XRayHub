using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XRayHub.Models;
using System.Globalization;

namespace XRayHub.Controllers

{
    public class AppointmentsController : Controller
    {
        private XRayHub_Models dbContext = new XRayHub_Models();


        // GET: Appointments/ScheduleAppointment
        public ActionResult ScheduleAppointment()
        {


            // Fetch the list of available practitioners for the dropdown in the view.
            ViewBag.Practitioners = dbContext.MedicalPractitioners.ToList();
            ViewBag.Facilities = GetDistinctFacilities(); // Re-populate facilities.

            return View();
        }

        // POST: Appointments/ScheduleAppointment
        [HttpPost]
        public ActionResult ScheduleAppointment(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewBagData(); // Combined the two ViewBag population lines.
                return View("ScheduleAppointment", model);
            }

            string userId = User.Identity.GetUserId();
            Patient patient = dbContext.Patients.FirstOrDefault(p => p.IdentityUserID == userId);

            if (patient == null)
            {
                // This might be an error condition based on your application's logic.
                PopulateViewBagData();
                return View("ScheduleAppointment", model);
            }

            Appointment appointment = ConvertToEntity(model, patient.PatientID);

            dbContext.Appointments.Add(appointment);
            dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }


        private void PopulateViewBagData()
        {
            ViewBag.Practitioners = dbContext.MedicalPractitioners.ToList();
            ViewBag.Facilities = GetDistinctFacilities();
        }

        private Appointment ConvertToEntity(AppointmentViewModel model, int patientId)
        {
            return new Appointment
            {
                PatientID = patientId,
                DateScheduled = model.DateScheduled,
                PractitionerID = model.PractitionerID,
                Reason = model.Reason,
                TypeOfXray = "xray",
                Status = "Scheduled",  // Changed status to "Pending".
                CreatedAt = DateTime.Now
            };
        }

    


        [HttpGet]
        public ActionResult CheckAvailability(int practitionerId, DateTime appointmentDateTime)
        {
            // Check if there's an appointment at the provided date and time for the given practitioner.
            var isAvailable = !dbContext.Appointments.Any(a => a.PractitionerID == practitionerId && a.DateScheduled == appointmentDateTime);

            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }


        // Create this method to provide practitioners based on the facility
        public JsonResult GetPractitionersByFacility(string facilityName)
        {
            var practitioners = dbContext.MedicalPractitioners
                .Where(p => p.Facility == facilityName)
                .Select(p => new { PractitionerID = p.PractitionerID, FirstName = p.FirstName, LastName = p.LastName })
                .ToList();

            return Json(practitioners, JsonRequestBehavior.AllowGet);
        }

        private List<string> GetDistinctFacilities()
        {
            var facilities = dbContext.MedicalPractitioners
                                      .Select(p => p.Facility)
                                      .Distinct()
                                      .ToList();
            // Log or debug facilities here to ensure it's not empty.
            return facilities;
        }
       


    }
}