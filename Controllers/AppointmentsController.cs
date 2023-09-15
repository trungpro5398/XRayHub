using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XRayHub.Models;
using System.Diagnostics;
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
            return View();
        }

        // POST: Appointments/ScheduleAppointment
        [HttpPost]
        public ActionResult ScheduleAppointment(AppointmentViewModel model)
        {
          
            if (ModelState.IsValid)
            {
                Debug.WriteLine(User.Identity.GetUserId());
                var identity = User.Identity.GetUserId();
                 var patient = dbContext.Patients.FirstOrDefault(p => p.IdentityUserID == identity);
                int patientId = patient.PatientID;

                // Convert ViewModel to actual Appointment entity.
                Appointment appointment = new Appointment
                {
                    PatientID = patientId,

                DateScheduled = model.DateScheduled,
                    PractitionerID = model.PractitionerID,
                    Reason = model.Reason,
                    TypeOfXray = "xray",
                    Status = "dasd",
                    CreatedAt = DateTime.Now

                    // ... You can set other properties if they're required.
                };

                // Save to the database.
                dbContext.Appointments.Add(appointment);
                dbContext.SaveChanges();

                // Redirect to a success/notification page or back to the form with a success message.
                return RedirectToAction("Index", "Home");
            }
            Console.WriteLine("trung");

            // If we get here, something failed in the validation, so redisplay the form.
            ViewBag.Practitioners = dbContext.MedicalPractitioners.ToList();
            return View("ScheduleAppointment", model);

        }



    }
}
