namespace XRayHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("XrayType")]
    public partial class XrayType
    {
        [Key]
        public int TypeID { get; set; }

        [Required]
        [StringLength(255)]
        public string TypeName { get; set; }

        public string Description { get; set; }
    }
}
