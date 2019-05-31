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
// <auto-generated />
using System;
using eFormAPI.Web.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eFormAPI.Web.Infrastructure.Database;

namespace eFormAPI.Web.Migrations
{
    [DbContext(typeof(BaseDbContext))]
    [Migration("20181101123321_AddEformPermissionsTables")]
    partial class AddEformPermissionsTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            string autoIDGenStrategy = "SqlServer:ValueGenerationStrategy";
            object autoIDGenStrategyValue = SqlServerValueGenerationStrategy.IdentityColumn;
            if (DbConfig.IsMySQL)
            {
                autoIDGenStrategy = "MySql:ValueGenerationStrategy";
                autoIDGenStrategyValue = MySqlValueGenerationStrategy.IdentityColumn;
            }
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.EformInGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<int>("SecurityGroupId");

                    b.Property<int>("TemplateId");

                    b.HasKey("Id");

                    b.HasIndex("SecurityGroupId");

                    b.HasIndex("TemplateId");

                    b.HasIndex("TemplateId", "SecurityGroupId")
                        .IsUnique();

                    b.ToTable("EformInGroups");
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.EformPermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<int>("EformInGroupId");

                    b.Property<int>("PermissionId");

                    b.HasKey("Id");

                    b.HasIndex("EformInGroupId");

                    b.HasIndex("PermissionId", "EformInGroupId")
                        .IsUnique();

                    b.ToTable("EformPermissions");
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.GroupPermission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<int>("PermissionId");

                    b.Property<int>("SecurityGroupId");

                    b.HasKey("Id");

                    b.HasIndex("SecurityGroupId");

                    b.HasIndex("PermissionId", "SecurityGroupId")
                        .IsUnique();

                    b.ToTable("GroupPermissions");
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.MenuItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<string>("E2EId");

                    b.Property<string>("Link");

                    b.Property<int>("MenuPosition");

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.Property<int?>("ParentId");

                    b.Property<int>("Position");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("MenuItems");
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<string>("ClaimName")
                        .HasMaxLength(250);

                    b.Property<string>("PermissionName")
                        .HasMaxLength(250);

                    b.Property<int>("PermissionTypeId");

                    b.HasKey("Id");

                    b.HasIndex("ClaimName")
                        .IsUnique()
                        .HasFilter("[ClaimName] IS NOT NULL");

                    b.HasIndex("PermissionTypeId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.PermissionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("PermissionTypes");
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.SecurityGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.ToTable("SecurityGroups");
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.SecurityGroupUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<int>("EformUserId");

                    b.Property<int>("SecurityGroupId");

                    b.HasKey("Id");

                    b.HasIndex("SecurityGroupId");

                    b.HasIndex("EformUserId", "SecurityGroupId")
                        .IsUnique();

                    b.ToTable("SecurityGroupUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation(autoIDGenStrategy, autoIDGenStrategyValue);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("GoogleAuthenticatorSecretKey");

                    b.Property<bool>("IsGoogleAuthenticatorEnabled");

                    b.Property<string>("LastName");

                    b.Property<string>("Locale");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformUserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.EformInGroup", b =>
                {
                    b.HasOne("eFormAPI.Web.Infrastructure.Database.Entities.SecurityGroup", "SecurityGroup")
                        .WithMany()
                        .HasForeignKey("SecurityGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.EformPermission", b =>
                {
                    b.HasOne("eFormAPI.Web.Infrastructure.Database.Entities.EformInGroup", "EformInGroup")
                        .WithMany("EformPermissions")
                        .HasForeignKey("EformInGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eFormAPI.Web.Infrastructure.Database.Entities.Permission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.GroupPermission", b =>
                {
                    b.HasOne("eFormAPI.Web.Infrastructure.Database.Entities.Permission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eFormAPI.Web.Infrastructure.Database.Entities.SecurityGroup", "SecurityGroup")
                        .WithMany()
                        .HasForeignKey("SecurityGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.MenuItem", b =>
                {
                    b.HasOne("eFormAPI.Web.Infrastructure.Database.Entities.MenuItem", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.Permission", b =>
                {
                    b.HasOne("eFormAPI.Web.Infrastructure.Database.Entities.PermissionType", "PermissionType")
                        .WithMany()
                        .HasForeignKey("PermissionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("eFormAPI.Web.Infrastructure.Database.Entities.SecurityGroupUser", b =>
                {
                    b.HasOne("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformUser", "EformUser")
                        .WithMany()
                        .HasForeignKey("EformUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("eFormAPI.Web.Infrastructure.Database.Entities.SecurityGroup", "SecurityGroup")
                        .WithMany("SecurityGroupUsers")
                        .HasForeignKey("SecurityGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformUserRole", b =>
                {
                    b.HasOne("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microting.eFormApi.BasePn.Infrastructure.Database.Entities.EformUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
