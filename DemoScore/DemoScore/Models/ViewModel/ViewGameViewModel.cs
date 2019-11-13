using Chart.Mvc.ComplexChart;
using SCORM1.Models.MainGame;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoScore.Models.ViewModel
{
    public class ViewGameViewModel
    {
    }

    public class AdminPreviw
    {
        public MG_SettingMp setting { get; set; }
        public int Sett_Id { get; set; }
        public int facil { get; set; }
        public int medio { get; set; }
        public int dificil { get; set; }
        public int contador { get; set; }
        public List<MultipleChoiceFacil> listquestionsFacil { get; set; }
        public List<MultipleChoiceMedio> listquestionsMedio { get; set; }
        public List<MultipleChoiceDificil> listquestionsDificil { get; set; }
        public List<listranking> ListRanking { get; set; }

    }

    public class UserGame 
    {
        [Display(Name = "Acepto Los terminos del juego.")]
        public bool termsandGame { get; set; }
        public string FileTerms { get; set; }

    }
    public class UserPreviw 
    {
        public int attemptUser { get; set; }
        public int attempts { get; set; }
        public string User_Id { get; set; }
        public ApplicationUser User { get; set; }
        public MG_SettingMp setting { get; set; }
        public int Sett_Id { get; set; }
        public int facil { get; set; }
        public int medio { get; set; }
        public int dificil { get; set; }
        public int contador { get; set; }
        public List<MultipleChoiceFacil> listquestionsFacil { get; set; }
        public List<MultipleChoiceMedio> listquestionsMedio { get; set; }
        public List<MultipleChoiceDificil> listquestionsDificil { get; set; }
        public List<listranking> ListRanking { get; set; }
        public List<listranking> ListRankingUser { get; set; }


    }

    public class MultipleChoiceFacil
    {
        public LEVEL MuCh_Level { get; set; }
        public int Sett_Id { get; set; }
        public int MuCh_ID { get; set; }
        public string MuCh_Description { get; set; }
        public string MuCh_NameQuestion { get; set; }
        public string MuCh_ImageQuestion { get; set; }
        public string MuCh_Feedback { get; set; }
        public int AnMul_ID { get; set; }
        public List<MG_AnswerMultipleChoice> listanswerM { get; set; }
    }
    public class MultipleChoiceMedio
    {
        public LEVEL MuCh_Level { get; set; }
        public int Sett_Id { get; set; }
        public int MuCh_ID { get; set; }
        public string MuCh_Description { get; set; }
        public string MuCh_NameQuestion { get; set; }
        public string MuCh_ImageQuestion { get; set; }
        public string MuCh_Feedback { get; set; }
        public int AnMul_ID { get; set; }
        public List<MG_AnswerMultipleChoice> listanswerM { get; set; }
    }
    public class MultipleChoiceDificil
    {
        public LEVEL MuCh_Level { get; set; }
        public int Sett_Id { get; set; }
        public int MuCh_ID { get; set; }
        public string MuCh_Description { get; set; }
        public string MuCh_NameQuestion { get; set; }
        public string MuCh_ImageQuestion { get; set; }
        public string MuCh_Feedback { get; set; }
        public int AnMul_ID { get; set; }
        public List<MG_AnswerMultipleChoice> listanswerM { get; set; }
    }

    public class QuestionSelect 
    {
        public string fechaingreso { get; set; }
        public int seg { get; set; }
        public MG_SettingMp setting { get; set; }
        public int Sett_Id { get; set; }
        public int facil { get; set; }
        public int medio { get; set; }
        public int dificil { get; set; }
        public int contador { get; set; }
        public List<MultipleChoiceselect> listquestionsselect { get; set; }
        public int cmd1 { get; set; }
        public int cmd2 { get; set; }
        public int v1 { get; set; }
        public int v2 { get; set; }
        public BarChart Result { get; set; }

    }
    public class MultipleChoiceselect
    {
        public LEVEL MuCh_Level { get; set; }
        public int Sett_Id { get; set; }
        public int MuCh_ID { get; set; }
        public int Cate_Id { get; set; }
        public string MuCh_Description { get; set; }
        public string MuCh_NameQuestion { get; set; }
        public string MuCh_ImageQuestion { get; set; }
        public string MuCh_Categoria { get; set; }
        public string MuCh_SubCategoria { get; set; }
        public int AnMul_ID { get; set; }
        public List<MG_AnswerMultipleChoice> listanswerM { get; set; }
    }

    public class listranking
    {
        public ApplicationUser user { get; set; }
        public int puntos { get; set; }
        public int Ranking { get; set; }
    }
}