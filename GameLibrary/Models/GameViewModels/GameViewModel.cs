using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Models
{
    public class GameViewModel
    {
        public Game Game { get; set; }
        public IFormFile GameImage { get; set; }
        public IEnumerable<Developer> Developers { get; set; }
        public IEnumerable<Game> Games { get; set; }
    }
}
