﻿// <auto-generated />
using System;
using FromLocalsToLocals.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FromLocalsToLocals.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210202002941_NewDb")]
    partial class NewDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.AppUser", b =>
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

                    b.Property<byte[]>("Image")
                        .HasColumnType("bytea");

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

                    b.Property<bool>("Subscribe")
                        .HasColumnType("boolean");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<int>("VendorsCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Contact", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("ReceiverID")
                        .HasColumnType("integer");

                    b.Property<bool>("ReceiverRead")
                        .HasColumnType("boolean");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("UserRead")
                        .HasColumnType("boolean");

                    b.HasKey("ID");

                    b.HasIndex("ReceiverID");

                    b.HasIndex("UserID");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Follower", b =>
                {
                    b.Property<string>("UserID")
                        .HasColumnType("text");

                    b.Property<int>("VendorID")
                        .HasColumnType("integer");

                    b.HasKey("UserID", "VendorID");

                    b.HasIndex("VendorID");

                    b.ToTable("Followers");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Message", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("ContactID")
                        .HasColumnType("integer");

                    b.Property<string>("Date")
                        .HasColumnType("text");

                    b.Property<bool>("IsUserSender")
                        .HasColumnType("boolean");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("ContactID");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Notification", b =>
                {
                    b.Property<int>("NotiId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("NotiBody")
                        .HasColumnType("text");

                    b.Property<string>("OwnerId")
                        .HasColumnType("text");

                    b.Property<int?>("ReviewID")
                        .HasColumnType("integer");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.Property<int>("VendorId")
                        .HasColumnType("integer");

                    b.HasKey("NotiId");

                    b.HasIndex("ReviewID");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Post", b =>
                {
                    b.Property<int>("PostID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Date")
                        .HasColumnType("text");

                    b.Property<byte[]>("Image")
                        .HasColumnType("bytea");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.Property<int>("VendorID")
                        .HasColumnType("integer");

                    b.HasKey("PostID");

                    b.HasIndex("VendorID");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Review", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("CommentID")
                        .HasColumnType("integer");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Reply")
                        .HasColumnType("text");

                    b.Property<string>("ReplyDate")
                        .HasColumnType("text");

                    b.Property<string>("ReplySender")
                        .HasColumnType("text");

                    b.Property<string>("SenderUsername")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Stars")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("VendorID")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("VendorID");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Vendor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("About")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DateCreated")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("Image")
                        .HasColumnType("bytea");

                    b.Property<DateTime>("LastClickDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<int>("Popularity")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VendorTypeDb")
                        .HasColumnType("text")
                        .HasColumnName("VendorType");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.WorkHours", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<TimeSpan>("CloseTime")
                        .HasColumnType("interval");

                    b.Property<int>("Day")
                        .HasColumnType("integer");

                    b.Property<bool>("IsWorking")
                        .HasColumnType("boolean");

                    b.Property<TimeSpan>("OpenTime")
                        .HasColumnType("interval");

                    b.Property<int>("VendorID")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("VendorID");

                    b.ToTable("VendorWorkHours");
                });

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

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
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

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
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

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Contact", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.Vendor", "Vendor")
                        .WithMany("Contacts")
                        .HasForeignKey("ReceiverID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FromLocalsToLocals.Contracts.Entities.AppUser", "User")
                        .WithMany("Contacts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Follower", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.AppUser", "User")
                        .WithMany("Following")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FromLocalsToLocals.Contracts.Entities.Vendor", "Vendor")
                        .WithMany("Followers")
                        .HasForeignKey("VendorID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Message", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.Contact", "Contact")
                        .WithMany("Messages")
                        .HasForeignKey("ContactID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contact");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Notification", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.Review", "Review")
                        .WithMany("Notifications")
                        .HasForeignKey("ReviewID");

                    b.Navigation("Review");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Post", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.Vendor", "Vendor")
                        .WithMany("Posts")
                        .HasForeignKey("VendorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Review", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.Vendor", "Vendor")
                        .WithMany("Reviews")
                        .HasForeignKey("VendorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Vendor", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.AppUser", "User")
                        .WithMany("Vendors")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.WorkHours", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.Vendor", "Vendor")
                        .WithMany("VendorHours")
                        .HasForeignKey("VendorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Vendor");
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
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.AppUser", null)
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

                    b.HasOne("FromLocalsToLocals.Contracts.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("FromLocalsToLocals.Contracts.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.AppUser", b =>
                {
                    b.Navigation("Contacts");

                    b.Navigation("Following");

                    b.Navigation("Vendors");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Contact", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Review", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("FromLocalsToLocals.Contracts.Entities.Vendor", b =>
                {
                    b.Navigation("Contacts");

                    b.Navigation("Followers");

                    b.Navigation("Posts");

                    b.Navigation("Reviews");

                    b.Navigation("VendorHours");
                });
#pragma warning restore 612, 618
        }
    }
}
