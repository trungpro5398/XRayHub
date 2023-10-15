using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace XRayHub.Models
{
    public class UploadXrayViewModel
    {
        public IEnumerable<SelectListItem> Patients { get; set; }
        public IEnumerable<SelectListItem> XrayTypes { get; set; }
        public IEnumerable<SelectListItem> Appointments { get; set; }

        // Additional properties related to Xray, e.g., TypeId, PatientId, etc.
        // Your uploaded image will be of type HttpPostedFileBase if using MVC 5 and earlier.
        public HttpPostedFileBase XrayImage { get; set; }
        public int PatientId { get; set; }
        public int TypeId { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
    }
}
