using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using StudentEnrollment.App.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentEnrollment.Entities;
using StudentEnrollment.Services;
using StudentEnrollment.App.Services;
using Microsoft.AspNetCore.Http;
using StudentEnrollment.Core.Services;
using System.Net;

namespace StudentEnrollment.App
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
            services.AddDatabaseDeveloperPageExceptionFilter();


            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("localhost")));
            services.AddIdentity<RequestUser, IdentityRole>(opt =>
            opt.User.RequireUniqueEmail = true
            ).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromMinutes(30));

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IUserAuthService, UserAuthService>();

            HttpClientServices(services);


            services.AddControllersWithViews();
            services.AddRazorPages();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Departments}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        public void HttpClientServices(IServiceCollection services)
        {
            services.AddScoped<IApiService, ApiService>();
            services.AddScoped<IUploadService, UploadService>();

            services.AddScoped<IHttpClientService, HttpClientService>();
            services.AddHttpClient<IHttpClientService, HttpClientService>(options =>
            {
                options.BaseAddress = new Uri("https://localhost:9999/student-enrollment/");
            })
            .ConfigureHttpMessageHandlerBuilder(builder =>
            {
                builder.PrimaryHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (m, cr, ch, e) => true
                    ,
                    UseProxy = false,
                    Proxy = null
                };
            });
        }
    }
}
