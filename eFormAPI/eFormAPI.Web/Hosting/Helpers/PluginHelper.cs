﻿/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using eFormAPI.Web.Hosting.Enums;
using eFormAPI.Web.Infrastructure.Database.Entities;
using eFormAPI.Web.Infrastructure.Database.Factories;
using eFormCore;
using McMaster.NETCore.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microting.eFormApi.BasePn;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Database.Base;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;
using Microting.eFormApi.BasePn.Infrastructure.Delegates;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormApi.BasePn.Infrastructure.Settings;
using Microting.eFormApi.BasePn.Services;
using Newtonsoft.Json.Linq;
using OpenStack.NetCoreSwiftClient;
using OpenStack.NetCoreSwiftClient.Infrastructure.Models;

namespace eFormAPI.Web.Hosting.Helpers
{
    public static class PluginHelper
    {
        public static List<IEformPlugin> GetPlugins(string connectionString)
        {
            // Load info from database
            List<EformPlugin> eformPlugins = null;
            var contextFactory = new BaseDbContextFactory();
            using (var dbContext = contextFactory.CreateDbContext(new[] {connectionString}))
            {
                try
                {
                    eformPlugins = dbContext.EformPlugins
                        .AsNoTracking()
                        .ToList();
                }
                catch
                {
                }
            }

            var plugins = new List<IEformPlugin>();
            // create plugin loaders
            if (eformPlugins != null)
            {
                using (var dbContext = contextFactory.CreateDbContext(new[] {connectionString}))
                {
                    var dbNameSection = Regex.Match(connectionString, @"(Database=\w*;)").Groups[0].Value;
                    var dbPrefix = Regex.Match(connectionString, @"Database=(\d*)_").Groups[1].Value;

                    foreach (var plugin in GetAllPlugins())
                    {
                        var eformPlugin = eformPlugins.FirstOrDefault(x => x.PluginId == plugin.PluginId);
                        if (eformPlugin != null)
                        {
//                            if (eformPlugin.ConnectionString.IsNullOrEmpty())
//                            {
//                                eformPlugin.ConnectionString = "";
//                            }
                            if (!eformPlugin.ConnectionString.Contains("PersistSecurityInfo=true;"))
                            {
                                var aPlugin =
                                    dbContext.EformPlugins.SingleOrDefault(x => x.PluginId == plugin.PluginId);
                                if (aPlugin != null) aPlugin.ConnectionString += "PersistSecurityInfo=true;";
                                dbContext.SaveChanges();
                            }
                            
                            if (eformPlugin.Status == (int) PluginStatus.Enabled)
                            {
                                plugins.Add(plugin);
                            }
                        }
                        else
                        {
                            var pluginDbName = $"Database={dbPrefix}_{plugin.PluginId};";
                            var pluginConnectionString =
                                connectionString.Replace(dbNameSection, pluginDbName) + 
                                "PersistSecurityInfo=true;";
                            var newPlugin = new EformPlugin
                            {
                                PluginId = plugin.PluginId,
                                ConnectionString = pluginConnectionString,
                                Status = (int) PluginStatus.Disabled
                            };
                            dbContext.EformPlugins.Add(newPlugin);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }

            return plugins;
        }
        
        public static async Task<string> GetLatestRepositoryVersion(string githubUserName, string pluginName)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri($"https://api.github.com/repos/{githubUserName}/{pluginName}/tags")
            };

            httpClient.DefaultRequestHeaders.Add("User-Agent", "ReleaseSearcher");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
            };

            string latestVersion = "";

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                JToken responseObj = JRaw.Parse(responseString);

                latestVersion = responseObj.First["name"].ToString();

            }

            latestVersion = latestVersion.Replace("v", "");
            if (latestVersion.Split(".").Count() == 2)
            {
                latestVersion += ".0";
            }

            if (pluginName == "eform-angular-frontend")
            {
                latestVersion += ".0";
            }
            return latestVersion;
        }


