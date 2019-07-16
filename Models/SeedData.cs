using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models
{
    public class SeedData
    {
        public static async Task CreateRolesAndAdminAccount(IApplicationBuilder app, IConfiguration configuration)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                UserManager<AppUser> userManager = services.GetRequiredService<UserManager<AppUser>>();
                RoleManager<IdentityRole> roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();
                string username = configuration["Data:AdminAccount:Username"];
                string email = configuration["Data:AdminAccount:Email"];
                string avatar = configuration["Data:AdminAccount:Avatar"];
                string password = configuration["Data:AdminAccount:Password"];
                string role = configuration["Data:AdminAccount:Role"];
                string roleList = configuration["Data:Roles"];
                List<string> roles = JsonConvert.DeserializeObject<List<string>>(configuration["Data:Roles"]);
                if (await userManager.FindByNameAsync(username) == null)
                {
                    if (await roleManager.FindByNameAsync(role) == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                    AppUser user = new AppUser
                    {
                        UserName = username,
                        Email = email,
                        Avatar = avatar,
                        RegisterDate = DateTime.Now
                    };
                    IdentityResult result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }

                foreach (string name in roles)
                {
                    if (await roleManager.FindByNameAsync(name) == null)
                    {
                        IdentityResult ir = await roleManager.CreateAsync(new IdentityRole(name));
                    }
                }
                AppUser guide = new AppUser
                {
                    UserName = "ken",
                    Email = "Ken@microsoft.com",
                    Avatar = avatar,
                    RegisterDate = DateTime.Now
                };
                await userManager.CreateAsync(guide, "Ken123*");
                await userManager.AddToRoleAsync(guide, "Guide");
            }
        }
    }
}
