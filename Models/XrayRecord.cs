namespace XRayHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("XrayRecord")]
    public partial class XrayRecord
    {
        [Key]
        public int RecordID { get; set; }

        public int PatientID { get; set; }

        [Required]
        [StringLength(255)]
        public string TypeOfXray { get; set; }

        public byte[] XrayImage { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
