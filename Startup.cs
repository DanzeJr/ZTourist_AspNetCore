using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.Configure<RouteOptions>(options => {
                options.LowercaseUrls = true;
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
            services.AddTransient<TouristDAL>();
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            services.AddHttpContextAccessor();
            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseStatusCodePages();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Error",
                    template: "Error",
                    defaults: new { controller = "Home", action = "Error" }
                    );
                routes.MapRoute(
                    name: null,
                    template: "{controller}s",
                    defaults: new { controller = "Tour", action = "Index" }
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
            //SeedData.CreateRolesAndAdminAccount(app, Configuration).Wait();
        }
    }
}
