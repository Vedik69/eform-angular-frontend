﻿/*
The MIT License (MIT)

Copyright (c) 2007 - 2020 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Linq;
using System.Collections.Generic;
using eFormAPI.Web.Abstractions;
using eFormAPI.Web.Abstractions.Advanced;
using eFormAPI.Web.Abstractions.Eforms;
using eFormAPI.Web.Abstractions.Security;
using eFormAPI.Web.Hosting.Extensions;
using eFormAPI.Web.Hosting.Security;
using eFormAPI.Web.Infrastructure.Database;
using eFormAPI.Web.Infrastructure.Models.Settings.Plugins;
using eFormAPI.Web.Services;
using eFormAPI.Web.Services.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Localization;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;
using Microting.eFormApi.BasePn.Infrastructure.Models.Application;
using Microting.eFormApi.BasePn.Localization;
using Microting.eFormApi.BasePn.Localization.Abstractions;
using Microting.eFormApi.BasePn.Services;
using eFormAPI.Web.Infrastructure.Database.Factories;
using eFormAPI.Web.Services.Export;
using eFormAPI.Web.Services.Mailing.CasePost;
using eFormAPI.Web.Services.Mailing.EmailRecipients;
using eFormAPI.Web.Services.Mailing.EmailService;
using eFormAPI.Web.Services.Mailing.EmailTags;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace eFormAPI.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO check if we need this or code needs to be updated.
            services.AddMvc(options => options.EnableEndpointRouting = false);

            // Configuration
            services.AddOptions();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
            if (!string.IsNullOrEmpty(Configuration.MyConnectionString()))
            {
                if (Configuration.MyConnectionString() != "...")
                {
                    services.AddEntityFrameworkMySql()
                        .AddDbContext<BaseDbContext>(o => o.UseMySql(Configuration.MyConnectionString(),
                            b => b.MigrationsAssembly("eFormAPI.Web")));
                }
                else
                {
                    // We use this hack to get the project started and we actually don't use this connection, but it's needed for the service to start.
                    // Once we have the correct connectionstring in the connection.json, we restart the server and the above method is used.
                    services.AddEntityFrameworkMySql()
                        .AddDbContext<BaseDbContext>(o => o.UseMySql("server=localhost;",
                            b => b.MigrationsAssembly("eFormAPI.Web")));
                }
            }

            // plugins
            services.AddEFormPluginsDbContext(Configuration, Program.Plugins);
            // Identity services
            services.AddIdentity<EformUser, EformRole>()
                .AddEntityFrameworkStores<BaseDbContext>()
                .AddDefaultTokenProviders();
            // Authentication
            services.AddEFormAuth(Configuration, GetPluginsPermissions());
            // Localization
            services.AddTransient<IEformLocalizerFactory, JsonStringLocalizerFactory>();
            services.AddTransient<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
            services.AddTransient<IStringLocalizer, JsonStringLocalizer>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            // MVC and API services with Plugins
            services.AddEFormMvc(Program.Plugins);
            // Writable options
            services.ConfigureWritable<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"),
                "connection.json");
            // Database options
            services.ConfigureDbOptions<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
            services.ConfigureDbOptions<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.ConfigureDbOptions<LoginPageSettings>(Configuration.GetSection("LoginPageSettings"));
            services.ConfigureDbOptions<HeaderSettings>(Configuration.GetSection("HeaderSettings"));
            services.ConfigureDbOptions<ConnectionStringsSdk>(Configuration.GetSection("ConnectionStringsSdk"));
            services.ConfigureDbOptions<EformTokenOptions>(Configuration.GetSection("EformTokenOptions"));
            services.ConfigureDbOptions<PluginStoreSettings>(Configuration.GetSection("PluginStoreSettings"));
            // Database plugins options
            foreach (var plugin in Program.Plugins)
            {
                plugin.ConfigureOptionsServices(
                    services,
                    Configuration
                );
            }

            // Use HttpClient factory
            services.AddHttpClient();
            // Form options
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
            });
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "eForm API",
                    Description = "API documentation"
                });
                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //var xmlPath = Path.Combine(basePath, "API.doc.xml");
                //c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });



            ConnectServices(services);

            // plugins
            services.AddEFormPlugins(Program.Plugins);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                    // IIS is also tagging a X-Forwarded-For header on, so we need to increase this limit,
                    // otherwise the X-Forwarded-For we are passing along from the browser will be ignored
                    ForwardLimit = 2
                });
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            if (env.IsDevelopment())
            {
                //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                //loggerFactory.AddDebug();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            // Plugins
            app.UseEFormLocalization();
            // MVC
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "api/{controller=Home}/{action=Index}/{id?}");
            });

            if (env.IsDevelopment())
            {
                // Since swagger is not accessible from outside the local server we do not need to disable it for production.
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });
            }

            // Plugins
            app.UseEFormPlugins(Program.Plugins);
            // Route all unknown requests to app root
            app.UseAngularMiddleware(env);
        }


        private static void ConnectServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<IEFormCoreService, EFormCoreService>();
            services.AddScoped<ITagsService, TagsService>();
            services.AddScoped<ITemplateColumnsService, TemplateColumnsService>();
            services.AddScoped<IUnitsService, UnitsService>();
            services.AddScoped<IWorkersService, WorkersService>();
            services.AddScoped<ISitesService, SitesService>();
            services.AddScoped<IFoldersService, FoldersService>();
            services.AddScoped<IDeviceUsersService, DeviceUsersService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IEntitySearchService, EntitySearchService>();
            services.AddScoped<IEntitySelectService, EntitySelectService>();
            services.AddScoped<ICasesService, CasesService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<ITemplatesService, TemplatesService>();
            services.AddScoped<ISecurityGroupService, SecurityGroupService>();
            services.AddTransient<IClaimsService, ClaimsService>();
            services.AddScoped<IPermissionsService, PermissionsService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IEformGroupService, EformGroupService>();
            services.AddScoped<IEformPermissionsService, EformPermissionsService>();
            services.AddScoped<IEformReportsService, EformReportsService>();
            services.AddScoped<IPluginsManagementService, PluginsManagementService>();
            services.AddScoped<ISiteTagsService, SiteTagsService>();
            services.AddScoped<IEmailTagsService, EmailTagsService>();
            services.AddScoped<IEmailRecipientsService, EmailRecipientsService>();
            services.AddScoped<ICasePostService, CasePostService>();
            services.AddScoped<ICasePostBaseService, CasePostService>();
            services.AddTransient<IEformExcelExportService, EformExcelExportService>();
        }

        private ICollection<PluginPermissionModel> GetPluginsPermissions()
        {

            var permissions = new List<PluginPermissionModel>();
            if (Configuration.MyConnectionString() != "...")
            {
                var contextFactory = new BaseDbContextFactory();
                using (var dbContext = contextFactory.CreateDbContext(new[] { Configuration.MyConnectionString() }))
                {
                    foreach (var eformPlugin in dbContext.EformPlugins
                        .AsNoTracking()
                        .Where(x => x.ConnectionString != "..."))
                    {
                        var plugin = Program.Plugins.FirstOrDefault(p => p.PluginId == eformPlugin.PluginId);
                        if (plugin != null)
                        {
                            var permissionsManager = plugin.GetPermissionsManager(eformPlugin.ConnectionString);
                            permissions.AddRange(permissionsManager.GetPluginPermissions().Result);
                        }
                    }
                }
            }

            return permissions;
        }
    }
}
