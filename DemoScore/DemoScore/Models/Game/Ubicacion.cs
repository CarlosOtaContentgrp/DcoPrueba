using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DemoScore.Models.Game
{
    public class Ubicacion
    {
        [Key]
        public int Loca_Id { get; set; }
        [Display(Name = "Descripción")]
        public string Loca_Description { get; set; }


        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}