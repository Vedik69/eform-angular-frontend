﻿using System;
using System.Configuration;
using Amazon.Runtime.Internal.Util;
using eFormAPI.Common.Models.Settings;
using eFormAPI.Database.Entities;
using Microsoft.AspNetCore.Identity;

namespace eFormAPI.Core.Helpers
{
    public class SettingsHelper
    {
        private string _connectionString;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SettingsHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateAdminUser(AdminSetupModel adminSetupModel)
        {
            if (_connectionString == null)
            {
                _connectionString = ConfigurationManager.ConnectionStrings["eFormMainConnection"].ConnectionString;
            }
            // Seed admin and demo users
            var manager = new EformUserManager(new EformUserStore(new ApplicationDbContext.BaseDbContext(_connectionString)));
            var adminUser = new EformUser()
            {
                UserName = adminSetupModel.UserName,
                Email = adminSetupModel.Email,
                FirstName = adminSetupModel.FirstName,
                LastName = adminSetupModel.LastName,
                EmailConfirmed = true,
                TwoFactorEnabled = false,
                IsGoogleAuthenticatorEnabled = false
            };
            if (!manager.Users.Any(x => x.Email.Equals(adminUser.Email)))
            {
                manager.UserValidator = new UserValidator<EformUser, int>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };
                IdentityResult ir = manager.Create(adminUser, adminSetupModel.Password);
                if (ir != null)
                {
                    manager.AddToRole(adminUser.Id, "admin");
                }
                else
                {
                    throw new Exception("Could not create the user");
                }

            }
        }

        //public static bool GetTwoFactorAuthForceInfo()
        //{
        //    try
        //    {

        //        var configuration = WebConfigurationManager.OpenWebConfiguration("~");
        //        var section = (AppSettingsSection)configuration.GetSection("appSettings");
        //        return section.Settings["auth:isTwoFactorForced"].Value.Equals("True");
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(e.Message);
        //    }
        //    return false;
        //}

        //public static void UpdateTwoFactorAuthForceInfo(bool isTwoFactorEnabled)
        //{
        //    try
        //    {
        //        var configuration = WebConfigurationManager.OpenWebConfiguration("~");
        //        var section = (AppSettingsSection)configuration.GetSection("appSettings");
        //        section.Settings["auth:isTwoFactorForced"].Value = isTwoFactorEnabled.ToString();
        //        configuration.Save();
        //        ConfigurationManager.RefreshSection("appSettings");
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(e.Message);
        //    }
        //}
    }
}