using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XRayHub.Models
{
    public class PractitionerDashboardViewModel
    {
        public Dictionary<DateTime, int> PatientTrends { get; set; } = new Dictionary<DateTime, int>();
        public Dictionary<string, int> CommonAilments { get; set; } = new Dictionary<string, int>();
        public Dictionary<DateTime, int> AppointmentLoad { get; set; } = new Dictionary<DateTime, int>();

        // Nested class for upcoming appointments
        public class AppointmentViewModel
        {
            public DateTime DateScheduled { get; set; }
            public string PatientName { get; set; }
            public string Reason { get; set; }
        }

        public List<AppointmentViewModel> UpcomingAppointments { get; set; }
    }
}
