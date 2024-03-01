using ibanking_server.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ibanking_server.Models
{
    public class OTP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(6)]
        public string OTPCode {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public OTPStatus OTPStatus {  get; set; } 
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
