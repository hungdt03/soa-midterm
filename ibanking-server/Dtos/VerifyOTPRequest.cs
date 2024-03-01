using System.ComponentModel.DataAnnotations;

namespace ibanking_server.Dtos
{
    public class VerifyOTPRequest
    {
        [Required]
        [StringLength(6)]
        public string Otp { get; set; }

        [Required]
        public int TransactionId { get; set; }
    }
}
