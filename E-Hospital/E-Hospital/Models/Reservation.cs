using System.ComponentModel;

namespace E_Hospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Reservation
    {
        public string Id { get; set; }

        [Column(TypeName = "date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "You must select a date!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [StringLength(128)]
        public string PatientId { get; set; }

        [Required]
        [StringLength(128)]
        public string DoctorId { get; set; }

        public virtual Doctor Doctor { get; set; }

        //[DisplayName("Doctor's Name")]
        //public virtual string DoctorFullName { get; set; }
    }
}
