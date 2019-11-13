using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoScore.Models.Game
{
    public class Ciudad
    {
        [Key]
        public int City_Id { get; set; }
        [Display(Name = "Ciudad")]
        public string City_Name { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }
    }

}