using System.ComponentModel.DataAnnotations;

namespace tution_service.Dtos
{
    public class TutionRequest
    {
        [Required(ErrorMessage = "Chưa nhập số tiền học phí")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Mã học phí là bắt buộc")]
        public string TutionCode { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu đóng học phí là bắt buộc")]
        public DateTime StartAt { get; set; }

        [Required(ErrorMessage = "Thời hạn đóng học phí là bắt buộc")]
        public DateTime EndAt { get; set; }

        [Required(ErrorMessage = "Thông tin mô tả học phí là bắt buộc")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Mã số sinh viên là bắt buộc")]
        public string StudentCode { get; set; }
    }
}
