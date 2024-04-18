using Enhancing_ECommerce_Security_A_Passwordless_Approach.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Dtos;
using Rsk.AspNetCore.Fido.Stores;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Enhancing_ECommerce_Security_A_Passwordless_Approach.Data;
using NuGet.Common;
using Enhancing_ECommerce_Security_A_Passwordless_Approach.Services;
using System.Drawing.Printing;
using System.Diagnostics;
using IEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender;

namespace Enhancing_ECommerce_Security_A_Passwordless_Approach.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IFidoAuthentication _fido;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegistrationModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IFidoKeyStore _keyStore;
        public AuthenticationController(IFidoKeyStore keyStore, IFidoAuthentication fido, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RegistrationModel> logger,
            IEmailSender emailSender)
        {
            this._keyStore = keyStore ?? throw new ArgumentNullException(nameof(keyStore));
            this._fido = fido ?? throw new ArgumentNullException(nameof(fido));
            this._signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }

        public IActionResult StartRegistration() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterFido(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { Name = model.Name, UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    // initalise registration process
                    var challenge = await _fido.InitiateRegistration(model.Email, model.DeviceName);
                    // challenge the device
                    return View(challenge.ToBase64Dto());
                }

                foreach (var error in result.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
                    return View("StartRegistration", model);
                }
            return View("StartRegistration", model);

        }

        [HttpPost]
        public async Task<IActionResult> CompleteRegistration(
            [FromQuery] string userName,
            [FromBody] Base64FidoRegistrationResponse registrationResponse)
        {
            var result = await _fido.CompleteRegistration(registrationResponse.ToFidoResponse());
            if (result.IsError)
            {
                var user = await _userManager.FindByEmailAsync(userName);
                var res = await _userManager.DeleteAsync(user);


                return BadRequest(result.ErrorDescription);
            }

            return Ok();
        }

        public IActionResult StartLogin() => View();

        public async Task<IActionResult> LoginTOTP(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GenerateUserTokenAsync(
                    user, "TOTPLoginTokenProvider", "PasswordlessLogin");

                    Debug.WriteLine($"Chosen userId: {user.Id.ToString()}");
                    var loginUrl = Url.Content("~/Authentication/VerifyTOTPLoginFido?token=" + token + "&userId=" + user.Id.ToString());
                    Debug.WriteLine($"Here is the constructed URL: {loginUrl}");
                    loginUrl = $"{Request.Scheme}://{Request.Host}{loginUrl}";
                    Debug.WriteLine($"Here is the constructed encoded URL: {HtmlEncoder.Default.Encode(loginUrl)}");
                    await _emailSender.SendEmailAsync(user.Email.ToString(), "Complete your login", $"Please login using this <a href='{HtmlEncoder.Default.Encode(loginUrl)}'>magic link</a>.");
                    
                    return View();
                }
                ModelState.AddModelError("", "Invalid user");
                return View("StartLogin", model);
            }
            return View("StartLogin", model);

        }

        public async Task<IActionResult> VerifyTOTPLoginFido()
        {
            string userId = Request.Query["userId"];
            string token = Request.Query["token"];

            // Optionally, log the extracted values for debugging
            Debug.WriteLine($"Extracted userId: {userId}");
            Debug.WriteLine($"Extracted token: {token}");

            var user = await _userManager.FindByIdAsync(userId);
            Debug.WriteLine($"Attempting to find user: {user}");
            if (user != null)
            {
                var succeeded = await _userManager.VerifyUserTokenAsync(
                    user, "TOTPLoginTokenProvider", "PasswordlessLogin", token);
                Debug.WriteLine(succeeded);
                if (succeeded)
                {
                    var ids = (await _keyStore.GetCredentialIdsForUser(user.Email))?.ToList();

                    if (ids != null || ids.Count != 0)
                    {
                        var challenge = await _fido.InitiateAuthentication(user.Email);

                        return View(challenge.ToBase64Dto());
                    }
                }
            }
            return View("StartLogin");
        }

        [HttpPost]
        public async Task<IActionResult> CompleteLogin(
             [FromBody] Base64FidoAuthenticationResponse authenticationResponse)
        {
            var authenticationResult = await _fido.CompleteAuthentication(authenticationResponse.ToFidoResponse());

            if (authenticationResult.IsSuccess)
            {
                var user = await _userManager.FindByEmailAsync(authenticationResult.UserId);


                await _signInManager.SignInAsync(user, false);
            }

            if (authenticationResult.IsError) return BadRequest(authenticationResult.ErrorDescription);

            return Ok();

        }

        
            
    }
}
