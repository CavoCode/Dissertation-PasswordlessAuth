using Enhancing_ECommerce_Security_A_Passwordless_Approach.Data;
using Microsoft.AspNetCore.Identity;

namespace Enhancing_ECommerce_Security_A_Passwordless_Approach.Services
{
    public class TOTPLoginTokenProvider : TotpSecurityStampBasedTokenProvider<ApplicationUser>
    {
        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        public override async Task<string> GetUserModifierAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            var emailAddress = await manager.GetEmailAsync(user);
            return $"PasswordlessLogin:{purpose}:{emailAddress}";
        }
    }
}
