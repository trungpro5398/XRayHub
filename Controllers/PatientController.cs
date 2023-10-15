using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XRayHub.Models;
using System.Globalization;
using System.Data.Entity.Infrastructure;

namespace XRayHub.Controllers
{

    public class PatientController : Controller
    {
        private XRayHub_Models dbContext = new XRayHub_Models();

        public ActionResult Dashboard()
        {
            var userId = User.Identity.GetUserId(); // Get current user's ID
            var patient = dbContext.Patients.FirstOrDefault(p => p.IdentityUserID == userId);

            var viewModel = new PatientDashboardViewModel
            {
                Appointments = dbContext.Appointments.Where(a => a.PatientID == patient.PatientID).ToList(),
                XrayTimeline = dbContext.XrayRecords.Where(x => x.PatientID == patient.PatientID).OrderBy(x => x.CreatedAt).ToList()

            };

            // Fetching upcoming appointments and adding them to notifications
            var upcomingAppointments = viewModel.Appointments.Where(a => a.DateScheduled > DateTime.Now);
            foreach (var appointment in upcomingAppointments)
            {
                viewModel.Notifications.Add(new Notification
                {
                    Message = $"You have an appointment scheduled on {appointment.DateScheduled:MMMM dd, yyyy}.",
                    Date = appointment.DateScheduled
                });
            }

            return View(viewModel);
        }

        public new ActionResult Profile()
        {
            var userId = User.Identity.GetUserId();
            var patient = dbContext.Patients.FirstOrDefault(p => p.IdentityUserID == userId);
           
            if (patient == null)
            {
                return RedirectToAction("Login", "Account");

            }
            return View(patient);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Patient updatedProfile)
        {
            // Fetch the current patient
            var currentPatient = dbContext.Patients.FirstOrDefault(p => p.PatientID == updatedProfile.PatientID);

            if (currentPatient == null)
            {
                // Handle error (e.g., show an error message or redirect to an error page)
                return RedirectToAction("Error", "Home");  // Redirecting to an "Error" action as an example
            }

            // Update the patient details
            currentPatient.FirstName = updatedProfile.FirstName;
            currentPatient.LastName = updatedProfile.LastName;
            currentPatient.BirthDate = updatedProfile.BirthDate;
            currentPatient.ContactNumber = updatedProfile.ContactNumber;
            currentPatient.Email = updatedProfile.Email;

            // Save changes to the database
            dbContext.SaveChanges();

            // Redirect back to the profile page or display a success message
            return RedirectToAction("Profile");
        }


        // GET: Patient/ScheduleAppointment
        public ActionResult ScheduleAppointment()
        {


            // Fetch the list of available practitioners for the dropdown in the view.
            ViewBag.Practitioners = dbContext.MedicalPractitioners.ToList();
            ViewBag.Facilities = GetDistinctFacilities(); // Re-populate facilities.

            return View();
        }

        // POST: Patient/ScheduleAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleAppointment(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LogModelErrors();
                PopulateViewBagData(); // Combined the two ViewBag population lines.
                return View( model);
            }

            string userId = User.Identity.GetUserId();
            Patient patient = dbContext.Patients.FirstOrDefault(p => p.IdentityUserID == userId);

            if (patient == null)
            {
                // This might be an error condition based on your application's logic.
                LogError("Patient not found for the current user.");
                PopulateViewBagData();
                return View( model);
            }

            Appointment appointment = ConvertToEntity(model, patient.PatientID);

            dbContext.Appointments.Add(appointment);
            dbContext.SaveChanges();

