using System.ComponentModel.DataAnnotations;

namespace tution_service.Dtos
{
    public class StudentDTO
    {
        [Required(ErrorMessage = "Student code is required")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Student code must 8 characters")]
        public string StudentCode { get; set; }

        [Required(ErrorMessage = "Student's fullname is required")]
        public string FullName { get; set; }
    }
}
