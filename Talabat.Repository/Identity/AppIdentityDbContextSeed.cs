using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "Nada Mahmoud",
                    Email = "nadaawad112002@gmail.com",
                    PhoneNumber = "01013818970",
                    UserName = "nadaawad112002"

                };
                await userManager.CreateAsync(User, "Pa$$w0rd");
            }
           
        }
    }
}
