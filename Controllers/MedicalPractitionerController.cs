using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XRayHub.Models;
using System.Data.Entity;

namespace XRayHub.Controllers
{
    [Authorize]
    public class MedicalPractitionerController : Controller
    {
        private XRayHub_Models dbContext = new XRayHub_Models();

        public string GetPractitionerName(int patientID)
        {
            var patient = dbContext.Patients
                    .FirstOrDefault(p => p.PatientID == patientID);

            if (patient != null)
            {
                return patient.FirstName + " " + patient.LastName;
            }
            else
            {
                throw new Exception($"Practitioner with ID {patientID} not found.");
            }
        }

        public ActionResult Dashboard()
        {
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            var userId = User.Identity.GetUserId(); // Get current user's ID

            if (string.IsNullOrEmpty(userId))
            {
                // Handle unauthenticated/unauthorized access appropriately
                return RedirectToAction("Login", "Account");
            }
            var practitioner = dbContext.MedicalPractitioners.FirstOrDefault(p => p.IdentityUserID == userId);
            var viewModel = new PractitionerDashboardViewModel
            {
                PatientTrends = dbContext.Appointments.GroupBy(a => DbFunctions.TruncateTime(a.DateScheduled))
                        .ToDictionary(g => (DateTime)g.Key, g => g.Count()),

                CommonAilments = dbContext.Appointments.GroupBy(a => a.Reason)
                        .ToDictionary(g => g.Key, g => g.Count()),

                AppointmentLoad = dbContext.Appointments.Where(a => a.DateScheduled > oneMonthAgo)
                        .GroupBy(a => DbFunctions.TruncateTime(a.DateScheduled))
                        .ToDictionary(g => (DateTime)g.Key, g => g.Count()),



                UpcomingAppointments = dbContext.Appointments
                .Where(a => a.DateScheduled > DateTime.Now && a.PractitionerID == practitioner.PractitionerID)
                .OrderBy(a => a.DateScheduled)
                .Take(10)  // Assuming you want to show only the next 10 appointments
                .Select(a => new PractitionerDashboardViewModel.AppointmentViewModel
                {

                    DateScheduled = a.DateScheduled,
                    PatientName = dbContext.Patients
                        .Where(p => p.PatientID == a.Patient.PatientID)
                        .Select(p => p.FirstName + " " + p.LastName)
                        .FirstOrDefault(),


                    Reason = a.Reason
                }).ToList(),
        };

            return View(viewModel);
        }




