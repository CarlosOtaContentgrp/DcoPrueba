
using DemoScore.Models;
using DemoScore.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models.MainGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoScore.Controllers
{
    public class ViewGameUserController : Controller
    {
            protected ApplicationDbContext ApplicationDbContext { get; set; }
            protected UserManager<ApplicationUser> UserManager { get; set; }
            public ViewGameUserController()
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
            public ActionResult TermsGame()
            {
                int CompanyUser = (int)GetActualUserId().CompanyId;
                var terms = ApplicationDbContext.MG_SettingMps.Where(x => x.Company_Id == CompanyUser).FirstOrDefault();
                if (GetActualUserId().TermsandConditions == Terms_and_Conditions.No)
                {
                    UserGame model = new UserGame { };
                    model.FileTerms = terms.Sett_Instruction;
                    return PartialView("_TermsGame", model);
                }
                else
                {
                    return RedirectToAction("Preview", "ViewGameuser", new { Id = terms.Sett_Id });
                }

            }
            [Authorize]
            public ActionResult Validateterms(UserGame model)
            {
                if (model.termsandGame == true)
                {
                    int CompanyUser = (int)GetActualUserId().CompanyId;
                    var id = GetActualUserId().Id;
                    ApplicationUser user = ApplicationDbContext.Users.Find(id);
                    user.TermsandConditions = Terms_and_Conditions.Si;
                    ApplicationDbContext.SaveChanges();
                    var terms = ApplicationDbContext.MG_SettingMps.Where(x => x.Company_Id == CompanyUser).FirstOrDefault();
                    return RedirectToAction("Preview", "ViewGameuser", new { Id = terms.Sett_Id });
                }
                else
                {
                    TempData["Info"] = "Para ingresar al contenido debe aceptar los terminos primero";
                    return RedirectToAction("Index", "Home");
                }
            }

            [Authorize]
            public ActionResult Preview(int Id)
            {
                var setting = ApplicationDbContext.MG_SettingMps.Find(Id);
                UserPreviw model = new UserPreviw
                {
                    setting = setting,
                    Sett_Id = setting.Sett_Id
                };
                return PartialView("_Preview",model);
            }

            [Authorize]
            public ActionResult Instructions(int Id)
            {
                var setting = ApplicationDbContext.MG_SettingMps.Find(Id);
                setting.MG_MultipleChoice = ApplicationDbContext.MG_MultipleChoices.Where(x => x.Nivel_Id == 1 && x.Sett_Id == Id).ToList();
                int cont = 0;
                var nivelattemp = 1;
                var useractual = ApplicationDbContext.Users.Find(GetActualUserId().Id);
                var countattempnivel = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id == nivelattemp).Count();
                var counquestionnivel = ApplicationDbContext.MG_MultipleChoices.Where(x => x.Nivel_Id == nivelattemp && x.Sett_Id == Id).Count();
                var temp = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.AnUs_Id).ToList();
                var b = temp.FirstOrDefault();
                    if (temp.Count != 0)
                    {
                    
                        nivelattemp = b.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id;
                    }
                var attempts = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id == nivelattemp).OrderByDescending(x => x.AnUs_Id).ToList();
                var a = attempts.GroupBy(x => x.Attemps).ToList();
                int attemptsUser = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id == nivelattemp).GroupBy(x => x.Attemps).Count();
                if (attempts.Count != 0)
                {
               
                
                
                // numero de preguntas de la empresa en ese nivel
                counquestionnivel = ApplicationDbContext.MG_MultipleChoices.Where(x => x.Nivel_Id == nivelattemp && x.Sett_Id == Id).Count();
                // numero de preguntas respondidas por el usuario en ese nivel
                countattempnivel = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id == nivelattemp).Count();
                if (countattempnivel >= counquestionnivel) {

                   
                    if (nivelattemp < 3)
                    {
                        TempData["ultimo"] = "¡Felicidades has pasado el nivel sigue participando hasta el último nivel!";
                        nivelattemp = nivelattemp + 1;
                        countattempnivel = 0;
                    }
                    else
                    {
                       
                        TempData["ultimo"] = "¡Gracias por tu participación!";
                        TempData["ultimo1"] = "¡Ya puedes cerrar tu sesión!";
                    }
                }
                setting.MG_MultipleChoice = ApplicationDbContext.MG_MultipleChoices.Where(x => x.Nivel_Id == nivelattemp && x.Sett_Id == Id).ToList();
                //setting.MG_MultipleChoice=mg_setting;


                int att = ApplicationDbContext.MG_AnswerUsers.Where(x => x.Attemps == b.Attemps  && x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id== nivelattemp).Count();
                cont = att + 1;
                if (att == counquestionnivel)
                {
                    if (TempData["ultimo"]==null)
                    {
                        if (b.Attemps == setting.Sett_Attemps)
                        {
                            TempData["ultimo"] = "¡Gracias por tu participación!";
                            TempData["ultimo1"] = "¡Ya puedes cerrar tu sesión!";
                        }
                        else
                        {
                            TempData["ultimo"] = "¡Felicidades has pasado el nivel sigue participando hasta el último nivel!";
                        }

                        if (attemptsUser == setting.Sett_Attemps)
                        {
                            cont = 0;
                        }
                        else
                        {
                            cont = 1;
                        }
                    }
                   
                }
                }
                
                
                
                else
                {
                    cont = 1;
                }
           
                UserPreviw model1 = new UserPreviw
                {
                    setting = setting,
                    Sett_Id = setting.Sett_Id,
                    contador = countattempnivel,
                    User_Id = useractual.Id,
                    attemptUser = nivelattemp,
                    countquestion=counquestionnivel,
                    initial=0
                };
                return View(model1);
            }

        [Authorize]
        public ActionResult Question()
        {
            //int level = 0;
            var useractual = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var company = ApplicationDbContext.Companies
                .Join(ApplicationDbContext.Categorias,
                        c => c.CompanyId,
                        cat => cat.Company_Id,
                        (c, cat) => new { c, cat })
                .Join(ApplicationDbContext.MG_MultipleChoices,
                        mch => mch.cat.Cate_ID,
                        mc => mc.Cate_Id,
                       (mch, mc) => new { mch, mc })
                .Join(ApplicationDbContext.Users,
                        u => u.mch.c.CompanyId,
                        com => com.CompanyId,
                       (u, com) => new { u, com })
                       .Where(us => us.com.Id == useractual.Id).FirstOrDefault();
            var nivelattemp = 1;
            var temp = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.AnUs_Id).ToList();
            var b2 = temp.FirstOrDefault();
            if (temp.Count != 0)
            {
                nivelattemp = b2.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id;
            }
            // numero de preguntas de la empresa en ese nivel
            var counquestionnivel = ApplicationDbContext.MG_MultipleChoices.Where(x => x.Nivel_Id == nivelattemp && x.Sett_Id == company.u.mc.Sett_Id).Count();
            // numero de preguntas respondidas por el usuario en ese nivel
            var countattempnivel = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id == nivelattemp).Count();
            if (countattempnivel >= counquestionnivel)
            {
                nivelattemp = nivelattemp + 1;
            }
            var attempts = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id == nivelattemp).OrderByDescending(x => x.AnUs_Id).ToList();
            var setting = ApplicationDbContext.MG_SettingMps.First(x => x.Company_Id == useractual.CompanyId);
            setting.MG_MultipleChoice = ApplicationDbContext.MG_MultipleChoices.Where(x => x.Nivel_Id == nivelattemp && x.Sett_Id == setting.Sett_Id).ToList();
            //var attempts = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id).OrderByDescending(x => x.AnUs_Id).ToList();
            List<MultipleChoiceselect> listselect = new List<MultipleChoiceselect>();
            List<MultipleChoiceselect> listselect2 = new List<MultipleChoiceselect>();
            List<MultipleChoiceselect> listselect3 = new List<MultipleChoiceselect>();
            List<MultipleChoiceselect> listselect4 = new List<MultipleChoiceselect>();
            List<MultipleChoiceselect> listselect5 = new List<MultipleChoiceselect>();
            List<MultipleChoiceselect> ListFinalfacil = new List<MultipleChoiceselect>();
           /*  if (attempts.Count != 0)
            {
                var att = attempts.GroupBy(x => x.Attemps).ToList();
                var a = att.FirstOrDefault();
                var b = a.FirstOrDefault();                     
                int cant = a.Count();
                var attuser = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id).ToList();
                            foreach (var item1 in attuser)
                            {
                    listselect.Add(new MultipleChoiceselect
                    {
                        Sett_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Sett_Id,
                        MuCh_ID = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID,
                        MuCh_NameQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_NameQuestion,
                        MuCh_Description = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                        MuCh_ImageQuestion = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ImageQuestion,
                        MuCh_Categoria = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Categoria.Cate_Description,
                        MuCh_SubCategoria = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.SubCategoria.SubC_Description,
                        Cate_Id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id,
                        listanswerM = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.MG_AnswerMultipleChoice.ToList(),
                        nivel = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Nivel_Id                                    
                                });
                            }
                var categoria = ApplicationDbContext.Categorias.Where(x=>x.Company_Id==useractual.CompanyId).ToList();
                int pre = 45 / categoria.Count + 1;
                foreach (var item in categoria)
                {
                    /// cambio aqui de la condicion quetion
                    foreach (var item1 in setting.MG_MultipleChoice.Where(X => X.Cate_Id == item.Cate_ID))
                    {
                        listselect.Add(new MultipleChoiceselect
                        {
                            Sett_Id = item1.Sett_Id,
                            MuCh_ID = item1.MuCh_ID,
                            MuCh_NameQuestion = item1.MuCh_NameQuestion,
                            MuCh_Description = item1.MuCh_Description,
                            MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                            MuCh_Categoria=item1.Categoria.Cate_Description,
                            MuCh_SubCategoria=item1.SubCategoria.SubC_Description,
                            Cate_Id = item1.Cate_Id,
                            listanswerM = item1.MG_AnswerMultipleChoice.ToList(),
                            nivel=item1.Nivel_Id
                        });
                     
                    }
                }
               
                            var aa = listselect.GroupBy(x => x.MuCh_ID).ToList();
                            foreach (var item in aa)
                            {
                                if (item.Count() == 1)
                                {
                                    foreach (var item1 in item)
                                    {
                                        listselect2.Add(new MultipleChoiceselect
                                        {
                                            Sett_Id = item1.Sett_Id,
                                            MuCh_ID = item1.MuCh_ID,
                                            MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                            MuCh_Description = item1.MuCh_Description,
                                            MuCh_Categoria=item1.MuCh_Categoria,
                                            MuCh_SubCategoria=item1.MuCh_SubCategoria,
                                            MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                            MuCh_Level = item1.MuCh_Level,
                                            Cate_Id=item1.Cate_Id,
                                            listanswerM = item1.listanswerM.ToList(),
                                            nivel=item1.nivel
                                        });
                                    }
                                }
                            }
                var rnd1 = new Random();
                var randomselect1 = listselect2.OrderBy(x => rnd1.Next()).ToList();
                var answer = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == useractual.Id).GroupBy(x => x.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id).ToList();

                foreach (var item in categoria)
                {
                    foreach (var item1 in answer.ToList())
                    {
                        int q = item1.Count();
                        if (q >= pre)
                        {
                            var cate = item1.FirstOrDefault();
                            foreach (var item2 in randomselect1.Where(x => x.Cate_Id != cate.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id))
                            {
                                listselect3.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item2.Sett_Id,
                                    MuCh_ID = item2.MuCh_ID,
                                    MuCh_NameQuestion = item2.MuCh_NameQuestion,
                                    MuCh_Description = item2.MuCh_Description,
                                    MuCh_Categoria = item2.MuCh_Categoria,
                                    MuCh_SubCategoria = item2.MuCh_SubCategoria,
                                    MuCh_ImageQuestion = item2.MuCh_ImageQuestion,
                                    Cate_Id = item2.Cate_Id,
                                    listanswerM = item2.listanswerM.ToList(),
                                    nivel=item2.nivel
                                });
                            }
                        }
                        else
                        {
                            var cate = item1.FirstOrDefault();
                            foreach (var item2 in randomselect1.Where(x => x.Cate_Id == cate.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id))
                            {
                                listselect3.Add(new MultipleChoiceselect
                                {
                                    Sett_Id = item2.Sett_Id,
                                    MuCh_ID = item2.MuCh_ID,
                                    MuCh_NameQuestion = item2.MuCh_NameQuestion,
                                    MuCh_Description = item2.MuCh_Description,
                                    MuCh_Categoria = item2.MuCh_Categoria,
                                    MuCh_SubCategoria = item2.MuCh_SubCategoria,
                                    MuCh_ImageQuestion = item2.MuCh_ImageQuestion,
                                    Cate_Id = item2.Cate_Id,
                                    listanswerM = item2.listanswerM.ToList(),
                                    nivel=item2.nivel
                                });
                            }
                        }

                    }
                }
                var aaa = listselect3.GroupBy(x => x.MuCh_ID).ToList();
                foreach (var item in aa)
                {
                    if (item.Count() == 1)
                    {
                        foreach (var item1 in item)
                        {
                            listselect4.Add(new MultipleChoiceselect
                            {
                                Sett_Id = item1.Sett_Id,
                                MuCh_ID = item1.MuCh_ID,
                                MuCh_NameQuestion = item1.MuCh_NameQuestion,
                                MuCh_Description = item1.MuCh_Description,
                                MuCh_Categoria = item1.MuCh_Categoria,
                                MuCh_SubCategoria = item1.MuCh_SubCategoria,
                                MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                                MuCh_Level = item1.MuCh_Level,
                                Cate_Id = item1.Cate_Id,
                                listanswerM = item1.listanswerM.ToList(),
                                nivel=item1.nivel
                            });
                        }
                    }
                }
                var rnd2 = new Random();
                var randomselect2 = listselect4.OrderBy(x => rnd1.Next()).ToList();
                foreach (var item in categoria)
                {
                    foreach (var item1 in randomselect2.Where(x=>x.Cate_Id==item.Cate_ID))
                    {
                        listselect5.Add(new MultipleChoiceselect
                        {
                            Sett_Id = item1.Sett_Id,
                            MuCh_ID = item1.MuCh_ID,
                            MuCh_NameQuestion = item1.MuCh_NameQuestion,
                            MuCh_Description = item1.MuCh_Description,
                            MuCh_Categoria = item1.MuCh_Categoria,
                            MuCh_SubCategoria = item1.MuCh_SubCategoria,
                            MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                            Cate_Id=item1.Cate_Id,
                            listanswerM = item1.listanswerM.ToList(),
                            nivel=item1.nivel
                        });

                    }
                }
                            var rnd = new Random();
                            var randomselect = listselect5.OrderBy(x => rnd.Next()).ToList();
                            var Newlistselect = randomselect.Take(1);
                            ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);            
            }
            else
            {*/
                var categoria = ApplicationDbContext.Categorias.Where(x => x.Company_Id == useractual.CompanyId).ToList();
                foreach (var item in categoria)
                {
                    /// cambio aqui de la condicion quetion
                    foreach (var item1 in setting.MG_MultipleChoice.Where(X=>X.Cate_Id==item.Cate_ID && X.Nivel_Id== nivelattemp))
                    {
                        listselect.Add(new MultipleChoiceselect
                        {
                            Sett_Id = item1.Sett_Id,
                            MuCh_ID = item1.MuCh_ID,
                            MuCh_NameQuestion = item1.MuCh_NameQuestion,
                            MuCh_Description = item1.MuCh_Description,
                            MuCh_ImageQuestion = item1.MuCh_ImageQuestion,
                            Cate_Id=item1.Cate_Id,
                            listanswerM = item1.MG_AnswerMultipleChoice.ToList(),
                            nivel=item1.Nivel_Id
                        });
                    }
                }               
                var rnd = new Random();
                var randomselect = listselect.OrderBy(x => rnd.Next()).ToList();
                var Newlistselect = randomselect.Take(1);
                ListFinalfacil = new List<MultipleChoiceselect>(Newlistselect);
           // }
            Session["Date"] = DateTime.Now;
            QuestionSelect model1 = new QuestionSelect
            {
                Sett_Id = setting.Sett_Id,
                listquestionsselect = ListFinalfacil,
                setting = setting,
            };
            return View(model1);
        }
        [Authorize]
        public ActionResult AnswerQuestions(int id)
        {
            DateTime fechainicio = new DateTime();
            fechainicio = (DateTime)Session["Date"];
            DateTime fechaenvio = DateTime.Now;          
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);           
            var answer = ApplicationDbContext.MG_AnswerMultipleChoices.Find(id);
                if (ValidQuestion(id) == true)
                {
                    var a = ApplicationDbContext.MG_AnswerMultipleChoices.Where(x => x.MuCh_ID == answer.MuCh_ID).ToList();
                    var z = a.Single(x => x.AnMul_TrueAnswer == OPTIONANSWER.Verdadero);
                    var attempts = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.AnUs_Id).ToList();
                    if (z.AnMul_ID == id)
                    {
                        if (attempts.Count != 0)
                        {
                            var atte = attempts.FirstOrDefault();
                            var att1 = attempts.GroupBy(x => x.Attemps).ToList();
                            var gru = att1.FirstOrDefault();
                         
                                int att = ApplicationDbContext.MG_AnswerUsers.Where(x => x.Attemps == atte.Attemps && x.User_Id == user.Id).Count();
                                if (att == 15)
                                {
                                    MG_AnswerUser antiguo = new MG_AnswerUser
                                    {
                                        User_Id = user.Id,
                                        Attemps = atte.Attemps + 1,
                                        ApplicationUser = user,
                                        Respuesta = RESPUESTA.Correcta,
                                        AnMul_ID = answer.AnMul_ID,
                                        MG_AnswerMultipleChoice = answer,
                                        FechaIngreso = fechainicio,
                                        FechaEnvio = fechaenvio
                                    };
                                    ApplicationDbContext.MG_AnswerUsers.Add(antiguo);                                   
                                    ApplicationDbContext.SaveChanges();
                                    TempData["Info1"] = "Respuesta Correcta.";
                                }
                                if (att == 14)
                                {
                                    MG_AnswerUser antiguo = new MG_AnswerUser
                                    {
                                        User_Id = user.Id,
                                        Attemps = atte.Attemps,
                                        ApplicationUser = user,
                                        Respuesta = RESPUESTA.Correcta,
                                        AnMul_ID = answer.AnMul_ID,
                                        MG_AnswerMultipleChoice = answer,
                                        FechaIngreso = fechainicio,
                                        FechaEnvio = fechaenvio
                                    };
                                    ApplicationDbContext.MG_AnswerUsers.Add(antiguo);
                                    ApplicationDbContext.SaveChanges();
                                }
                                if (att < 14)

                                {
                                    MG_AnswerUser antiguo = new MG_AnswerUser
                                    {
                                        User_Id = user.Id,
                                        Attemps = atte.Attemps,
                                        ApplicationUser = user,
                                        Respuesta = RESPUESTA.Correcta,
                                        AnMul_ID = answer.AnMul_ID,
                                        MG_AnswerMultipleChoice = answer,
                                        FechaIngreso = fechainicio,
                                        FechaEnvio = fechaenvio,
                                    };
                                    ApplicationDbContext.MG_AnswerUsers.Add(antiguo);
                                    ApplicationDbContext.SaveChanges();
                                    TempData["Info1"] = "Respuesta Guardada.";
                                }
                      
                        }
                        else
                        {
                            MG_AnswerUser nuevo = new MG_AnswerUser
                            {
                                User_Id = user.Id,
                                Attemps = 1,
                                ApplicationUser = user,
                                AnMul_ID = answer.AnMul_ID,
                                Respuesta = RESPUESTA.Correcta,
                                MG_AnswerMultipleChoice = answer,
                                FechaIngreso = fechainicio,
                                FechaEnvio = fechaenvio,
                            };
                            ApplicationDbContext.MG_AnswerUsers.Add(nuevo);
                            ApplicationDbContext.SaveChanges();                          
                            TempData["Info1"] = "Respuesta Guardada.";
                        }
                    }
                    else
                    {
                        if (attempts.Count != 0)
                        {
                            var att = attempts.GroupBy(x => x.Attemps).ToList();
                            var gru = att.FirstOrDefault();
                            var atte = attempts.FirstOrDefault();
                                int atta = ApplicationDbContext.MG_AnswerUsers.Where(x => x.Attemps == atte.Attemps && x.User_Id == user.Id).Count();
                                if (atta == 15)
                                {
                                    MG_AnswerUser antiguo = new MG_AnswerUser
                                    {
                                        User_Id = user.Id,
                                        Attemps = atte.Attemps + 1,
                                        ApplicationUser = user,
                                        Respuesta = RESPUESTA.Incorrecta,
                                        AnMul_ID = answer.AnMul_ID,
                                        MG_AnswerMultipleChoice = answer,
                                        FechaIngreso = fechainicio,
                                        FechaEnvio = fechaenvio
                                    };
                                    ApplicationDbContext.MG_AnswerUsers.Add(antiguo);
                                    ApplicationDbContext.SaveChanges();
                                TempData["Info1"] = "Respuesta Correcta.";
                            }
                            if (atta == 14)
                            {
                                MG_AnswerUser antiguo = new MG_AnswerUser
                                {
                                    User_Id = user.Id,
                                    Attemps = atte.Attemps,
                                    ApplicationUser = user,
                                    Respuesta = RESPUESTA.Incorrecta,
                                    AnMul_ID = answer.AnMul_ID,
                                    MG_AnswerMultipleChoice = answer,
                                    FechaIngreso = fechainicio,
                                    FechaEnvio = fechaenvio
                                };
                                ApplicationDbContext.MG_AnswerUsers.Add(antiguo);
                                ApplicationDbContext.SaveChanges();
                            }
                            if (atta < 14)

                            {
                                MG_AnswerUser antiguo = new MG_AnswerUser
                                {
                                    User_Id = user.Id,
                                    Attemps = atte.Attemps,
                                    ApplicationUser = user,
                                    Respuesta = RESPUESTA.Incorrecta,
                                    AnMul_ID = answer.AnMul_ID,
                                    MG_AnswerMultipleChoice = answer,
                                    FechaIngreso = fechainicio,
                                    FechaEnvio = fechaenvio,
                                };
                                ApplicationDbContext.MG_AnswerUsers.Add(antiguo);
                                ApplicationDbContext.SaveChanges();
                                TempData["Info1"] = "Respuesta Guardada.";
                            }
              
                        }
                        else
                        {
                            MG_AnswerUser nuevo = new MG_AnswerUser
                            {
                                User_Id = user.Id,
                                Attemps = 1,
                                ApplicationUser = user,
                                Respuesta = RESPUESTA.Incorrecta,
                                AnMul_ID = answer.AnMul_ID,
                                MG_AnswerMultipleChoice = answer,
                                FechaIngreso = fechainicio,
                                FechaEnvio = fechaenvio
                            };
                            ApplicationDbContext.MG_AnswerUsers.Add(nuevo);
                            ApplicationDbContext.SaveChanges();
                        TempData["Info1"] = "Respuesta Guardada.";
                    }
                        ApplicationDbContext.SaveChanges();
                   
                }
                    ApplicationDbContext.SaveChanges();
                return RedirectToAction("Instructions", new { Id = answer.MG_MultipleChoice.Sett_Id });
            }
                else
                {
                TempData["ultimo"] = "¡Ya has respondido esta pregunta!";
                return RedirectToAction("Instructions", new { Id = answer.MG_MultipleChoice.Sett_Id });
                }
            }

        public bool ValidQuestion(int Id)
        {
            var user = ApplicationDbContext.Users.Find(GetActualUserId().Id);
            var ans = ApplicationDbContext.MG_AnswerMultipleChoices.Find(Id);      
                var valpre = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == user.Id && x.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_ID == ans.MuCh_ID).ToList();
            if (valpre.Count != 0)
            {
                
                DateTime fechainicio = new DateTime();
                fechainicio = (DateTime)Session["Date"];
                DateTime fechaenvio = DateTime.Now;
                var answer = ApplicationDbContext.MG_AnswerMultipleChoices.Find(Id);
                var attempts = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.AnUs_Id).ToList();
                if (attempts.Count != 0)
                {
                    var atte = attempts.FirstOrDefault();


                    if (atte.Respuesta == RESPUESTA.Incorrecta)
                    {
                        int atta = ApplicationDbContext.MG_AnswerUsers.Where(x => x.Attemps == atte.Attemps && x.User_Id == user.Id).Count();
                        if (atta == 15)
                        {
                            if (atte.Attemps == answer.MG_MultipleChoice.MG_SettingMp.Sett_Attemps)
                            {
                                return false;
                            }
                            else
                            {
                                MG_AnswerUser antiguo = new MG_AnswerUser
                                {
                                    User_Id = user.Id,
                                    Attemps = atte.Attemps + 1,
                                    ApplicationUser = user,
                                    Respuesta = RESPUESTA.Incorrecta,
                                    AnMul_ID = answer.AnMul_ID,
                                    MG_AnswerMultipleChoice = answer,
                                    FechaIngreso = fechainicio,
                                    FechaEnvio = fechaenvio
                                };
                                ApplicationDbContext.MG_AnswerUsers.Add(antiguo);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            MG_AnswerUser antiguo = new MG_AnswerUser
                            {
                                User_Id = user.Id,
                                Attemps = atte.Attemps,
                                ApplicationUser = user,
                                Respuesta = RESPUESTA.Incorrecta,
                                AnMul_ID = answer.AnMul_ID,
                                MG_AnswerMultipleChoice = answer,
                                FechaIngreso = fechainicio,
                                FechaEnvio = fechaenvio
                            };
                            ApplicationDbContext.MG_AnswerUsers.Add(antiguo);
                            ApplicationDbContext.SaveChanges();
                        }

                    }
                    else
                    {
                        int atta = ApplicationDbContext.MG_AnswerUsers.Where(x => x.Attemps == atte.Attemps && x.User_Id == user.Id).Count();
                        if (atta == 15)
                        {
                            if (atte.Attemps == answer.MG_MultipleChoice.MG_SettingMp.Sett_Attemps)
                            {
                                return false;
                            }
                            else
                            {
                                MG_AnswerUser antiguo = new MG_AnswerUser
                                {
                                    User_Id = user.Id,
                                    Attemps = atte.Attemps + 1,
                                    ApplicationUser = user,
                                    Respuesta = RESPUESTA.Incorrecta,
                                    AnMul_ID = answer.AnMul_ID,
                                    MG_AnswerMultipleChoice = answer,
                                    FechaIngreso = fechainicio,
                                    FechaEnvio = fechaenvio
                                };
                                ApplicationDbContext.MG_AnswerUsers.Add(antiguo);
                                ApplicationDbContext.SaveChanges();
                            }

                        }
                        else
                        {
                            MG_AnswerUser antiguo = new MG_AnswerUser
                            {
                                User_Id = user.Id,
                                Attemps = atte.Attemps,
                                ApplicationUser = user,
                                Respuesta = RESPUESTA.Incorrecta,
                                AnMul_ID = answer.AnMul_ID,
                                MG_AnswerMultipleChoice = answer,
                                FechaIngreso = fechainicio,
                                FechaEnvio = fechaenvio
                            };
                            ApplicationDbContext.MG_AnswerUsers.Add(antiguo);
                            ApplicationDbContext.SaveChanges();
                        }

                    }
                }
                else
                {
                    MG_AnswerUser nuevo = new MG_AnswerUser
                    {
                        User_Id = user.Id,
                        Attemps = 1,
                        ApplicationUser = user,
                        Respuesta = RESPUESTA.Incorrecta,
                        AnMul_ID = answer.AnMul_ID,
                        MG_AnswerMultipleChoice = answer,
                        FechaIngreso = fechainicio,
                        FechaEnvio = fechaenvio
                    };
                    ApplicationDbContext.MG_AnswerUsers.Add(nuevo);
                    ApplicationDbContext.SaveChanges();
                }
                ApplicationDbContext.SaveChanges();

                return false;
            }
            else
            {
                var attempts = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.AnUs_Id).ToList();
                if (attempts.Count !=0)
                {
                    var atte = attempts.FirstOrDefault();
                    int atta = ApplicationDbContext.MG_AnswerUsers.Where(x => x.Attemps == atte.Attemps && x.User_Id == user.Id).Count();
                    if (atta == 15)
                    {
                        if (atte.Attemps == ans.MG_MultipleChoice.MG_SettingMp.Sett_Attemps)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
               

            }
        }



    }
    }