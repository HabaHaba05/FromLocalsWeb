using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using FromLocalsToLocals.Utilities;
using FromLocalsToLocals.ViewModels;
using Geocoding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NToastNotify;
using SendGrid;
using SendGrid.Helpers.Mail;
using SuppLocals;
using SendGridAccount = FromLocalsToLocals.Utilities.SendGridAccount;

namespace FromLocalsToLocals.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly SendGridAccount _userOptions;
        private readonly IStringLocalizer<AccountController> _localizer;

        delegate bool MyPredicate<in T>(T arg);

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                 AppDbContext context, IToastNotification toastNotification,
                                 IOptions<SendGridAccount>  userOptions, IStringLocalizer<AccountController> localizer)
        {
            _localizer = localizer;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _toastNotification = toastNotification;
            _userOptions = userOptions.Value;
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = model.Email,
                    UserName = model.Username
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }

                CheckForErrors(new List<IdentityResult>() { result });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }
                ModelState.AddModelError(string.Empty, _localizer["Invalid Login Attempt"]);
            }

                return View(model);
            }
            catch(Exception e)
            {
                await e.ExceptionSender();
                return View("Error");
            }
        }


        public IActionResult Profile()
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Single(x => x.Id == userId);

            var model = GetNewProfileVM(user);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(string submitBtn, ProfileVM model)
        {
            return submitBtn switch
            {
                "picName" => await PicChange(model),
                "accDetails" => await AccountDetailsChange(model),
                "password" => await ChangePassword(model),
                _ => View(),
            };
        }

        private async Task<IActionResult> AccountDetailsChange(ProfileVM model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            var oldModel = GetNewProfileVM(user);
            var resultsList = new List<IdentityResult>();

            if (model.UserName != user.UserName)
            {
                if (string.IsNullOrWhiteSpace(model.UserName))
                {
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.UserName)).Value.RawValue = user.UserName;
                    ModelState.AddModelError("", _localizer[$"Username cannot be empty!"]);
                }
                else if (!_context.Users.Any(x => x.UserName == model.UserName))
                {
                    var resUser = await _userManager.SetUserNameAsync(user, model.UserName);
                    resultsList.Add(resUser);
                }
                else
                {
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.UserName)).Value.RawValue = user.UserName;
                    ModelState.AddModelError("", _localizer[$"Username '{model.UserName}' is already taken."]);
                }
            }
            if (model.Email != user.Email)
            {
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.Email)).Value.RawValue = user.Email;
                    ModelState.AddModelError("", $"Email cannot be empty");
                }
                else if (!_context.Users.Any(x => x.Email == model.Email))
                {
                    var resEmail = await _userManager.SetEmailAsync(user, model.Email);
                    resultsList.Add(resEmail);
                }
                else
                {
                    ModelState.FirstOrDefault(x => x.Key == nameof(model.Email)).Value.RawValue = user.Email;
                    ModelState.AddModelError("", $"Email '{model.Email}' is already in use.");
                }
            }

            CheckForErrors(resultsList);
            return Profile();
        }

        private async Task<IActionResult> PicChange(ProfileVM model)
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            var oldModel = GetNewProfileVM(user);
            var resultsList = new List<IdentityResult>();

            if (model.ImageFile != null && !model.ImageFile.ValidImage())
            {
                ModelState.AddModelError("", "Invalid profile image");
                _toastNotification.AddErrorToastMessage("Invalid profile image");
                return Profile();
            }

            if (model.ImageFile.ValidImage())
            {
                using (var target = new MemoryStream())
                {
                    model.ImageFile.CopyTo(target);
                    user.Image = target.ToArray();
                    model.Image = user.Image;
                }
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            CheckForErrors(resultsList);
            return Profile();
        }

        private async Task<IActionResult> ChangePassword(ProfileVM model)
        {
            MyPredicate<string[]> StringArrNull = (s) =>
            {
                foreach (var i in s)
                {
                    if (string.IsNullOrEmpty(i))
                    {
                        return true;
                    }
                }
                return false;
            };

            Func<string, Func<bool>, bool> InvalidPassword = (err, action) =>
              {
                  if (action())
                  {
                      ModelState.AddModelError("", err);
                      return true;
                  }
                  return false;
              };

            if (InvalidPassword("Please, fill all fields",
                                () => { return StringArrNull(new string[] { model.Password, model.NewPassword, model.ConfirmPassword });} ) ||
                InvalidPassword("Password must be at least 6 characters long", () => { return model.NewPassword.Length < Config.minPasswordLength; }) ||
                InvalidPassword("Passwords do not match", () => { return model.NewPassword != model.ConfirmPassword; })) 
            {
                return Profile();
            }

            var user = await _userManager.GetUserAsync(User);

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Profile();
            }

            await _signInManager.RefreshSignInAsync(user);
            _toastNotification.AddSuccessToastMessage("Password changed successfully");

            return Profile();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Register") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordVM model)
        {
            try
            {
                var isValid = false;


                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction("Register", "Account");
                }

                if (model.ConfirmPassword == model.Password)
                {
                    if (model.ConfirmPassword is null || model.Password is null)
                    {
                        ModelState.AddModelError("", "Please fill both passwords field.");
                        return View();
                    }
                    else
                    {
                        isValid = true;
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Password do not match!");
                    return View();
                }

                if (isValid is true)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ResetPasswordConfirmation", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unexpected error");
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
            catch(Exception e)
            {
                await e.ExceptionSender();
                return View("Error");
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);


                    await Execute();
                    return View("ForgotPasswordConfirmation");
                }
            }
            catch(Exception e)
            {
                await e.ExceptionSender();
                return View("Error");
            }

            async Task Execute()
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var key = Config.Send_Grid_Key;
                var client = new SendGridClient(key);

                var from = new EmailAddress(_userOptions.ReceiverEmail, "Forgot password");
                var subject = "Forgot Password Confirmation";
                var to = new EmailAddress(model.Email, "Dear User");
                var plainTextContent = "";

                var code = await _userManager.GeneratePasswordResetTokenAsync(user); 

             
                var callbackUrl = Url.Action("ResetPassword", "Account",
                new { user = user , code = code }, protocol: Request.Scheme); 

                var htmlContent = "<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body>" +

                                  "Please confirm your account by clicking this link: <a href =\""
                                                 + callbackUrl + "\">link</a> </body></html>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }

            return View();
        }

        #region Helpers
        private void CheckForErrors(List<IdentityResult> results)
        {
            var errors = GetErrors(results);
            if (!errors.IsNullOrEmpty())
            {
                errors.ForEach(e => ModelState.AddModelError("", e));
            }

            if (ModelState.ErrorCount == 0)
            {
                _toastNotification.AddSuccessToastMessage("Changes saved successfully");
            }
        }

        private List<string> GetErrors(List<IdentityResult> results)
        {
            var tempAns = new List<string>();

            foreach (var r in results)
            {
                if (!r.Succeeded)
                {
                    r.Errors.ForEach(x => tempAns.Add(x.Description));
                }
            }
            return tempAns;
        }

        public static ProfileVM GetNewProfileVM(AppUser user)
        {
            return new ProfileVM(user.Email, user.UserName, user.Image, user.Subscribe);
        }
        #endregion


      

    }
}