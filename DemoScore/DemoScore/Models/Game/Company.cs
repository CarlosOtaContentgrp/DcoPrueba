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
        [Display(Name = "Cantidad de preguntas nivel 1")]
        public int qnivel1 { get; set; }
        [Display(Name = "Cantidad de preguntas nivel 2")]
        public int qnivel2 { get; set; }
        [Display(Name = "Cantidad de preguntas nivel 3")]
        public int qnivel3 { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
        public virtual ICollection<Area> Area { get; set; }
        public virtual ICollection<Ubicacion> Ubicacion { get; set; }
        public virtual ICollection<Cargo> Cargo { get; set; }
        public virtual ICollection<MG_Template> MG_Template { get; set; }
        public virtual ICollection<Categoria> Categoria { get; set; }
        public virtual ICollection<SubCategoria> SubCategoria { get; set; }
    }
}