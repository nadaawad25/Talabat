using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.Apis.Extentions
{
    public static class UserManagerExtention 
    {
        //First Parameter is Caller That call FindUserWithAddreesAsync By Any name its Typed "UserManager<AppUser>"
        public static Task<AppUser?> FindUserWithAddreesAsync(this UserManager<AppUser> userManager  , ClaimsPrincipal user)
        {
            var Email = user.FindFirstValue(ClaimTypes.Email);
            var appUser =userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == Email);
            return appUser;

        }

    }
}
