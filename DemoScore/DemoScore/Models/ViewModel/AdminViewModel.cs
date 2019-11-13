using DemoScore.Models.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoScore.Models.ViewModel
{
    public class infocompany 
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int CompanyUser { get; set; }
    }
    public class AdminGeneralViewModel
    {
        public List<infocompany> ListCompanies { get; set; }

        public Company CompanyToUpdate { get; set; }

        public int CompanyId { get; set; }

        [Display(Name = "Nombre")]
        public string CompanyName { get; set; }
    }
    public class AdminReports 
    {
        public int company_Id { get; set;}
    }

    public class Resultadosusuario
    {
        internal string areanam;
        internal string loca_descri;
        //variable nueva
        internal string ubicacion;
        //fin variable nueva
        public string User_Id { get; set; }
        public int cate_id { get; set;}
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Categoria { get; set; }
        public int totalpreguntas { get; set; }
        public int PreguntasCorrectas { get; set; }
        public string Pregunta { get; internal set; }
        public int Nivel { get; internal set; }
        public RESPUESTA Resultado { get; internal set; }
        public string Respuesta { get; internal set; }
        
        

    }

    public class Resultadoscategorias
    {
        public int cate_id { get; set; }
        public string Categoria { get; set; }
        public int totalpreguntas { get; set; }
        public int PreguntasCorrectas { get; set; }

    }





}