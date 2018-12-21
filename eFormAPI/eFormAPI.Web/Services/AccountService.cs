﻿using System.Linq;
using System.Threading.Tasks;
using eFormAPI.Web.Abstractions;
using eFormAPI.Web.Infrastructure.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.WritableOptions;
using Microting.eFormApi.BasePn.Infrastructure.Models.Application;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormApi.BasePn.Infrastructure.Models.Auth;
using System.Collections.Generic;
using eFormAPI.Web.Infrastructure.Models.Settings.User;

namespace eFormAPI.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IWritableOptions<ApplicationSettings> _appSettings;
        private readonly ILogger<AccountService> _logger;
        private readonly ILocalizationService _localizationService;
        private readonly UserManager<EformUser> _userManager;

        public AccountService(UserManager<EformUser> userManager,
            IUserService userService,
            IWritableOptions<ApplicationSettings> appSettings, 
            ILogger<AccountService> logger,
            ILocalizationService localizationService, 
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userService = userService;
            _appSettings = appSettings;
            _logger = logger;
            _localizationService = localizationService;
            _emailSender = emailSender;
        }

        public async Task<UserInfoViewModel> GetUserInfo()
        {
            EformUser user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return null;
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault();
            return new UserInfoViewModel
            {
                Email = user.Email,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role
            };
        }

        public async Task<OperationDataResult<UserSettingsModel>> GetUserSettings()
        {
            EformUser user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return new OperationDataResult<UserSettingsModel>(false, _localizationService.GetString("UserNotFound"));
            }

            string locale = user.Locale;
            if (string.IsNullOrEmpty(locale))
            {
                locale = _appSettings.Value.DefaultLocale;
                if (locale == null)
                {
                    locale = "en-US";
                }
            }

            return new OperationDataResult<UserSettingsModel>(true, new UserSettingsModel()
            {
                Locale = locale
            });
        }

        public async Task<OperationResult> UpdateUserSettings(UserSettingsModel model)
        {
            EformUser user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return new OperationResult(false, _localizationService.GetString("UserNotFound"));
            }

            user.Locale = model.Locale;
            IdentityResult updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new OperationResult(false,
                    $"Error while updating user settings: {string.Join(", ", updateResult.Errors.Select(x => x.ToString()).ToArray())}");
            }

            return new OperationResult(true);
        }

        public async Task<OperationResult> ChangePassword(ChangePasswordModel model)
        {
            IdentityResult result = await _userManager.ChangePasswordAsync(
                await _userService.GetCurrentUserAsync(),
                model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return new OperationResult(false, string.Join(" ", result.Errors));
            }

            return new OperationResult(true);
        }

        public async Task<OperationResult> ForgotPassword(ForgotPasswordModel model)
        {
            EformUser user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new OperationResult(false, $"User with {model.Email} not found");
            }

            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            string link = _appSettings.Value.SiteLink;
            link = $"{link}/auth/restore-password-confirmation?userId={user.Id}&code={code}";
            await _emailSender.SendEmailAsync(user.Email, "EForm Password Reset",
                "Please reset your password by clicking <a href=\"" + link + "\">here</a>");
            return new OperationResult(true);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("reset-admin-password")]
        public async Task<OperationResult> ResetAdminPassword(string code)
        {
            string securityCode = _appSettings.Value.SecurityCode;
            if (string.IsNullOrEmpty(securityCode))
            {
                return new OperationResult(false, _localizationService.GetString("PleaseSetupSecurityCode"));
            }

            string defaultPassword = _appSettings.Value.DefaultPassword;
            if (code != securityCode)
            {
                return new OperationResult(false, _localizationService.GetString("InvalidSecurityCode"));
            }

            IList<EformUser> users = await _userManager.GetUsersInRoleAsync(EformRole.Admin);
            EformUser user = users.FirstOrDefault();

            if (user == null)
            {
                return new OperationResult(false, _localizationService.GetString("AdminUserNotFound"));
            }

            IdentityResult removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                return new OperationResult(false,
                    _localizationService.GetString("ErrorWhileRemovingOldPassword") + ". \n" +
                    string.Join(" ", removeResult.Errors));
            }

            IdentityResult addPasswordResult = await _userManager.AddPasswordAsync(user, defaultPassword);
            if (!addPasswordResult.Succeeded)
            {
                return new OperationResult(false,
                    _localizationService.GetString("ErrorWhileAddNewPassword") + ". \n" +
                    string.Join(" ", addPasswordResult.Errors));
            }

            return new OperationResult(true, _localizationService.GetString("YourEmailPasswordHasBeenReset", user.Email));
        }

        public async Task<OperationResult> ResetPassword(ResetPasswordModel model)
        {
            EformUser user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                return new OperationResult(false);
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return new OperationResult(true);
            }

            return new OperationResult(false, string.Join(" ", result));
        }
    }
}