using DemoScore.Models.Game;
using SCORM1.Models.MainGame;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoScore.Models.ViewModel
{
    public class AdminGameViewModel
    {
    }
    public class AdminGameSetting 

    {
        public int companyId { get; set; }
        [Display(Name = "Terminos del juego")]
        public string TermsGame { get; set; }
        public List<MG_SettingMp> ListSetting { get; set; }
        public int Sett_Id { get; set; }

        [Display(Name = "Intentos")]
        [Required(ErrorMessage = "Se Debe Contar con un número de intentos disponibles")]
        public int Sett_Attemps { get; set; }

        [Display(Name = "Inicio")]
        [Required(ErrorMessage = "Se Debe Contar con una fecha inicial")]
        [DataType(DataType.Date)]
        public DateTime Sett_InitialDate { get; set; }

        [Display(Name = "Finalización")]
        [Required(ErrorMessage = "Se Debe Contar con una fecha de finalización")]
        [DataType(DataType.Date)]
        public DateTime Sett_CloseDate { get; set; }

        [Display(Name = "Preguntas")]
        public int Sett_NumberOfQuestions { get; set; }

        [Display(Name = "Plantilla")]
        [Required(ErrorMessage = "Se Debe Contar con una plantilla para el juego")]
        public int Plan_Id { get; set; }
        public IEnumerable<SelectListItem> Sett_Templates { get; set; }
        public IEnumerable<SelectListItem> Sett_Audios { get; set; }
        [Display(Name = "Audio_Instruciones")]
        public string Sett_Audio1 { get; set; }
        [Display(Name = "Audio_Respuesta")]
        public string Sett_Audio2 { get; set; }
        [Display(Name = "Audio_Inicio")]
        public string Sett_Audio3 { get; set; }
        [Display(Name = "Audio_Pregunta")]
        public string Sett_Audio4 { get; set; }
        [Display(Name = "Audio_Ganador")]
        public string Sett_Audio5 { get; set; }
    }

    public class AdminTemplate
    {
        public List<MG_Template> ListTemplate { get; set; }
        public int Plant_Id { get; set; }
        public string Plant_ImagePath { get; set; }
    }

    public class AdminGameQuestionViewModel
    {
        public int Sett_Id { get; set; }
        public MG_SettingMp Setting { get; set; }
        public int TotalQuestion { get; set; }
        //MultipleChoice
        public int MuCh_ID { get; set; }
        [Display(Name = "Descripción")]
        public string MuCh_Description { get; set; }
        [Display(Name = "Pregunta")]
        public string MuCh_NameQuestion { get; set; }
        [Display(Name = "Retroalimentación")]
        public string MuCh_Feedback { get; set; }
        [Display(Name = "Nivel")]
        public LEVEL MuCh_Level { get; set; }
        //AnswerMultipleChoice
        public int AnMu_Id { get; set; }
        [Display(Name = "Respuesta1")]
        public string AnMul_Description { get; set; }
        [Display(Name = "Estado1")]
        public OPTIONANSWER AnMul_TrueAnswer { get; set; }
        [Display(Name = "Respuesta2")]
        public string AnMul_Description2 { get; set; }
        [Display(Name = "Estado2")]
        public OPTIONANSWER AnMul_TrueAnswer2 { get; set; }
        [Display(Name = "Respuesta3")]
        public string AnMul_Description3 { get; set; }
        [Display(Name = "Estado3")]
        public OPTIONANSWER AnMul_TrueAnswer3 { get; set; }
        [Display(Name = "Respuesta4")]
        public string AnMul_Description4 { get; set; }
        [Display(Name = "Estado4")]
        public OPTIONANSWER AnMul_TrueAnswer4 { get; set; }
        public IEnumerable<SelectListItem> Sett_Category { get; set; }
        public IEnumerable<SelectListItem> Sett_SubCategory { get; set; }
        public IEnumerable<SelectListItem> Sett_Nivel { get; set; }
        public int Nivel_Id { get; set; }
        public int state { get; set; }
        public int cate_Id { get; set; }
        public int sub_Id { get; set; }
        public string Cate_Name { get; set; }
        public string SubC_name { get; set; }
        public List<Categoria> Listcate { get; set; }
        public List<SubCategoria>listsub { get; set; }
        public List<Nivel> Listnivel { get; set; }
        public int company_Id { get; set; }


    }
    public class Mg_MultipleChoise 
    {
        public int OpMu_Id { get; set; }
        [Display(Name = "Pregunta")]
        public string OpMu_Question { get; set; }
        [Display(Name = "Puntaje")]
        public int AnOp_Id { get; set; }
        [Display(Name = "Descripción Pregunta")]
        public string OpMu_Description { get; set; }
        //Answer Option Multiple
        public int Sett_Id { get; set; }
        public List<MG_AnswerMultipleChoice> listanswer { get; set; }


    }

    public class AdmingameMassiveQuestion
    {
        public MG_SettingMp Setting { get; set; }
    }
}