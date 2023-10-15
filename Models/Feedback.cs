using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XRayHub.Models
{
    [Table("Feedback")]
    public partial class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        public int PatientID { get; set; }

        // New columns
        public int? AppointmentID { get; set; }  // Nullable, as it's optional in the table definition
        public int? RecordID { get; set; }  // Nullable, as it's optional in the table definition

        public int PractitionerID { get; set; }


        public byte Rating { get; set; }

        public string Comments { get; set; }
        public DateTime? CreatedAt { get; set; }

        [Required]
        [StringLength(255)]
        public string FeedbackGiver { get; set; }

        // Navigation properties for related entities
        public virtual Patient Patient { get; set; }
        public virtual Appointment Appointment { get; set; }  
        public virtual XrayRecord XrayRecord { get; set; } 

        public virtual MedicalPractitioner MedicalPractitioner { get; set; }
    }
}
