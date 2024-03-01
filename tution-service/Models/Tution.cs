using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using tution_service.Enums;

namespace tution_service.Models
{
    public class Tution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        public double Amount { get; set; }

        [Required]
        public string TutionCode { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartAt { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndAt { get; set; }

        [Required]
        public string Description { get; set; }
        public TutionStatus Status { get; set; }
        public int TransactionId { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

    }
}
