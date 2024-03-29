﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DemoScore.Models.Game
{
    public class Cargo
    {
        [Key]
        public int Posi_id { get; set; }
        [Display(Name = "Descripción")]
        public string Posi_Description { get; set; }

        [ForeignKey("Company")]
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
    }
}