using DemoScore.Models;
using DemoScore.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoScore.Controllers
{
    public class ViewGameController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public ViewGameController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

        [Authorize]
        public ActionResult Preview(int Id)
        {
            var setting = ApplicationDbContext.MG_SettingMps.Find(Id);
            AdminPreviw model = new AdminPreviw
            {
                setting = setting,
                Sett_Id = setting.Sett_Id
            };
            return View(model);
        }

        [Authorize]
        public ActionResult Instructions(int Id)
        {
            var setting = ApplicationDbContext.MG_SettingMps.Find(Id);
            List<MultipleChoiceDificil> ListQuestionsdificil = new List<MultipleChoiceDificil>();
            if (setting.MG_MultipleChoice.Count> 44)
            {
                foreach (var item in setting.MG_MultipleChoice)
                {
                    ListQuestionsdificil.Add(new MultipleChoiceDificil
                    {
                        Sett_Id = item.Sett_Id,
                        MuCh_ID = item.MuCh_ID,
                        MuCh_Description = item.MuCh_Description,
                        MuCh_ImageQuestion = item.MuCh_ImageQuestion,
                        listanswerM = item.MG_AnswerMultipleChoice.ToList()
                    });
                }
               
            }
            else
            {
                TempData["Info"] = "Por favor complete el banco de pregunta, el cual debe corresponder a 45 preguntas";
            }
           
            var rnd = new Random();
            //var randomlistdificil = ListQuestionsdificil.OrderBy(x => rnd.Next()).ToList();
            var Newlistdificil = ListQuestionsdificil;
            List<MultipleChoiceDificil> ListFinaldificil = new List<MultipleChoiceDificil>(Newlistdificil);
            AdminPreviw model = new AdminPreviw
            {
                setting = setting,
                Sett_Id = setting.Sett_Id,
                listquestionsDificil = ListFinaldificil
            };
            
            return View(model);
        }

        [Authorize]
        public ActionResult Question(int id, int contador)
        {
            var question = ApplicationDbContext.MG_MultipleChoices.Find(id);
            List<MultipleChoiceselect> listselect = new List<MultipleChoiceselect>();
            if (question != null)
            {
                listselect.Add(new MultipleChoiceselect
                {
                    MuCh_ID = question.MuCh_ID,
                    MuCh_Description = question.MuCh_Description,
                    MuCh_ImageQuestion = question.MuCh_ImageQuestion,
                    Sett_Id = question.Sett_Id,
                    listanswerM = question.MG_AnswerMultipleChoice.ToList()
                });
                QuestionSelect model = new QuestionSelect
                {
                    Sett_Id = question.Sett_Id,
                    contador = contador,
                    listquestionsselect = listselect,
                    setting = question.MG_SettingMp
                };
                return View(model);
            }
            return RedirectToAction("Instructions", new { Id = question.Sett_Id });
        }


    }
}