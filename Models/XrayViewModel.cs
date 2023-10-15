using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XRayHub.Models
{
    public class XrayViewModel
    {
        public List<XrayRecord> XrayRecords { get; set; }
        public Feedback UserFeedback { get; set; }  // Assuming one feedback per user for now.
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<XrayType> XrayTypes { get; set; }
        public Dictionary<int, string> PractitionerNames { get; set; }

    }
}
