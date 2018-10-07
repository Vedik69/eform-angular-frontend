﻿using System.Threading.Tasks;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormApi.BasePn.Infrastructure.Models.Auth;
using Microting.eFormApi.BasePn.Infrastructure.Models.Settings.User;
using Microting.eFormApi.BasePn.Infrastructure.Models.User;

namespace eFormAPI.Web.Abstractions
{
    public interface IAccountService
    {
        Task<OperationResult> ChangePassword(ChangePasswordModel model);
        Task<OperationResult> ForgotPassword(ForgotPasswordModel model);
        Task<UserInfoViewModel> GetUserInfo();
        Task<OperationDataResult<UserSettingsModel>> GetUserSettings();
        Task<OperationResult> ResetAdminPassword(string code);
        Task<OperationResult> ResetPassword(ResetPasswordModel model);
        Task<OperationResult> UpdateUserSettings(UserSettingsModel model);
    }
}