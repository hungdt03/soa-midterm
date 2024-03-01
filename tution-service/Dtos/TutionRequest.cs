using System.ComponentModel.DataAnnotations;

namespace tution_service.Dtos
{
    public class TutionRequest
    {
        [Required(ErrorMessage = "The tution's amount is required")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "The tution's code is required")]
        public string TutionCode { get; set; }

        [Required(ErrorMessage = "The tution's start is required")]
        public DateTime StartAt { get; set; }

        [Required(ErrorMessage = "The tution's end is required")]
        public DateTime EndAt { get; set; }

        [Required(ErrorMessage = "The tution's description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The tution's studentId is required")]
        public int StudentId { get; set; }
    }
}
