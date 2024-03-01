using System.ComponentModel.DataAnnotations;

namespace authentication_service.Dtos
{
    public class RegistryRequest
    {
        [Required(ErrorMessage = "Fullname is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(\+[0-9]{1,4})?[0-9]{10,15}$", ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
    }
}
