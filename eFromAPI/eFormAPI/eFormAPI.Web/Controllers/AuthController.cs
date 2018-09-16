﻿using System.Threading.Tasks;
using eFormAPI.Common.Infrastructure.Helpers;
using eFormAPI.Common.Infrastructure.Models.API;
using eFormAPI.Common.Models.Auth;
using eFormAPI.Core.Abstractions;
using eFormAPI.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eFormAPI.Web.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IUserService userService,
            UserManager<EformUser> userManager,
            IAuthService authService)
        {
            _authService = authService;
        }


        [HttpGet]
        [Route("api/auth/logout")]
        public async Task<OperationResult> Logout()
        {
            var result = await _authService.LogOut();
            if (result.Success)
                Response.Cookies.Delete("Authorization");
            return new OperationResult(true);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/auth/two-factor-info")]
        public OperationDataResult<bool> TwoFactorAuthForceInfo()
        {
            return _authService.TwoFactorAuthForceInfo();
        }

        [HttpGet]
        [Route("api/auth/google-auth-info")]
        public async Task<OperationDataResult<GoogleAuthInfoModel>> GetGoogleAuthenticatorInfo()
        {
            return await _authService.GetGoogleAuthenticatorInfo();
        }

        [HttpPost]
        [Route("api/auth/google-auth-info")]
        public async Task<OperationResult> UpdateGoogleAuthenticatorInfo([FromBody] GoogleAuthInfoModel requestModel)
        {
            return await _authService.UpdateGoogleAuthenticatorInfo(requestModel);
        }

        [HttpDelete]
        [Route("api/auth/google-auth-info")]
        public async Task<OperationResult> DeleteGoogleAuthenticatorInfo()
        {
            return await _authService.DeleteGoogleAuthenticatorInfo();
        }

        /// <summary>
        /// Get secret key and barcode to enable GoogleAuthenticator for account
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/auth/google-auth-key")]
        public async Task<OperationDataResult<GoogleAuthenticatorModel>> GetGoogleAuthenticator([FromBody] LoginModel loginModel)
        {
            // check model
            if (!ModelState.IsValid)
            {
                return new OperationDataResult<GoogleAuthenticatorModel>(false,
                    LocaleHelper.GetString("InvalidUserNameOrPassword"));
            }

            return await _authService.GetGoogleAuthenticator(loginModel);
        }
    }
}