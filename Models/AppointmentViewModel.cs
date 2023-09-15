using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XRayHub.Models
{
    public class AppointmentViewModel
    {
        public DateTime DateScheduled { get; set; }
        public int PractitionerID { get; set; }
        public string Reason { get; set; }
        // ... Add other properties as necessary.
    }

}