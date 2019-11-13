using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DemoScore.Models.Game
{
    public class Area
    {
        [Key]
        public int AreaId { get; set; }
        [Display(Name = "Nombre de Area")]
        public string AreaName { get; set; }
        

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}