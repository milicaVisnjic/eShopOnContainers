﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.eShopOnContainers.WebMVC.Models;
using Microsoft.eShopOnContainers.WebMVC.Models.AccountViewModels;
using Microsoft.eShopOnContainers.WebMVC.Services;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.eShopOnContainers.WebMVC.Extensions;

namespace Microsoft.eShopOnContainers.WebMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IIdentityParser<ApplicationUser> _identityParser;
        public AccountController(IIdentityParser<ApplicationUser> identityParser)
        {
            _identityParser = identityParser;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult SignIn(string returnUrl)
        {
            var user = User as ClaimsPrincipal;
            
            //TODO - Not retrieving AccessToken yet
            var token = user.FindFirst("access_token");
            if (token != null)
            {
                ViewData["access_token"] = token.Value;
            }

            return Redirect("/");
        }

        [Authorize]
        public async Task<ActionResult> CallApi()
        {
            var token = (User as ClaimsPrincipal).FindFirst("access_token").Value;

            var client = new HttpClient();
            client.SetBearerToken(token);

            var result = await client.GetStringAsync("apiuri");
            ViewBag.Json = JArray.Parse(result.ToString());

            return View();
        }


        public async Task<ActionResult> Signout()
        {
            await Request.HttpContext.Authentication.SignOutAsync("oidc");
            return Redirect("/");
        }

        public async Task SignoutCleanup(string sid)
        {
            var cp = (ClaimsPrincipal)User;
            var sidClaim = cp.FindFirst("sid");
            if (sidClaim != null && sidClaim.Value == sid)
            {
                await Request.HttpContext.Authentication.SignOutAsync("Cookies");
            }
        }

        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly ILogger _logger;

        //public AccountController(
        //    UserManager<ApplicationUser> userManager,
        //    SignInManager<ApplicationUser> signInManager,
        //    ILoggerFactory loggerFactory)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //    _logger = loggerFactory.CreateLogger<AccountController>();
        //}

        ////
        //// GET: /Account/Login
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult Login(string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View();
        //}

        ////
        //// POST: /Account/Login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    if (ModelState.IsValid)
        //    {
        //        // This doesn't count login failures towards account lockout
        //        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {
        //            _logger.LogInformation(1, "User logged in.");
        //            return RedirectToLocal(returnUrl);
        //        }
        //        if (result.RequiresTwoFactor)
        //        {
        //            return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //        }
        //        if (result.IsLockedOut)
        //        {
        //            _logger.LogWarning(2, "User account locked out.");
        //            return View("Lockout");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //            return View(model);
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/Register
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult Register(string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View();
        //}

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser
        //        {
        //            UserName = model.Email,
        //            Email = model.Email,
        //            CardHolderName = model.User.CardHolderName,
        //            CardNumber = model.User.CardNumber,
        //            CardType = model.User.CardType,
        //            City = model.User.City,
        //            Country = model.User.Country,
        //            Expiration = model.User.Expiration,
        //            LastName = model.User.LastName,
        //            Name = model.User.Name,
        //            Street = model.User.Street, 
        //            State = model.User.State, 
        //            ZipCode = model.User.ZipCode, 
        //            PhoneNumber = model.User.PhoneNumber, 
        //            SecurityNumber = model.User.SecurityNumber
        //        };
        //        var result = await _userManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
        //            // Send an email with this link
        //            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
        //            //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
        //            //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            _logger.LogInformation(3, "User created a new account with password.");
        //            return RedirectToLocal(returnUrl);
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LogOff()
        //{
        //    await _signInManager.SignOutAsync();
        //    _logger.LogInformation(4, "User logged out.");
        //    return RedirectToAction(nameof(CatalogController.Index), "Catalog");
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public IActionResult ExternalLogin(string provider, string returnUrl = null)
        //{
        //    // Request a redirect to the external login provider.
        //    var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //    return Challenge(properties, provider);
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        //{
        //    if (remoteError != null)
        //    {
        //        ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
        //        return View(nameof(Login));
        //    }
        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        return RedirectToAction(nameof(Login));
        //    }

        //    // Sign in the user with this external login provider if the user already has a login.
        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        //    if (result.Succeeded)
        //    {
        //        _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    if (result.RequiresTwoFactor)
        //    {
        //        return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        return View("Lockout");
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then ask the user to create an account.
        //        ViewData["ReturnUrl"] = returnUrl;
        //        ViewData["LoginProvider"] = info.LoginProvider;
        //        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await _signInManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await _userManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await _userManager.AddLoginAsync(user, info);
        //            if (result.Succeeded)
        //            {
        //                await _signInManager.SignInAsync(user, isPersistent: false);
        //                _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View(model);
        //}

        //// GET: /Account/ConfirmEmail
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await _userManager.ConfirmEmailAsync(user, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        ////
        //// GET: /Account/ForgotPassword
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
        //        // Send an email with this link
        //        //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
        //        //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
        //        //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
        //        //return View("ForgotPasswordConfirmation");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPassword
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ResetPassword(string code = null)
        //{
        //    return code == null ? View("Error") : View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await _userManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
        //    }
        //    var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/SendCode
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        //{
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// GET: /Account/VerifyCode
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes.
        //    // If a user enters incorrect codes for a specified amount of time then the user account
        //    // will be locked out for a specified amount of time.
        //    var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToLocal(model.ReturnUrl);
        //    }
        //    if (result.IsLockedOut)
        //    {
        //        _logger.LogWarning(7, "User account locked out.");
        //        return View("Lockout");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Invalid code.");
        //        return View(model);
        //    }
        //}

        //#region Helpers

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }
        //}

        //private Task<ApplicationUser> GetCurrentUserAsync()
        //{
        //    return _userManager.GetUserAsync(HttpContext.User);
        //}

        //private IActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction(nameof(CatalogController.Index), "Catalog");
        //    }
        //}

        //#endregion
    }
}
