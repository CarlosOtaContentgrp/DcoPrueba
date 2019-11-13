using SCORM1.Models.MainGame;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoScore.Models.Game
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        [Display(Name = "Nombre")]
        public string CompanyName { get; set; }

      
        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
        public virtual ICollection<Area> Area { get; set; }
        public virtual ICollection<Ubicacion> Ubicacion { get; set; }
        public virtual ICollection<Cargo> Cargo { get; set; }
        public virtual ICollection<MG_Template> MG_Template { get; set; }
        public virtual ICollection<Categoria> Categoria { get; set; }
        public virtual ICollection<SubCategoria> SubCategoria { get; set; }
    }
}