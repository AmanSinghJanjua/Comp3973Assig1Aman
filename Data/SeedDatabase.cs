using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsumeGoogleApi.Data
{
    public class SeedDatabase
    {
        public static void Intialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            context.Database.EnsureCreated();
            if(!context.Users.Any())
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = "a@a.a",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "a@a.a",
                    EmailConfirmed = true
                };
                userManager.CreateAsync(user, "P@$$w0rd");
            }

        }
    }
}
