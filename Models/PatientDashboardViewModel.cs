using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XRayHub.Models
{
    public class Notification
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
    public class PatientDashboardViewModel
    {
        public IEnumerable<Appointment> Appointments { get; set; }
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public List<XrayRecord> XrayTimeline { get; set; }

    }
}