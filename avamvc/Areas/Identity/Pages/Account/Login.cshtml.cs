// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace avamvc.Areas.Identity.Pages.Account {
    public class LoginModel : PageModel {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel {
            [Required(ErrorMessage = "�п�J�b����Email!")]
            [Display(Name = "�b����Email")]
            public string LoginIdentifier { get; set; }  // �i��J�b����Email

            [Required(ErrorMessage = "�п�J�K�X!")]
            [DataType(DataType.Password)]
            [Display(Name = "�K�X")]
            public string Password { get; set; }

            [Display(Name = "�O���")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null) {
            if (!string.IsNullOrEmpty(ErrorMessage)) {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // �M���~���n�J�� cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid) {
                IdentityUser user = null;

                // �P�_��J�O���O Email �榡
                if (new EmailAddressAttribute().IsValid(Input.LoginIdentifier)) {
                    // �� Email ��ϥΪ�
                    user = await _userManager.FindByEmailAsync(Input.LoginIdentifier);
                } else {
                    // �� UserName ��ϥΪ�
                    user = await _userManager.FindByNameAsync(Input.LoginIdentifier);
                }

                if (user == null) {
                    ModelState.AddModelError(string.Empty, "�ϥΪ̤��s�b�C");
                    return Page();
                }

                // ���ұK�X�õn�J
                var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded) {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut) {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                } else {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // �p�G���ҥ��ѡA���s��ܭ���
            return Page();
        }
    }
}
