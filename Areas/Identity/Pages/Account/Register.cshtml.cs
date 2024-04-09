// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EduSchool.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using EduSchool.Models.Email;
using EduSchool.Models.DataModel;
using EduSchool.Models.Context;

namespace EduSchool.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly Models.Email.IEmailSender _emailSender;
        private readonly EduContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            Models.Email.IEmailSender emailSender,
            EduContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Az email cím megadása kötelező.")]
            [EmailAddress(ErrorMessage = "Az email cím formátuma nem megfelelő.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "A jelszó megadása kötelező.")]
            [StringLength(100, ErrorMessage = "A(z) {0} legalább {2}, legfeljebb pedig {1} karakter hosszú kell legyen.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Jelszó")]
            public string Password { get; set; }


            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Jelszó megerősítése")]
            [Compare("Password", ErrorMessage = "A két jelszó nem egyezik.")]
            public string ConfirmPassword { get; set; }

            //Username
            [Required]
            [StringLength(100, ErrorMessage = "A {0} legalább {2} és legfeljebb {1} karakterből állhat.", MinimumLength = 3)]
            [Display(Name = "Username")]
            public string Username { get; set; }

            //FirstName
            [Required]
            [StringLength(100, ErrorMessage = "A {0} legalább {2} és legfeljebb {1} karakterből állhat.", MinimumLength = 3)]
            [Display(Name = "Keresztnév")]
            public string FirstName { get; set; }

            //LastName
            [Required]
            [StringLength(100, ErrorMessage = "A {0} legalább {2} és legfeljebb {1} karakterből állhat.", MinimumLength = 3)]
            [Display(Name = "Vezetéknév")]
            public string LastName { get; set; }

            //UserType
            [Required]
            [Display(Name = "Felhasználó típusa")]
            public UserType UserType { get; set; }

            //contactPhoneNumber
            [Required]
            [Display(Name = "Telefonszám")]
            public string ContactPhoneNumber { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    var userToCreate = new User
                    {
                        UserID = user.Id,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        UserType = Input.UserType,
                        ContactEmail = Input.Email,
                        ContactPhoneNumber = Input.ContactPhoneNumber,
                    };

                    _context.Users.Add(userToCreate);
                    await _context.SaveChangesAsync();


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        #region emailhtml
                        await _emailSender.SendEmailAsync(Input.Email, "Erősítsd meg az EduSchool regisztrációd",
        $@"
    <!DOCTYPE html>
<html lang=""hu"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f7f7f7;
            padding: 20px;
        }}
        .container {{
            max-width: 600px;
            margin: auto;
            background: linear-gradient(135deg, #ff9e2c, #ff6b6b);
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
            color: #fff;
            text-align: center;
        }}
        h2 {{
            font-size: 28px;
            margin-bottom: 20px;
            text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.3);
        }}
        p {{
            font-size: 16px;
            margin-bottom: 30px;
        }}
        .btn {{
            display: inline-block;
            background-color: #fff;
            color: #ff6b6b;
            text-decoration: none;
            padding: 15px 30px;
            border-radius: 30px;
            font-weight: bold;
            transition: all 0.3s ease;
            box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
        }}
        .btn:hover {{
            transform: translateY(-3px);
            box-shadow: 0 8px 15px rgba(0, 0, 0, 0.3);
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h2>Erősítsd meg az EduSchool regisztrációd</h2>
        <p>Kedves {Input.Username}! Üdvözlünk az EduSchool rendszerben! Kattints az alábbi gombra, hogy megerősítsd a regisztrációdat:</p>
        <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' class=""btn"">FIÓK MEGERŐSÍTÉSE</a>
    </div>
</body>
</html>
    ");
                        #endregion
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