        public static List<IEformPlugin> GetAllPlugins()
        {
            var plugins = new List<IEformPlugin>();
            // create plugin loaders
            Console.ForegroundColor = ConsoleColor.Green;
            var pluginsDir = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
            Console.WriteLine($"[INF] Trying to discover plugins in folder : {pluginsDir}");
            if (!Directory.Exists(pluginsDir))
            {
                try
                {
                    Directory.CreateDirectory(pluginsDir);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERR] Unable to create directory for plugins");
                    throw new Exception("Unable to create directory for plugins");
                }
            }

            var directories = Directory.EnumerateDirectories(pluginsDir);
            foreach (var directory in directories)
            {
                List<string> pluginList;


                string path = Path.Combine(directory, "netcoreapp2.2");
                if (Directory.Exists(path))
                {
                    pluginList = Directory.GetFiles(path)
                        .Where(x => x.EndsWith("Pn.dll") && Path.GetFileName(x) != "eFormApi.BasePn.dll")
                        .ToList();    
                }
                else
                {
                    pluginList = Directory.GetFiles(directory)
                        .Where(x => x.EndsWith("Pn.dll") && Path.GetFileName(x) != "eFormApi.BasePn.dll")
                        .ToList();
                }

                foreach (var pluginFile in pluginList)
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(pluginFile,
                        // this ensures that the plugin resolves to the same version of DependencyInjection
                        // and ASP.NET Core that the current app uses
                        new[]
                        {
                            typeof(IApplicationBuilder),
                            typeof(IEformPlugin),
                            typeof(IServiceCollection),
                            typeof(IEFormCoreService),
                            typeof(IPluginConfigurationSeedData),
                            typeof(IPluginDbContext),
                            typeof(IConfigurationBuilder),
                            typeof(ReloadDbConfiguration),
                            typeof(DbSet<PluginConfigurationValueVersion>),
                            typeof(DbSet<PluginConfigurationValue>),
                            typeof(ReloadDbConfiguration),
                            typeof(EFormCoreService),

                            typeof(ModelBuilder),
                            typeof(PluginConfigurationValue),
                            typeof(PluginConfigurationValueVersion),
                            typeof(BaseEntity),
                            typeof(PluginConfigurationProvider<>),
                            typeof(ConfigurationProvider),
                            typeof(DbContext),
                            typeof(WarningsConfiguration),
                            typeof(WarningBehavior),
                            typeof(DbContextOptionsBuilder),
                            typeof(DbContextOptions),
                            typeof(InMemoryEventId),
                            typeof(LoggerCategory<>),
                            typeof(DbLoggerCategory),
                            typeof(WarningsConfigurationBuilder),
                            typeof(MySqlDbContextOptionsExtensions),
                            typeof(CoreOptionsExtension),
                            typeof(RelationalEventId),
                            typeof(IDbContextOptionsBuilderInfrastructure),
                            typeof(ModelSnapshot),       
                            typeof(ILazyLoader),

                            typeof(IPluginDbOptions<>),
                            typeof(IOptionsSnapshot<>),
                            typeof(PluginDbOptions<>),
                            typeof(IDesignTimeDbContextFactory<>),
                            typeof(Core),
                            typeof(GetObjectResponse),
                            typeof(AmazonS3Client),
                            typeof(SwiftObjectGetResponse),
                            typeof(SwiftClient)
                        });
                    var types = loader
                        .LoadDefaultAssembly()
                        .GetTypes();

                    foreach (var type in types
                        .Where(t => typeof(IEformPlugin).IsAssignableFrom(t) && !t.IsAbstract))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[INF] Found plugin : " + type.Name);
                        var plugin = (IEformPlugin) Activator.CreateInstance(type);
                        plugins.Add(plugin);
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INF] {plugins.Count} plugins found");

            Console.ForegroundColor = ConsoleColor.Gray;
            return plugins;
        }
    }
}