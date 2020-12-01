using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FromLocalsToLocals.Database;
using FromLocalsToLocals.Models;
using Microsoft.AspNet.Identity;
using FromLocalsToLocals.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Components;
using FromLocalsToLocals.Models.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Localization;
using SendGridAccount = FromLocalsToLocals.Utilities.SendGridAccount;
using Quartz;
using Quartz.Impl;

namespace FromLocalsToLocals
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
            services.AddLocalization(opts =>
            {
                opts.ResourcesPath = "Resources";
            });

            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });
            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();
            //services.AddMvc()
            //    .AddViewLocalization(opts => { opts.ResourcesPath = "Resources"; })
            //    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            //    .AddDataAnnotationsLocalization()
            //    .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo> {
                new CultureInfo("en"),
                new CultureInfo("lt"),
            };
                opts.DefaultRequestCulture = new RequestCulture("en");
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
            });
            services.AddControllersWithViews();
            services.AddRazorPages();
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseLazyLoadingProxies()
                .UseSqlServer(Configuration.GetConnectionString("AppDbContext")), ServiceLifetime.Transient);


            services.Configure<SendGridAccount>(Configuration.GetSection("SendGridAccount"));

            services.AddIdentity<AppUser, IdentityRole>(options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.SignIn.RequireConfirmedEmail = false;
                }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();



            services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
            {
                ProgressBar = false,
                PositionClass = ToastPositions.BottomCenter
            });

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped<IReviewsService, ReviewsService>();


            services.AddSignalR();

   
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseNToastNotify();

            //var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<NotificationHub>("/notificationHub");
            });
        }

      

    }
}
