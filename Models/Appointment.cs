namespace XRayHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Appointment")]
    public partial class Appointment
    {
        public int AppointmentID { get; set; }

        public int PatientID { get; set; }

        public int PractitionerID { get; set; }

        public DateTime DateScheduled { get; set; }

        [Required]
        [StringLength(255)]
        public string TypeOfXray { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public string Reason { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual MedicalPractitioner MedicalPractitioner { get; set; }
    }
}
