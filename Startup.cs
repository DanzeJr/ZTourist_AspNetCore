using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using ZTourist.Infrastructure;
using ZTourist.Models;

namespace ZTourist
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddTransient<IAuthorizationHandler, NotRolesHandler>();
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("OnlyAnonymous", policy =>
                {
                    policy.AddRequirements(new NotRolesRequirement("Customer", "Admin", "Guide"));
                });
                opts.AddPolicy("NotCustomer", policy =>
                {
                    policy.AddRequirements(new NotRolesRequirement("Customer"));
                });
                opts.AddPolicy("NotEmployee", policy =>
                {
                    policy.AddRequirements(new NotRolesRequirement("Admin", "Guide"));
                });
                opts.AddPolicy("Customer", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Customer");
                });
                opts.AddPolicy("Employee", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin", "Guide");
                    policy.AddAuthenticationSchemes("Identity.Application", "COMPANY");
                });
                opts.AddPolicy("Admin", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                    policy.AddAuthenticationSchemes("Identity.Application", "COMPANY");
                });
            });
            services.AddDbContext<AppIdentityDbContext>(opts =>
                opts.UseSqlServer(Configuration["Data:ZTouristDB:ConnectionString"])
            );

            services.AddIdentity<AppUser, IdentityRole>(opts =>
                {
                    opts.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();
            services.AddAuthentication()
                .AddCookie("COMPANY", opts =>
                {
                    opts.LoginPath = "/company/account/login";
                    opts.LogoutPath = "/company/account/logout";
                    opts.AccessDeniedPath = "/company/account/accessdenied";
                })
                .AddGoogle(opts => {
                    opts.ClientId = Configuration["google:client_id"];
                    opts.ClientSecret = Configuration["google:client_secret"];
                })
                .AddFacebook(opts => {
                    opts.AppId = Configuration["facebook:app_id"];
                    opts.AppSecret = Configuration["facebook:app_secret"];
                });

            services.Configure<SecurityStampValidatorOptions>(options => {
                options.ValidationInterval = TimeSpan.FromSeconds(0);
            });
            services.AddTransient<TourDAL>();
            services.AddTransient<DestinationDAL>();
            services.AddTransient<OrderDAL>();
            services.AddTransient<CouponDAL>();
            services.AddTransient<BlobService>();
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            services.AddHttpContextAccessor();
            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = new LoggerConfiguration().WriteTo.AzureBlobStorage(Configuration["Data:StorageAccount"], Serilog.Events.LogEventLevel.Information, "logs", "{yyyy}/{MM}/{dd}/log.txt").CreateLogger();
            loggerFactory.AddSerilog(logger);
            if (env.IsDevelopment())
            {
                app.UseStatusCodePages();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "CompanyArea",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                    );
                routes.MapRoute(
                    name: "ErrorWithStatusCode",
                    template: "Error/{statusCode:int}",
                    defaults: new { controller = "Home", action = "Error" }
                    );
                routes.MapRoute(
                    name: "Error",
                    template: "Error",
                    defaults: new { controller = "Home", action = "Error" }
                    );
                routes.MapRoute(
                    name: null,
                    template: "Tours/Page{page:int}",
                    defaults: new { controller = "Tour", action = "Index", page = 1 }
                    );
                routes.MapRoute(
                    name: null,
                    template: "Tours/Search/Page{page:int}",
                    defaults: new { controller = "Tour", action = "Search", page = 1 }
                    );
                routes.MapRoute(
                    name: null,
                    template: "Tours/{id}",
                    defaults: new { controller = "Tour", action = "Details" }
                    );
                routes.MapRoute(
                    name: null,
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" }
                    );
            });
            try
            {
                SeedData.CreateRolesAndAdminAccount(app, Configuration).Wait();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }
            
        }
    }
}
