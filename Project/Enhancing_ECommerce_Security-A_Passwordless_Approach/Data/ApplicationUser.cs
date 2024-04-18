using Microsoft.AspNetCore.Identity;
namespace Enhancing_ECommerce_Security_A_Passwordless_Approach.Data
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set; } // Add the full name property
    }
}
