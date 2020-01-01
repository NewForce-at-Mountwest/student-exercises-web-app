using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesWebApp.Models
{
    public class Exercise
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Exercise Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Exercise Language")]
        public string Language { get; set; }
    }
}
