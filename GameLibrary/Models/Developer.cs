using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Models
{
    public class Developer
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Year Established")]
        public int Created { get; set; }

        [Required]
        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        public string PhotoId { get; set; }

        public string PhotoUrl { get; set; }

        public IEnumerable<Game> Games { get; set; }
    }
}
