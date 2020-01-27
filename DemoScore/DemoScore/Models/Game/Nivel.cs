using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DemoScore.Models.Game
{
    public class Nivel
    {
        [Key]
        public int Nivel_Id { get; set; }
        [Display(Name = "Nivel")]
        public string Nivel_Name { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }

    }
}