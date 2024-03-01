using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections;

namespace tution_service.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string StudentCode { get; set; }
        [Required]
        public string FullName { get; set; }

        public ICollection<Tution> Tutions { get; set; }
    }
}
