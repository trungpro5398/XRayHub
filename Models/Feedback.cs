namespace XRayHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Feedback")]
    public partial class Feedback
    {
        public int FeedbackID { get; set; }

        public int PatientID { get; set; }

        [Required]
        [StringLength(255)]
        public string TypeOfXray { get; set; }

        public byte Rating { get; set; }

        public string Comments { get; set; }

        [Required]
        [StringLength(255)]
        public string FeedbackGiver { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
