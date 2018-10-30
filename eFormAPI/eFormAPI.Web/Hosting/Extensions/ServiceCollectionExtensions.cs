﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using eFormAPI.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microting.eFormApi.BasePn;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.WritableOptions;
using Newtonsoft.Json.Serialization;

namespace eFormAPI.Web.Hosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var environment = provider.GetService<IHostingEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(environment, options, section.Key, file);
            });
        }

        public static void AddEFormPlugins(this IServiceCollection services,
            List<IEformPlugin> plugins)
        {
            foreach (var plugin in plugins)
            {
                plugin.ConfigureServices(services);
            }
        }

        public static void AddEFormAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = configuration["EformTokenOptions:Issuer"],
                ValidAudience = configuration["EformTokenOptions:Issuer"],
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["EformTokenOptions:SigningKey"]))
            };
            services.AddAuthentication((cfg =>
                {
                    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    cfg.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    cfg.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }))
                .AddCookie(cfg =>
                {
                    cfg.SlidingExpiration = true;
                    cfg.ExpireTimeSpan = TimeSpan.FromHours(10);
                    cfg.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                    cfg.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };
                    cfg.Events.OnRedirectToReturnUrl = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };
                })
                .AddJwtBearer(cfg =>
                {
                    // JWT Bearer
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = tokenValidationParameters;
                });
            services.AddAuthorization(options =>
            {
                // Workers
                options.AddPolicy(AuthConsts.EformPolicies.Workers.Create,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.WorkersClaims.Create,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.Workers.Read,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.WorkersClaims.Read,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.Workers.Update,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.WorkersClaims.Update,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.Workers.Delete,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.WorkersClaims.Delete,
                        AuthConsts.ClaimDefaultValue));
                // Sites
                options.AddPolicy(AuthConsts.EformPolicies.Sites.Read,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.SitesClaims.Read,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.Sites.Update,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.SitesClaims.Update,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.Sites.Delete,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.SitesClaims.Delete,
                        AuthConsts.ClaimDefaultValue));
                // Entity Search
                options.AddPolicy(AuthConsts.EformPolicies.EntitySearch.Create,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EntitySearchClaims.Create,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.EntitySearch.Read,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EntitySearchClaims.Read,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.EntitySearch.Update,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EntitySearchClaims.Update,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.EntitySearch.Delete,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EntitySearchClaims.Delete,
                        AuthConsts.ClaimDefaultValue));
                // Entity Select
                options.AddPolicy(AuthConsts.EformPolicies.EntitySelect.Create,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EntitySelectClaims.Create,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.EntitySelect.Read,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EntitySelectClaims.Read,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.EntitySelect.Update,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EntitySelectClaims.Update,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.EntitySelect.Delete,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EntitySelectClaims.Delete,
                        AuthConsts.ClaimDefaultValue));
                // User management
                options.AddPolicy(AuthConsts.EformPolicies.UserManagement.Create,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.UserManagementClaims.Create,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.UserManagement.Read,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.UserManagementClaims.Read,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.UserManagement.Update,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.UserManagementClaims.Update,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.UserManagement.Delete,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.UserManagementClaims.Delete,
                        AuthConsts.ClaimDefaultValue));
                // Entity Select
                options.AddPolicy(AuthConsts.EformPolicies.Units.Update,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.UnitsClaims.Update,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.Units.Read,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.UnitsClaims.Read,
                        AuthConsts.ClaimDefaultValue));
                // Entity Select
                options.AddPolicy(AuthConsts.EformPolicies.DeviceUsers.Create,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.DeviceUsersClaims.Create,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.DeviceUsers.Read,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.DeviceUsersClaims.Read,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.DeviceUsers.Update,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.DeviceUsersClaims.Update,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.DeviceUsers.Delete,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.DeviceUsersClaims.Delete,
                        AuthConsts.ClaimDefaultValue));
                // Eforms
                options.AddPolicy(AuthConsts.EformPolicies.Eforms.Create,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EformsClaims.Create,
                        AuthConsts.ClaimDefaultValue));
                options.AddPolicy(AuthConsts.EformPolicies.Eforms.Delete,
                    policy => policy.RequireClaim(AuthConsts.EformClaims.EformsClaims.Delete,
                        AuthConsts.ClaimDefaultValue));
            });
        }

        public static void AddEFormMvc(this IServiceCollection services,
            List<IEformPlugin> plugins)
        {
            var mvcBuilder = services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            foreach (var plugin in plugins)
            {
                mvcBuilder.AddApplicationPart(plugin.PluginAssembly())
                    .AddControllersAsServices();
            }
        }

        public static void AddEFormPluginsDbContext(this IServiceCollection services,
            IConfiguration configuration,
            List<IEformPlugin> plugins)
        {
            foreach (var plugin in plugins)
            {
                var connectionString = configuration.GetConnectionString(plugin.ConnectionStringName());
                plugin.ConfigureDbContext(services, connectionString);
            }
        }
    }
}