        public new ActionResult Profile()
        {
            var userId = User.Identity.GetUserId();
            var practitioner = dbContext.MedicalPractitioners.FirstOrDefault(p => p.IdentityUserID == userId);

            if (practitioner == null)
            {
                return RedirectToAction("Login", "Account");

            }
            return View(practitioner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(MedicalPractitioner updatedProfile)
        {
            // Fetch the current patient
            var currentPractitioner = dbContext.MedicalPractitioners.FirstOrDefault(p => p.PractitionerID == updatedProfile.PractitionerID);

            if (currentPractitioner == null)
            {
                // Handle error (e.g., show an error message or redirect to an error page)
                return RedirectToAction("Error", "Home");  // Redirecting to an "Error" action as an example
            }

            // Update the patient details
            currentPractitioner.FirstName = updatedProfile.FirstName;
            currentPractitioner.LastName = updatedProfile.LastName;
            currentPractitioner.Specialization = updatedProfile.Specialization;
            currentPractitioner.Facility = updatedProfile.Facility;
            currentPractitioner.Email = updatedProfile.Email;

            // Save changes to the database
            dbContext.SaveChanges();

            // Redirect back to the profile page or display a success message
            return RedirectToAction("Profile");
        }

        // GET: MedicalPractitioner/UploadXray
        public ActionResult UploadXray()
        {
            var model = new UploadXrayViewModel
            {
                // Populate Patients and XrayTypes with data from your database here
                Patients = GetPatients(),
                XrayTypes = GetXrayTypes()
            };

            return View( model);
        }

        private IEnumerable<SelectListItem> GetPatients()
        {
            var practitionerId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(practitionerId))
            {
                // Handle unauthenticated/unauthorized access appropriately
                // Redirect, throw an exception, or return an empty list:
                return new List<SelectListItem>();
            }

            var practitioner = dbContext.MedicalPractitioners
                                       .FirstOrDefault(p => p.IdentityUserID == practitionerId);

            if (practitioner == null)
            {
                // Handle the situation when the practitioner is not found
                // Logging or error handling can go here
                // return an empty list or handle differently as per your use case:
                return new List<SelectListItem>();
            }

            // First, get PatientIDs from Appointments related to the Practitioner
            var patientIds = dbContext.Appointments
                                      .Where(a => a.PractitionerID == practitioner.PractitionerID)
                                      .Select(a => a.PatientID)
                                      .Distinct()
                                      .ToList();

            // Next, get Patient details from Patients using IDs retrieved
            var patients = dbContext.Patients
                                    .Where(p => patientIds.Contains(p.PatientID))
                                    .Select(p => new SelectListItem
                                    {
                                        Value = p.PatientID.ToString(),
                                        Text = p.FirstName // Assuming there's a PatientName property
                                    })
                                    .ToList();

            return patients ?? new List<SelectListItem>();
        }


        private IEnumerable<SelectListItem> GetXrayTypes()
        {
            // Example: Getting xray types and creating a SelectList for a dropdown
            return dbContext.XrayTypes.Select(x => new SelectListItem
            {
                Value = x.TypeID.ToString(),
                Text = x.TypeName
            }).ToList();
        }

        // GET: MedicalPractitioner/ReviewAppointments
        public ActionResult ReviewAppointments()
        {
            var practitionerId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(practitionerId))
            {
                // Handle unauthenticated/unauthorized access appropriately
                return RedirectToAction("Login", "Account");
            }

            var practitioner = dbContext.MedicalPractitioners
                                       .FirstOrDefault(p => p.IdentityUserID == practitionerId);

            if (practitioner == null)
            {
                // Handle the situation when the practitioner is not found
                // You might want to log this situation for further analysis
                return View("Error"); // or return a custom error view
            }

            var appointments = dbContext.Appointments
                                       .Where(a => a.PractitionerID == practitioner.PractitionerID)
                                       .ToList()
                                       ?? new List<Appointment>(); // provide an empty list if null is returned
           
            return View(appointments);
        }

        // Action to review records
        public ActionResult ReviewXrayRecords()
        {
            var practitionerId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(practitionerId))
            {
                // Handle unauthenticated/unauthorized access appropriately
                return RedirectToAction("Login", "Account");
            }

            var practitioner = dbContext.MedicalPractitioners
                                       .FirstOrDefault(p => p.IdentityUserID == practitionerId);
            var xrayRecords = dbContext.XrayRecords.Where(x => x.PractitionerID == practitioner.PractitionerID).ToList();

            return View(xrayRecords);
        }
        // Action to fetch X-ray record details for modal
        public ActionResult GetXrayRecord(int? id)
        {
            if (id == null)
            {
                // Sending a bad request status code if id is null
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID cannot be null");
            }

            var xrayRecord = dbContext.XrayRecords.Find(id);

            if (xrayRecord == null)
            {
                // Sending a not found status code if the record is not found
                return HttpNotFound("Record not found");
            }

            // Returning record data as JSON
            return Json(new
            {
                RecordID = xrayRecord.RecordID,
                Description = xrayRecord.Description,
                // Additional fields as needed
            }, JsonRequestBehavior.AllowGet);
        }


