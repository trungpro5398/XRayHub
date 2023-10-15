namespace XRayHub.Models
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("XrayRecord")]
    public partial class XrayRecord
    {
        [Key]
        public int RecordID { get; set; }

        public int PatientID { get; set; }

        public int TypeID { get; set; }

        public int? PractitionerID { get; set; }  // Making it nullable. Remove '?' if it's required.

        public byte[] XrayImage { get; set; }

        [StringLength(255)]
        public string XrayImagePath { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual Patient Patient { get; set; }

        public virtual MedicalPractitioner Practitioner { get; set; }
       
        public virtual XrayType XrayType { get; set; }
        // Other properties...
    }
}
