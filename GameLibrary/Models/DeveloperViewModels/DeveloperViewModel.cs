using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Models.DeveloperViewModels
{
    public class DeveloperViewModel
    {
        public Developer Developer { get; set; }
        public IFormFile DeveloperImage { get; set; }
        public IEnumerable<Developer> DeveloperList { get; set; }
    }
}
