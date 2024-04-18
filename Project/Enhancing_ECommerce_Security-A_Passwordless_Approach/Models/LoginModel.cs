using System.ComponentModel.DataAnnotations;

namespace Enhancing_ECommerce_Security_A_Passwordless_Approach.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }
    }
}