            return RedirectToAction("ScheduleAppointment");
        }

        private void LogModelErrors()
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Debug.WriteLine($"Error: {error.ErrorMessage}, Exception: {error.Exception}");
                }
            }
        }

        private void PopulateViewBagData()
        {
            ViewBag.Practitioners = dbContext.MedicalPractitioners.ToList();
            ViewBag.Facilities = GetDistinctFacilities();
        }

        private Appointment ConvertToEntity(AppointmentViewModel model, int patientId)
        {
            string formattedDateTime = model.DateScheduled.ToString("dd/MM/yyyy HH:mm:ss");
            Debug.WriteLine(formattedDateTime);

            return new Appointment
            {

                PatientID = patientId,
                DateScheduled = DateTime.ParseExact(formattedDateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                PractitionerID = model.PractitionerID,

                Reason = model.Reason,
                TypeOfXray = "xray",
                Status = "Scheduled", 
                CreatedAt = DateTime.Now
            };
        }

        private void LogError(string message)
        {
            // You can use a more sophisticated logging system here if available.
            Debug.WriteLine($"Error: {message}");
        }





        [HttpGet]
        public ActionResult CheckAvailability(int practitionerId, DateTime appointmentDateTime)
        {
            // Check if there's an appointment at the provided date and time for the given practitioner.
            var isAvailable = !dbContext.Appointments.Any(a => a.PractitionerID == practitionerId && a.DateScheduled == appointmentDateTime);
            Debug.WriteLine(appointmentDateTime);

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


        public ActionResult ReviewRecords()
        {
            var patientId = User.Identity.GetUserId();
            var patient = dbContext.Patients.FirstOrDefault(p => p.IdentityUserID == patientId);

            if (patient == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var xrayRecords = dbContext.XrayRecords.Where(x => x.PatientID == patient.PatientID).ToList();

            // Fetch Practitioner details
            var practitionerIds = xrayRecords.Select(x => x.PractitionerID).Distinct().ToList();
            var practitioners = dbContext.MedicalPractitioners.Where(p => practitionerIds.Contains(p.PractitionerID))
                                 .ToDictionary(p => p.PractitionerID, p => p.FirstName + " " + p.LastName);

            var xrayTypes = dbContext.XrayTypes.ToList();

            var viewModel = new XRayHub.Models.XrayViewModel
            {
                XrayRecords = xrayRecords,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                XrayTypes = xrayTypes,
                PractitionerNames = practitioners // Updated this line
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult GetFilteredXrays(DateTime? fromDate, DateTime? toDate, int? practitionerID, string xrayType)
        {
            var patientId = User.Identity.GetUserId();
            var patient = dbContext.Patients.FirstOrDefault(p => p.IdentityUserID == patientId);
            if (patient == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Fetch the filtered records
            var filteredRecords = dbContext.XrayRecords
                .Where(x => x.PatientID == patient.PatientID)
                .Include(x => x.XrayType) // Ensure you load the related XrayType
                .Where(x =>
                    (!fromDate.HasValue || x.CreatedAt >= fromDate.Value) &&
                    (!toDate.HasValue || x.CreatedAt <= toDate.Value) &&
                    (string.IsNullOrEmpty(xrayType) || x.XrayType.TypeName == xrayType) &&
                    (!practitionerID.HasValue || x.PractitionerID == practitionerID.Value)

                )
                .ToList();

            // Fetch Practitioner details
            var practitionerIds = filteredRecords.Select(x => x.PractitionerID).Distinct().ToList();
            var practitioners = dbContext.MedicalPractitioners.Where(p => practitionerIds.Contains(p.PractitionerID))
                                 .ToDictionary(p => p.PractitionerID, p => p.FirstName + " " + p.LastName);
            var viewModel = new XRayHub.Models.XrayViewModel
            {
                XrayRecords = filteredRecords,
             
                PractitionerNames = practitioners // Updated this line
            };
            return PartialView("_XrayRecordsPartial", viewModel);
        }


        // GET: Patient/Feedback/{xrayRecordId}
        public ActionResult CreateFeedback(int xrayRecordId)
        {
            ViewBag.XrayRecordId = xrayRecordId;
            return View();
        }

        [HttpPost]
        public ActionResult CreateFeedback(Feedback model)
        {
            var patientId = User.Identity.GetUserId();
            var patient = dbContext.Patients.FirstOrDefault(p => p.IdentityUserID == patientId);

            if (patient == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Feedback feedback = new Feedback
            {
                RecordID = model.RecordID,
                Rating = model.Rating,
                Comments = model.Comments,
                FeedbackGiver = model.FeedbackGiver,
                PatientID = patient.PatientID,
                PractitionerID = model.PractitionerID,
                CreatedAt = DateTime.Now,
            };
            if (ModelState.IsValid)
            {
               
                try
                {
                    dbContext.Feedbacks.Add(feedback);
                    dbContext.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine("Outer Exception: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        if (ex.InnerException.InnerException != null)
                        {
                            Debug.WriteLine("Inner Inner Exception: " + ex.InnerException.InnerException.Message);
                        }
                    }
                    throw;
                }




                if (Request.IsAjaxRequest()) // Check if this is an AJAX request
                {
                    return Json(new { success = true });
                }

                return RedirectToAction("ReviewRecords");
            }

            ViewBag.XrayRecordId = feedback.RecordID; // Keep the XrayRecordId in case of a validation error.
            if (Request.IsAjaxRequest()) // Check if this is an AJAX request
            {
                return Json(new { success = false, message = "Validation failed." });
            }

            return View(feedback);
        }


    }

}