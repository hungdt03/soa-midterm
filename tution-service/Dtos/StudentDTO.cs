using System.ComponentModel.DataAnnotations;

namespace tution_service.Dtos
{
    public class StudentDTO
    {
        [Required(ErrorMessage = "Mã số sinh viên là bắt buộc")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "MSSV phải gồm đủ 8 kí tự")]
        public string StudentCode { get; set; }

        [Required(ErrorMessage = "Họ tên sinh viên là bắt buộc")]
        public string FullName { get; set; }
    }
}
