using GameLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Data
{
    public class AddClaim
    {
        private readonly ApplicationDbContext _context;

        public AddClaim(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Developer> Devs { get; set; }

        public void Test()
        {
            Devs = _context.Developers.ToList();
        }
    }
}
