using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XRayHub.Models;
using Microsoft.AspNet.Identity; // for User.Identity.GetUserId()
using System.Diagnostics;

namespace XRayHub.Controllers
{
    [Authorize]
    public class XrayController : Controller
    {
        private XRayHub_Models dbContext = new XRayHub_Models();

        // GET: Xray/UploadXray
        public ActionResult UploadXray()
        {
            var model = new UploadXrayViewModel
            {
                Patients = GetPatients(),
                XrayTypes = GetXrayTypes(),
                Appointments = GetAppointments()
            };

            return View(model);
        }

        // POST: Xray/UploadXray
        [HttpPost]
        public ActionResult UploadXray(UploadXrayViewModel model)
        {
            Debug.WriteLine(ModelState.IsValid);

            if (ModelState.IsValid)
            {
                if (model.XrayImage != null && model.XrayImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(model.XrayImage.FileName);
                    Debug.WriteLine(fileName);

                    var path = Path.Combine(Server.MapPath("~/UploadedImages/"), fileName);
                    model.XrayImage.SaveAs(path);

                    var practitionerId = User.Identity.GetUserId();


                    var practitioner = dbContext.MedicalPractitioners
                                               .FirstOrDefault(p => p.IdentityUserID == practitionerId);
                    var xRayRecord = new XrayRecord
                    {
                        PractitionerID = practitioner.PractitionerID,
                        PatientID = model.PatientId,
                        TypeID = model.TypeId,
                        XrayImagePath = "~/UploadedImages/" + fileName,
                        CreatedAt = DateTime.Now,  // if you want to set this explicitly despite having a default in SQL
                        UpdatedAt = DateTime.Now ,  // likewise, though for updates this might be set in an Update method instead
                        Description = model.Description
                    };


                    dbContext.XrayRecords.Add(xRayRecord);
                    dbContext.SaveChanges();

                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError("XrayImage", "An image is required.");
                }
            }

            model.Patients = GetPatients();
            model.XrayTypes = GetXrayTypes();
            model.Appointments = GetAppointments();

            return View(model);
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

        private IEnumerable<SelectListItem> GetAppointments()
        {
            var practitionerId = User.Identity.GetUserId();
            var practitioner = dbContext.MedicalPractitioners
                                       .FirstOrDefault(p => p.IdentityUserID == practitionerId);

            // This ensures we don't try to operate on a null practitioner.
            if (practitioner == null)
            {
                return new List<SelectListItem>();
            }

            // First retrieve the data from the database, then perform the conversion.
            var appointments = dbContext.Appointments
                                       .Where(a => a.PractitionerID == practitioner.PractitionerID)
                                       .ToList()  // Materialize the query here
                                       .Select(a => new SelectListItem
                                       {
                                           Value = a.AppointmentID.ToString(),
                                           Text = a.DateScheduled.ToString("yyyy-MM-dd HH:mm") // Now safe to use ToString
                                       })
                                       .ToList();

            return appointments ?? new List<SelectListItem>();
        }

    }
}
