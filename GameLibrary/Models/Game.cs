using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Year { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; }

        [Required]
        public int DeveloperId { get; set; }

        [ForeignKey("DeveloperId")]
        public Developer Developer { get; set; }

        public string PhotoId { get; set; }

        public string PhotoUrl { get; set; }
    }
}