        // Action to update X-ray record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateXrayRecord(XrayRecord updatedRecord, HttpPostedFileBase newXrayImage)
        {
            
            try
            {

                if (ModelState.IsValid)
                {
                    var recordToUpdate = await dbContext.XrayRecords.FindAsync(updatedRecord.RecordID);

                    if (recordToUpdate != null)
                    {
                        // Update the description
                        recordToUpdate.Description = updatedRecord.Description;

                        // Handle the new X-ray image if provided
                        if (newXrayImage != null && newXrayImage.ContentLength > 0)
                        {
                            // Generate a unique file name or use the record ID as the file name
                            string fileName = recordToUpdate.RecordID.ToString() + "_" + Path.GetFileName(newXrayImage.FileName);
                            string filePath = Path.Combine(Server.MapPath("~/UploadedImages"), fileName);

                            // Save the new image to the server
                            newXrayImage.SaveAs(filePath);

                            // Update the XrayImagePath in the database
                            recordToUpdate.XrayImagePath = "~/UploadedImages/" + fileName;
                        }

                        await dbContext.SaveChangesAsync();

                       
                    }
                    return RedirectToAction("ReviewXrayRecords");

                }
                else
                {
                    ModelState.AddModelError("", "Record not found."); return RedirectToAction("ReviewXrayRecords");


                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the record.");
                Debug.WriteLine($"Error: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return RedirectToAction("ReviewXrayRecords");

            }
        }
        [HttpPost]
        public JsonResult UpdateAppointmentStatus(int appointmentId, string status)
        {
            Debug.WriteLine(appointmentId);

            try
            {
                var appointment = dbContext.Appointments.Find(appointmentId);
                Debug.WriteLine(appointment);
                Debug.WriteLine(status);

                if (appointment != null)
                {
                    appointment.Status = status;
                    dbContext.SaveChanges();
                    return Json(new { success = true, message = "Status updated successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Appointment not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: MedicalPractitioner/ReviewFeedbacks
        public ActionResult ReviewFeedbacks()
        {
            // Get the ID of the currently logged-in user
            var userId = User.Identity.GetUserId();

            // Ensure the user is logged in
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the associated medical practitioner
            var practitioner = dbContext.MedicalPractitioners.FirstOrDefault(p => p.IdentityUserID == userId);

            // If the practitioner does not exist, redirect to an error page or login
            if (practitioner == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch feedback for the medical practitioner
            var feedbacks = dbContext.Feedbacks
                                     .Where(f => f.PractitionerID == practitioner.PractitionerID)
                                     .OrderByDescending(f => f.CreatedAt) // Latest feedback first
                                     .ToList();

            return View(feedbacks); // This will send the feedback data to the ReviewFeedback.cshtml view
        }

        [HttpGet]
        public ActionResult GetFilteredFeedbacks(DateTime? fromDate, DateTime? toDate, int? patientID)
        {
            var practitionerID = User.Identity.GetUserId();
            var practitioner = dbContext.MedicalPractitioners.FirstOrDefault(p => p.IdentityUserID == practitionerID);
            if (practitioner == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // Fetch the filtered records
            var filteredRecords = dbContext.Feedbacks
                .Where(x => x.PractitionerID == practitioner.PractitionerID)
                .Where(x =>
                    (!fromDate.HasValue || x.CreatedAt >= fromDate.Value) &&
                    (!toDate.HasValue || x.CreatedAt <= toDate.Value) &&
                    (!patientID.HasValue || x.PatientID == patientID.Value)

                )
                .ToList();


            return PartialView("_FeedbackListPartial", filteredRecords);

        }

        [HttpGet]
        public ActionResult FilterAppointments(DateTime? fromDate, DateTime? toDate, int? patientID, string xrayType, string status)
        {
            var practitionerID = User.Identity.GetUserId();
            var practitioner = dbContext.MedicalPractitioners.FirstOrDefault(p => p.IdentityUserID == practitionerID);
            if (practitioner == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Debug.WriteLine("FilterAppointments");

            Debug.WriteLine(status);
            // Fetch the filtered records
            var filteredRecords = dbContext.Appointments
                .Where(x => x.PractitionerID == practitioner.PractitionerID)
                .Where(x =>
                    (!fromDate.HasValue || x.DateScheduled >= fromDate.Value) &&
                    (!toDate.HasValue || x.DateScheduled <= toDate.Value) &&
                    (!patientID.HasValue || x.PatientID == patientID.Value) &&
                    (string.IsNullOrEmpty(xrayType) || x.TypeOfXray == xrayType) &&
                    (string.IsNullOrEmpty(status) || x.Status == status)

                )
                .ToList();


            return PartialView("_AppointmentsListPartial", filteredRecords);

        }
    }
}
