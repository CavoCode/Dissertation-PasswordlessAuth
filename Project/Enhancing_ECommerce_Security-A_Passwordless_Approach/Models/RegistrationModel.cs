using System.ComponentModel.DataAnnotations;

namespace Enhancing_ECommerce_Security_A_Passwordless_Approach.Models
{
    public class RegistrationModel
    {
        [Required]
        [Display(Name = "Name")]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }
        [Required]
        [Display(Name = "Device Name")]
        public required string DeviceName { get; set; }
    }
}
