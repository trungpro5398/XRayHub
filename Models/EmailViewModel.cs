using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XRayHub.Models
{
    public class EmailViewModel
    {
        public List<string> Emails { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}