using GameLibrary.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Data
{
    public class Seed
    {
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public Seed(UserManager<ApplicationUser> userManager)
        {
            //_roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task AddRoles()
        {
            var user = await _userManager.FindByEmailAsync("cvagan@gmail.com");
            Console.Write(user.DisplayName);
        }
    }
}
