﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UI_DSM.Server.Context;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                            ConcurrencyStamp = "c1a9421c-4176-4acb-b473-ebc03780f807",
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                            RoleId = "AF8956F8-CA85-4DF2-8CB6-C46D0845B987"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Entity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Entity", (string)null);
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "172b3dd7-4ed4-4b2f-be64-206637a544be",
                            EmailConfirmed = false,
                            IsAdmin = true,
                            LockoutEnabled = false,
                            NormalizedUserName = "ADMIN",
                            PasswordHash = "AQAAAAEAACcQAAAAEA8NqgZEzQJQBXCNn8kdGlZ+LCKDV9ScuiZIXaJJXZ/Eo5lIHa3BLffcJk/tAPBv5Q==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "662edb1d-3e9c-4bdf-91de-0a9050e42dfa",
                            TwoFactorEnabled = false,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Participant", b =>
                {
                    b.HasBaseType("UI_DSM.Shared.Models.Entity");

                    b.Property<Guid?>("EntityContainerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasIndex("EntityContainerId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("Participant", (string)null);
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Project", b =>
                {
                    b.HasBaseType("UI_DSM.Shared.Models.Entity");

                    b.Property<Guid?>("EntityContainerId")
                        .HasColumnType("uuid");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("EntityContainerId");

                    b.HasIndex("ProjectName")
                        .IsUnique();

                    b.ToTable("Project", (string)null);
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Review", b =>
                {
                    b.HasBaseType("UI_DSM.Shared.Models.Entity");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("EntityContainerId")
                        .HasColumnType("uuid");

                    b.Property<int>("ReviewNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ReviewNumber"));

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("AuthorId");

                    b.HasIndex("EntityContainerId");

                    b.ToTable("Review", (string)null);
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.ReviewObjective", b =>
                {
                    b.HasBaseType("UI_DSM.Shared.Models.Entity");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("EntityContainerId")
                        .HasColumnType("uuid");

                    b.Property<int>("ReviewObjectiveNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ReviewObjectiveNumber"));

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("AuthorId");

                    b.HasIndex("EntityContainerId");

                    b.ToTable("ReviewObjective", (string)null);
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Role", b =>
                {
                    b.HasBaseType("UI_DSM.Shared.Models.Entity");

                    b.Property<int[]>("AccessRights")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.Property<Guid?>("EntityContainerId")
                        .HasColumnType("uuid");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("EntityContainerId");

                    b.HasIndex("RoleName")
                        .IsUnique();

                    b.ToTable("Role", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("fd580a55-9666-4abe-a02b-3a99478996f7"),
                            AccessRights = new[] { 0, 1, 2, 3 },
                            RoleName = "Project Administrator"
                        },
                        new
                        {
                            Id = new Guid("28b83519-fb7c-4a9a-8279-194140bfcfbe"),
                            AccessRights = new[] { 4 },
                            RoleName = "Reviewer"
                        });
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.UserEntity", b =>
                {
                    b.HasBaseType("UI_DSM.Shared.Models.Entity");

                    b.Property<Guid?>("EntityContainerId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasIndex("EntityContainerId");

                    b.HasIndex("UserId");

                    b.ToTable("UserEntity", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("3503bf4c-1211-41eb-b369-aaa6bbdf5ff8"),
                            IsAdmin = true,
                            UserId = "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("UI_DSM.Shared.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("UI_DSM.Shared.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UI_DSM.Shared.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("UI_DSM.Shared.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Participant", b =>
                {
                    b.HasOne("UI_DSM.Shared.Models.Project", "EntityContainer")
                        .WithMany("Participants")
                        .HasForeignKey("EntityContainerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("UI_DSM.Shared.Models.Entity", null)
                        .WithOne()
                        .HasForeignKey("UI_DSM.Shared.Models.Participant", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UI_DSM.Shared.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UI_DSM.Shared.Models.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EntityContainer");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Project", b =>
                {
                    b.HasOne("UI_DSM.Shared.Models.Entity", "EntityContainer")
                        .WithMany()
                        .HasForeignKey("EntityContainerId");

                    b.HasOne("UI_DSM.Shared.Models.Entity", null)
                        .WithOne()
                        .HasForeignKey("UI_DSM.Shared.Models.Project", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EntityContainer");
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Review", b =>
                {
                    b.HasOne("UI_DSM.Shared.Models.Participant", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("UI_DSM.Shared.Models.Project", "EntityContainer")
                        .WithMany("Reviews")
                        .HasForeignKey("EntityContainerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("UI_DSM.Shared.Models.Entity", null)
                        .WithOne()
                        .HasForeignKey("UI_DSM.Shared.Models.Review", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("EntityContainer");
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.ReviewObjective", b =>
                {
                    b.HasOne("UI_DSM.Shared.Models.Participant", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId");

                    b.HasOne("UI_DSM.Shared.Models.Review", "EntityContainer")
                        .WithMany("ReviewObjectives")
                        .HasForeignKey("EntityContainerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("UI_DSM.Shared.Models.Entity", null)
                        .WithOne()
                        .HasForeignKey("UI_DSM.Shared.Models.ReviewObjective", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("EntityContainer");
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Role", b =>
                {
                    b.HasOne("UI_DSM.Shared.Models.Entity", "EntityContainer")
                        .WithMany()
                        .HasForeignKey("EntityContainerId");

                    b.HasOne("UI_DSM.Shared.Models.Entity", null)
                        .WithOne()
                        .HasForeignKey("UI_DSM.Shared.Models.Role", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EntityContainer");
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.UserEntity", b =>
                {
                    b.HasOne("UI_DSM.Shared.Models.Entity", "EntityContainer")
                        .WithMany()
                        .HasForeignKey("EntityContainerId");

                    b.HasOne("UI_DSM.Shared.Models.Entity", null)
                        .WithOne()
                        .HasForeignKey("UI_DSM.Shared.Models.UserEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UI_DSM.Shared.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("EntityContainer");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Project", b =>
                {
                    b.Navigation("Participants");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("UI_DSM.Shared.Models.Review", b =>
                {
                    b.Navigation("ReviewObjectives");
                });
#pragma warning restore 612, 618
        }
    }
}
