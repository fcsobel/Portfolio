using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Data.Context;
using Portfolio.Data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Web.App
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
            services.AddControllersWithViews(o => o.Filters.Add(new AuthorizeFilter()));

            services.AddSingleton<IConfiguration>(Configuration);

            // Auth
            // Configuew Authenication using "Cookie" Authentication scheme
            // Authentice / Challenge / Forbid
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                //.AddAuthentication(o =>
                //{
                //    o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use Cookies for Auth
                //    o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Use Google for Default Challenge
                //})
                .AddCookie(o => // Default Cookies Container for Local Login under "Cookies"
                {
                    o.LoginPath = "/auth"; 
                    o.ForwardForbid = "/auth/denied"; // Forbid 
                    //o.Events = new CookieAuthenticationEvents { OnValidatePrincipal = (c) => { return c.RejectPrincipal(); }  ;
                })
                .AddCookie("ExternalAuth") // External Cookie Container for Google Login Cookie under "ExternalAuth"
                .AddGoogle(o =>
                {
                    o.ClientId = Configuration["Google:ClientId"];
                    o.ClientSecret = Configuration["Google:ClientSecret"];
                    o.SignInScheme = "ExternalAuth"; // Use ExternalAuth Cookie COntrainer for SIgnin
                    o.ClaimActions.MapJsonKey("image", "picture"); // Map the external picture claim to the internally used image claim
                });


            // Databases
            services.AddDbContext<PortfolioContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PortfolioContext")));

            // Services
            services.AddSingleton<IEXService>();
            services.AddScoped<PortfolioService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PortfolioContext context)
        {
            // https://abelsquidhead.com/index.php/2017/07/31/deploying-dbs-in-your-cicd-pipeline-with-ef-core-code-first/
            // When the environment is hit for the first time after deployment, 
            // Run all the migrations that haven’t been run yet in the correct order, and your new DB schemas will automatically be deployed.
            context.Database.Migrate();

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

            // Add Authenication middleware
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
