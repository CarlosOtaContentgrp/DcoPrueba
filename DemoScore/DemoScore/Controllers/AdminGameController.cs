using DemoScore.Models;
using DemoScore.Models.Game;
using DemoScore.Models.ViewModel;
using Excel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SCORM1.Models.MainGame;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoScore.Controllers
{
    public class AdminGameController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public AdminGameController()
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


        public IEnumerable<SelectListItem> GetTemplate()
        {
            List<MG_Template> Templates = ApplicationDbContext.MG_Templates.ToList();
            IEnumerable<SelectListItem> Template = Templates.Select(x =>
           new SelectListItem
           {
               Value = x.Plant_Id.ToString(),
               Text = x.Plant_Color
           });
            return new SelectList(Template, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetAudios()
        {

            List<string> audios = new List<string>();
            audios.Add("Instrucciones.mp3");
            audios.Add("respuesta.mp3");
            audios.Add("inicio.mp3");
            audios.Add("pregunta.mp3");
            audios.Add("ganador.mp3");
            IEnumerable<SelectListItem> audio = audios.Select(x =>
           new SelectListItem
           {
               Value = x.ToString(),
               Text = x.ToString()
           });
            return new SelectList(audio, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetCategory(int com)
        {
            List<Categoria> category = ApplicationDbContext.Categorias.Where(x => x.Company_Id == com).ToList();
            IEnumerable<SelectListItem> Template = category.Select(x =>
           new SelectListItem
           {
               Value = x.Cate_ID.ToString(),
               Text = x.Cate_Description
           });
            return new SelectList(Template, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetSubcategory(int com)
        {

            List<SubCategoria> subcate = ApplicationDbContext.SubCategorias.Where(x => x.Company_Id == com).ToList();
            IEnumerable<SelectListItem> Template = subcate.Select(x =>
           new SelectListItem
           {
               Value = x.SubC_ID.ToString(),
               Text = x.SubC_Description
           });
            return new SelectList(Template, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetNivel()
        {
            List<Nivel> nivel = ApplicationDbContext.Nivels.ToList();
            IEnumerable<SelectListItem> Template = nivel.Select(x =>
           new SelectListItem
           {
               Value = x.Nivel_Id.ToString(),
               Text = x.Nivel_Name
           });
            return new SelectList(Template, "Value", "Text");
        }

        public ActionResult Companies()
        {
            List<Company> Companies = ApplicationDbContext.Companies.ToList();
            List<infocompany> listcompany = new List<infocompany>();
            if (Companies.Count != 0)
            {


                foreach (var item in Companies)
                {
                    int user = ApplicationDbContext.Users.Where(X => X.CompanyId == item.CompanyId).ToList().Count();
                    listcompany.Add(new infocompany
                    {
                        CompanyId = item.CompanyId,
                        CompanyName = item.CompanyName,
                        CompanyUser = user
                    });
                }
            }
            AdminGeneralViewModel model = new AdminGeneralViewModel
            {
                ListCompanies = listcompany
            };
            return View(model);
        }
        [Authorize]
        public ActionResult Setting(int id)
        {
            var company = ApplicationDbContext.Companies.Find(id);
            var setting = ApplicationDbContext.MG_SettingMps.Where(x => x.Company_Id == company.CompanyId).ToList();
            var a = "";
            if (setting.Count != 0)
            {
                a = setting.FirstOrDefault().Sett_Instruction;
            }
            else
            {
                var settings = ApplicationDbContext.MG_SettingMps.FirstOrDefault();
                a = settings.Sett_Instruction;
            }
            AdminGameSetting model = new AdminGameSetting
            {
                ListSetting = setting,
                Sett_Templates = GetTemplate(),
                Sett_Audios = GetAudios(),
                TermsGame = a,
                companyId = id
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddSetting(AdminGameSetting model)
        {
            var GetCompanyId = ApplicationDbContext.Companies.Find(model.companyId);
            if (ModelState.IsValid)
            {
                var template = ApplicationDbContext.MG_Templates.Find(model.Plan_Id);

                MG_SettingMp Setting = new MG_SettingMp
                {

                    Sett_InitialDate = model.Sett_InitialDate,
                    Sett_CloseDate = model.Sett_CloseDate,
                    MG_Template = template,
                    Company_Id = GetCompanyId.CompanyId,
                    Company = GetCompanyId,
                    Sett_Instruction = "20171017152016-administracion formacion - matriculas.pdf",
                    Sett_Audio1 = model.Sett_Audio1,
                    Sett_Audio2 = model.Sett_Audio2,
                    Sett_Audio3 = model.Sett_Audio3,
                    Sett_Audio4 = model.Sett_Audio4,
                    Sett_Audio5 = model.Sett_Audio5,
                    Sett_Attemps = 3
                };
                ApplicationDbContext.MG_SettingMps.Add(Setting);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Setting", new { id = model.companyId });
            }
            TempData["AddMessageError"] = "Se debe Crear una plantilla primero";
            return View("Setting", model);
        }

        [Authorize]
        public ActionResult DeleteSetting(int id)
        {
            MG_SettingMp deletedSetting = ApplicationDbContext.MG_SettingMps.Find(id);
            int com = deletedSetting.Company_Id;
            if (deletedSetting.MG_MultipleChoice.Count != 0)
            {
                TempData["Info"] = "No se puede eliminar la configuración del juego debido a que, se encuentra información asociada";
            }
            else
            {
                TempData["Info"] = "Configuración eliminada con éxito";
                ApplicationDbContext.MG_SettingMps.Remove(deletedSetting);
                ApplicationDbContext.SaveChanges();
            }

            return RedirectToAction("Setting", new { id = com });
        }

        [Authorize]
        public ActionResult UpdateSetting(int id)
        {

            MG_SettingMp updatedSetting = ApplicationDbContext.MG_SettingMps.Find(id);
            TempData["UpdateSetting"] = "Modificada con exito";
            AdminGameSetting model = new AdminGameSetting
            {

                ListSetting = ApplicationDbContext.MG_SettingMps.Where(c => c.Company_Id == updatedSetting.Company_Id).ToList(),
                Sett_Id = updatedSetting.Sett_Id,
                Sett_InitialDate = updatedSetting.Sett_InitialDate,
                Sett_CloseDate = updatedSetting.Sett_CloseDate,
                Plan_Id = updatedSetting.Plan_Id,
                Sett_Audio1 = updatedSetting.Sett_Audio1,
                Sett_Audio2 = updatedSetting.Sett_Audio2,
                Sett_Audio3 = updatedSetting.Sett_Audio3,
                Sett_Audio4 = updatedSetting.Sett_Audio4,
                Sett_Audio5 = updatedSetting.Sett_Audio5,
                Sett_Templates = GetTemplate(),
                Sett_Audios = GetAudios()
            };
            return View("Setting", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateSetting(AdminGameSetting model)
        {
            MG_SettingMp updatedSetting = ApplicationDbContext.MG_SettingMps.Find(model.Sett_Id);
            if (ModelState.IsValid)
            {

                TempData["Info"] = "La configuración se ha modificado con éxito.";
                updatedSetting.Sett_InitialDate = model.Sett_InitialDate;
                updatedSetting.Sett_CloseDate = model.Sett_CloseDate;
                updatedSetting.Plan_Id = model.Plan_Id;
                updatedSetting.MG_Template = ApplicationDbContext.MG_Templates.Find(model.Plan_Id);
                updatedSetting.Sett_Audio1 = model.Sett_Audio1;
                updatedSetting.Sett_Audio2 = model.Sett_Audio2;
                updatedSetting.Sett_Audio3 = model.Sett_Audio3;
                updatedSetting.Sett_Audio4 = model.Sett_Audio4;
                updatedSetting.Sett_Audio5 = model.Sett_Audio5;
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("Setting", new { id = updatedSetting.Company_Id });
            }
            else
            {
                TempData["Info"] = "Los campos no pueden ser vacios ";
                return RedirectToAction("Setting", new { id = updatedSetting.Company_Id });
            }
        }


        [Authorize]
        public ActionResult MgQuestions(int id)
        {
            MG_SettingMp setting = ApplicationDbContext.MG_SettingMps.Find(id);
            int TotalQuestion = setting.MG_MultipleChoice.Count();
            var cate = ApplicationDbContext.Categorias.Where(x => x.Company_Id == setting.Company_Id).ToList();
            var subcate = ApplicationDbContext.SubCategorias.Where(x => x.Company_Id == setting.Company_Id).ToList();           
            AdminGameQuestionViewModel model = new AdminGameQuestionViewModel
            {
                Sett_Id = id,
                Setting = setting,
                TotalQuestion = TotalQuestion,
                Sett_Category = GetCategory(setting.Company_Id),
                Sett_SubCategory = GetSubcategory(setting.Company_Id),
                Sett_Nivel=GetNivel(),
                Listcate = cate,
                listsub = subcate,
                company_Id = setting.Company_Id

            };
            return View(model);
        }

        [Authorize]
        public ActionResult AddCategory(AdminGameQuestionViewModel model)
        {
            var Getsetting = ApplicationDbContext.MG_SettingMps.Find(model.Sett_Id);
            var company = ApplicationDbContext.Companies.Find(Getsetting.Company_Id);
            if (model.Cate_Name != null)
            {
                TempData["Add"] = "Pregunta Creada con éxito";
                Categoria cat = new Categoria
                {
                    Cate_Description = model.Cate_Name,
                    Company_Id = company.CompanyId,
                    Company = company
                };
                ApplicationDbContext.Categorias.Add(cat);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
            else
            {
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
        }

        [Authorize]
        public ActionResult UpdateCategory(int id, int sett_Id)
        {
            var cate = ApplicationDbContext.Categorias.Find(id);
            var setting = ApplicationDbContext.MG_SettingMps.Find(sett_Id);
            AdminGameQuestionViewModel model = new AdminGameQuestionViewModel
            {
                Setting = setting,
                Sett_Id = setting.Sett_Id,
                cate_Id = cate.Cate_ID,
                Cate_Name = cate.Cate_Description,
                Sett_Category = GetCategory(setting.Company_Id),
                Sett_SubCategory = GetSubcategory(setting.Company_Id)
            };
            TempData["Form1"] = "Activar Formulario";
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateCategory(AdminGameQuestionViewModel model)
        {
            var updatecat = ApplicationDbContext.Categorias.Find(model.cate_Id);
            if (ModelState.IsValid)
            {

                TempData["Info"] = "categoria modificada";
                updatecat.Cate_Description = model.Cate_Name;
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
            else
            {
                TempData["Info"] = "Los campos no pueden ser vacios ";
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
        }


        [Authorize]
        public ActionResult DeleteCategory(int id)
        {
            var catego = ApplicationDbContext.Categorias.Find(id);
            var sett = ApplicationDbContext.MG_SettingMps.FirstOrDefault(x => x.Company_Id == catego.Company_Id);
            var answer = catego.MG_MultipleChoice.Count;
            if (answer == 0)
            {
                TempData["Add"] = "Categoria Eliminada";
                ApplicationDbContext.Categorias.Remove(catego);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("MgQuestions", new { id = sett.Sett_Id });
            }
            else
            {
                TempData["Add"] = "No se puede eliminar la categoria por que tiene preguntas asociadas";
                return RedirectToAction("MgQuestions", new { id = sett.Sett_Id });
            }
        }

        [Authorize]
        public ActionResult AddSubCategory(AdminGameQuestionViewModel model)
        {
            var Getsetting = ApplicationDbContext.MG_SettingMps.Find(model.Sett_Id);
            var company = ApplicationDbContext.Companies.Find(Getsetting.Company_Id);
            if (model.SubC_name != null)
            {
                var cate = ApplicationDbContext.Categorias.Find(model.cate_Id);
                TempData["Add"] = "SubCategoria creada";
                SubCategoria cat = new SubCategoria
                {
                    SubC_Description = model.SubC_name,
                    Company_Id = company.CompanyId,
                    Company = company,
                    Cate_Id = model.cate_Id,
                    Categoria = cate
                };
                ApplicationDbContext.SubCategorias.Add(cat);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
            else
            {
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
        }

        [Authorize]
        public ActionResult UpdateSubCategory(int id, int sett_Id)
        {
            var subcate = ApplicationDbContext.SubCategorias.Find(id);
            var setting = ApplicationDbContext.MG_SettingMps.Find(sett_Id);
            AdminGameQuestionViewModel model = new AdminGameQuestionViewModel
            {
                Setting = setting,
                Sett_Id = setting.Sett_Id,
                cate_Id = subcate.Cate_Id,
                SubC_name = subcate.SubC_Description,
                sub_Id = subcate.SubC_ID,
                Sett_Category = GetCategory(setting.Company_Id),
                Sett_SubCategory = GetSubcategory(setting.Company_Id)
            };
            TempData["Form2"] = "Activar Formulario";
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateSubCategory(AdminGameQuestionViewModel model)
        {
            var updatecat = ApplicationDbContext.SubCategorias.Find(model.sub_Id);
            if (ModelState.IsValid)
            {

                TempData["Info"] = "Subcategoria modificada";
                updatecat.SubC_Description = model.SubC_name;
                updatecat.Cate_Id = model.cate_Id;
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
            else
            {
                TempData["Info"] = "Los campos no pueden ser vacios ";
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
        }


        [Authorize]
        public ActionResult DeleteSubCategory(int id)
        {
            var catego = ApplicationDbContext.SubCategorias.Find(id);
            var sett = ApplicationDbContext.MG_SettingMps.FirstOrDefault(x => x.Company_Id == catego.Company_Id);
            var answer = catego.MG_MultipleChoice.Count;
            if (answer == 0)
            {
                TempData["Add"] = "SubCategoria Eliminada";
                ApplicationDbContext.SubCategorias.Remove(catego);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("MgQuestions", new { id = sett.Sett_Id });
            }
            else
            {
                TempData["Add"] = "No se puede eliminar la subcategoria por que tiene preguntas asociadas";
                return RedirectToAction("MgQuestions", new { id = sett.Sett_Id });
            }
        }
        //Multiple Choice
        [Authorize]
        public ActionResult AddMgMultipleChoice(AdminGameQuestionViewModel model, HttpPostedFileBase upload)
        {
            var Getsetting = ApplicationDbContext.MG_SettingMps.Find(model.Sett_Id);
            if (ModelState.IsValid)
            {
                if (model.cate_Id != 0)
                {
                    var cate = ApplicationDbContext.Categorias.Find(model.cate_Id);
                    if (model.sub_Id != 0)
                    {
                        var sub = ApplicationDbContext.SubCategorias.Find(model.sub_Id);
                        var niv = ApplicationDbContext.Nivels.Find(model.Nivel_Id);
                        if (upload != null && upload.ContentLength <= (2 * 1000000))
                        {
                            string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                            var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            foreach (var Ext in allowedExtensions)
                            {
                                if (Ext.Contains(file))
                                {
                                    file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                                    upload.SaveAs(Server.MapPath("~/Mg_Game_Image/" + file));
                                    TempData["add"] = "El curso se ha creado con éxito";
                                    string ruta = file;
                                    TempData["Add"] = "Pregunta Creada con éxito";
                                    MG_MultipleChoice multiplechouse = new MG_MultipleChoice
                                    {
                                        MuCh_Description = model.MuCh_Description,
                                        MG_SettingMp = Getsetting,
                                        Sett_Id = Getsetting.Sett_Id,
                                        MuCh_ImageQuestion = ruta,
                                        Categoria = cate,
                                        Cate_Id = cate.Cate_ID,
                                        SubCategoria = sub,
                                        SubC_Id = sub.SubC_ID
                                    };
                                    ApplicationDbContext.MG_MultipleChoices.Add(multiplechouse);
                                    ApplicationDbContext.SaveChanges();
                                    var MultiplechoiseId = ApplicationDbContext.MG_MultipleChoices.Find(multiplechouse.MuCh_ID);
                                    if (model.AnMul_Description != null)
                                    {
                                        MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                                        {
                                            AnMul_Description = model.AnMul_Description,
                                            AnMul_TrueAnswer = model.AnMul_TrueAnswer,
                                            MG_MultipleChoice = multiplechouse,
                                            MuCh_ID = multiplechouse.MuCh_ID
                                        };
                                        ApplicationDbContext.MG_AnswerMultipleChoices.Add(answeroptionmeultiple);
                                        ApplicationDbContext.SaveChanges();
                                    }
                                    if (model.AnMul_Description2 != null)
                                    {
                                        MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                                        {
                                            AnMul_Description = model.AnMul_Description2,
                                            AnMul_TrueAnswer = model.AnMul_TrueAnswer2,
                                            MG_MultipleChoice = multiplechouse,
                                            MuCh_ID = multiplechouse.MuCh_ID
                                        };
                                        ApplicationDbContext.MG_AnswerMultipleChoices.Add(answeroptionmeultiple);
                                        ApplicationDbContext.SaveChanges();
                                    }
                                    if (model.AnMul_Description3 != null)
                                    {
                                        MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                                        {
                                            AnMul_Description = model.AnMul_Description3,
                                            AnMul_TrueAnswer = model.AnMul_TrueAnswer3,
                                            MG_MultipleChoice = multiplechouse,
                                            MuCh_ID = multiplechouse.MuCh_ID
                                        };
                                        ApplicationDbContext.MG_AnswerMultipleChoices.Add(answeroptionmeultiple);
                                        ApplicationDbContext.SaveChanges();
                                    }
                                    if (model.AnMul_Description4 != null)
                                    {
                                        MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                                        {
                                            AnMul_Description = model.AnMul_Description4,
                                            AnMul_TrueAnswer = model.AnMul_TrueAnswer4,
                                            MG_MultipleChoice = multiplechouse,
                                            MuCh_ID = multiplechouse.MuCh_ID
                                        };
                                        ApplicationDbContext.MG_AnswerMultipleChoices.Add(answeroptionmeultiple);
                                        ApplicationDbContext.SaveChanges();
                                    }
                                    return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
                                }
                            }
                            TempData["add"] = "El formato del archivo no es valido";
                        }
                        else
                        {
                            TempData["Add"] = "Pregunta Creada con éxito";
                            MG_MultipleChoice multiplechouse = new MG_MultipleChoice
                            {
                                MuCh_Description = model.MuCh_Description,
                                MG_SettingMp = Getsetting,
                                Sett_Id = Getsetting.Sett_Id,
                                Categoria = cate,
                                Cate_Id = cate.Cate_ID,
                                SubCategoria = sub,
                                SubC_Id = sub.SubC_ID,
                                Nivel=niv,
                                Nivel_Id=niv.Nivel_Id,
                                state=model.state
                            };
                            ApplicationDbContext.MG_MultipleChoices.Add(multiplechouse);
                            ApplicationDbContext.SaveChanges();
                            var MultiplechoiseId = ApplicationDbContext.MG_MultipleChoices.Find(multiplechouse.MuCh_ID);
                            if (model.AnMul_Description != null)
                            {
                                MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                                {
                                    AnMul_Description = model.AnMul_Description,
                                    AnMul_TrueAnswer = model.AnMul_TrueAnswer,
                                    MG_MultipleChoice = multiplechouse,
                                    MuCh_ID = multiplechouse.MuCh_ID
                                };
                                ApplicationDbContext.MG_AnswerMultipleChoices.Add(answeroptionmeultiple);
                                ApplicationDbContext.SaveChanges();
                            }
                            if (model.AnMul_Description2 != null)
                            {
                                MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                                {
                                    AnMul_Description = model.AnMul_Description2,
                                    AnMul_TrueAnswer = model.AnMul_TrueAnswer2,
                                    MG_MultipleChoice = multiplechouse,
                                    MuCh_ID = multiplechouse.MuCh_ID
                                };
                                ApplicationDbContext.MG_AnswerMultipleChoices.Add(answeroptionmeultiple);
                                ApplicationDbContext.SaveChanges();
                            }
                            if (model.AnMul_Description3 != null)
                            {
                                MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                                {
                                    AnMul_Description = model.AnMul_Description3,
                                    AnMul_TrueAnswer = model.AnMul_TrueAnswer3,
                                    MG_MultipleChoice = multiplechouse,
                                    MuCh_ID = multiplechouse.MuCh_ID
                                };
                                ApplicationDbContext.MG_AnswerMultipleChoices.Add(answeroptionmeultiple);
                                ApplicationDbContext.SaveChanges();
                            }
                            if (model.AnMul_Description4 != null)
                            {
                                MG_AnswerMultipleChoice answeroptionmeultiple = new MG_AnswerMultipleChoice
                                {
                                    AnMul_Description = model.AnMul_Description4,
                                    AnMul_TrueAnswer = model.AnMul_TrueAnswer4,
                                    MG_MultipleChoice = multiplechouse,
                                    MuCh_ID = multiplechouse.MuCh_ID
                                };
                                ApplicationDbContext.MG_AnswerMultipleChoices.Add(answeroptionmeultiple);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
                    }
                }

                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
            else
            {
                TempData["Add"] = "Los campos no puedes ser vacios";
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
        }

        [Authorize]
        public ActionResult AddOtherMultipleChoice(AdminGameQuestionViewModel model, int id)
        {

            var Getsetting = ApplicationDbContext.MG_SettingMps.Find(id);
            TempData["Add"] = "Pregunta Creada";
            model.Setting = Getsetting;
            model.Sett_Id = Getsetting.Sett_Id;
            model.Sett_Category = GetCategory(Getsetting.Company_Id);
            model.Sett_SubCategory = GetSubcategory(Getsetting.Company_Id);
            model.Sett_Nivel = GetNivel();
            return PartialView("_AddMultipleChoice", model);
        }

        [Authorize]
        public ActionResult EditMultipleCh(int id)
        {
            var MultipleId = ApplicationDbContext.MG_MultipleChoices.Find(id);
            var setting = ApplicationDbContext.MG_SettingMps.Find(MultipleId.Sett_Id);
            AdminGameQuestionViewModel model = new AdminGameQuestionViewModel
            {
                Setting = setting,
                Sett_Id = setting.Sett_Id,
                MuCh_ID = MultipleId.MuCh_ID,
                MuCh_Description = MultipleId.MuCh_Description,
                Sett_Category = GetCategory(setting.Company_Id),
                Sett_SubCategory = GetSubcategory(setting.Company_Id)
            };
            TempData["Form"] = "Activar Formulario";
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditMultipleChoices(AdminGameQuestionViewModel model, HttpPostedFileBase upload)
        {
            var setting = ApplicationDbContext.MG_SettingMps.Find(model.Sett_Id);
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength <= (2 * 1000000))
                {
                    string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var file = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                    foreach (var Ext in allowedExtensions)
                    {
                        if (Ext.Contains(file))
                        {
                            file = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                            upload.SaveAs(Server.MapPath("~/Mg_Game_Image/" + file));
                            string ruta = file;
                            var UpdatedOptionMultiple = ApplicationDbContext.MG_MultipleChoices.Find(model.MuCh_ID);
                            TempData["Add"] = "Pregunta Modificada con éxito";
                            UpdatedOptionMultiple.MuCh_Description = model.MuCh_Description;
                            UpdatedOptionMultiple.MuCh_ImageQuestion = ruta;
                            UpdatedOptionMultiple.Cate_Id = model.cate_Id;
                            UpdatedOptionMultiple.SubC_Id = model.sub_Id;
                            ApplicationDbContext.SaveChanges();
                            return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
                        }
                    }
                    TempData["add"] = "El formato del archivo no es valido";
                }
                else
                {
                    var UpdatedOptionMultiple = ApplicationDbContext.MG_MultipleChoices.Find(model.MuCh_ID);
                    TempData["Add"] = "Pregunta Modificada con éxito";
                    UpdatedOptionMultiple.MuCh_Description = model.MuCh_Description;
                    UpdatedOptionMultiple.Cate_Id = model.cate_Id;
                    UpdatedOptionMultiple.SubC_Id = model.sub_Id;
                    ApplicationDbContext.SaveChanges();
                }
                return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
            }
            return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
        }

        [Authorize]
        public ActionResult EditAnswerOMultiple(int id)
        {
            var AnswerOptionMultiple = ApplicationDbContext.MG_AnswerMultipleChoices.Find(id);
            var setting = ApplicationDbContext.MG_SettingMps.Find(AnswerOptionMultiple.MG_MultipleChoice.Sett_Id);
            AdminGameQuestionViewModel model = new AdminGameQuestionViewModel
            {
                Setting = setting,
                Sett_Id = setting.Sett_Id,
                AnMu_Id = AnswerOptionMultiple.AnMul_ID,
                AnMul_Description = AnswerOptionMultiple.AnMul_Description,
                AnMul_TrueAnswer = AnswerOptionMultiple.AnMul_TrueAnswer,
            };
            TempData["FormAnswer"] = "Activar Formulario";
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditAnswerOptionMultiples(AdminGameQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var UpdatedAnswerOptionMultiple = ApplicationDbContext.MG_AnswerMultipleChoices.Find(model.AnMu_Id);
                TempData["Add"] = "Respuesta Modificada";
                UpdatedAnswerOptionMultiple.AnMul_Description = model.AnMul_Description;
                UpdatedAnswerOptionMultiple.AnMul_TrueAnswer = model.AnMul_TrueAnswer;
                ApplicationDbContext.SaveChanges();
            }
            return RedirectToAction("MgQuestions", new { id = model.Sett_Id });
        }

        [Authorize]
        public ActionResult DeleteMultipleChoice(int id)
        {
            var OptionMultiple = ApplicationDbContext.MG_MultipleChoices.Find(id);

            TempData["Add"] = "Pregunta Eliminada";
            ApplicationDbContext.MG_MultipleChoices.Remove(OptionMultiple);
            ApplicationDbContext.SaveChanges();
            return RedirectToAction("MgQuestions", new { id = OptionMultiple.Sett_Id });

        }

        [Authorize]
        public ActionResult DeleteAnswerMultipleChoice(int id)
        {
            var AnswerOptionMultipled = ApplicationDbContext.MG_AnswerMultipleChoices.Find(id);
            var settingId = AnswerOptionMultipled.MG_MultipleChoice.Sett_Id;
            TempData["Add"] = "Respuesta Eliminada";
            ApplicationDbContext.MG_AnswerMultipleChoices.Remove(AnswerOptionMultipled);
            ApplicationDbContext.SaveChanges();
            return RedirectToAction("MgQuestions", new { id = settingId });
        }

        [HttpGet]
        [AllowAnonymous]
        [Authorize]
        public ActionResult MassiveQuestions(int Id)
        {
            var setting = ApplicationDbContext.MG_SettingMps.Find(Id);
            AdmingameMassiveQuestion model = new AdmingameMassiveQuestion
            {
                Setting = setting
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult MassiveQuestions(AdmingameMassiveQuestion model, HttpPostedFileBase excelUpload)
        {
            var setting = ApplicationDbContext.MG_SettingMps.Find(model.Setting.Sett_Id);
            if (excelUpload != null && excelUpload.ContentLength > 0)
            {
                Stream stream = excelUpload.InputStream;
                IExcelDataReader reader = null;
                if (excelUpload.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (excelUpload.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    ModelState.AddModelError("File", "Este formato no es soportado");
                    model = new AdmingameMassiveQuestion
                    {
                        Setting = setting
                    };
                    return View(model);
                }
                reader.IsFirstRowAsColumnNames = true;
                DataSet result = reader.AsDataSet();
                string next = VerifyQuestionsFields(result);
                if (next == "success")
                {
                    foreach (DataTable table in result.Tables)
                    {
                        switch (table.TableName)
                        {
                            case "PreguntaOpcionMultiple":
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    string TitleOpenMultiple = table.Rows[i].ItemArray[0].ToString();
                                    string DescriptionOptionMultiple = table.Rows[i].ItemArray[1].ToString();
                                    string AnswerOM = table.Rows[i].ItemArray[2].ToString();
                                    string TrueAnswer = table.Rows[i].ItemArray[3].ToString();
                                    int CateId = Int32.Parse(table.Rows[i].ItemArray[4].ToString());
                                    int SubcateId = Int32.Parse(table.Rows[i].ItemArray[5].ToString());
                                    string[] a = AnswerOM.Split(';');
                                    string[] t = TrueAnswer.Split(';');
                                    MG_MultipleChoice optionmultiples = new MG_MultipleChoice
                                    {
                                        MuCh_NameQuestion = TitleOpenMultiple,
                                        MuCh_Description = DescriptionOptionMultiple,
                                        Sett_Id = setting.Sett_Id,
                                        Cate_Id = CateId,
                                        SubC_Id = SubcateId,
                                        MG_SettingMp = setting,
                                    };
                                    ApplicationDbContext.MG_MultipleChoices.Add(optionmultiples);
                                    ApplicationDbContext.SaveChanges();
                                    for (int c = 0; c < a.Length; c++)
                                    {
                                        var m = a[c];
                                        var n = t[c];
                                        int Z = Int32.Parse(n);
                                        MG_AnswerMultipleChoice AnswerOptionMultiple = new MG_AnswerMultipleChoice
                                        {
                                            AnMul_Description = m,
                                            AnMul_TrueAnswer = (OPTIONANSWER)Z,
                                            MuCh_ID = optionmultiples.MuCh_ID,
                                            MG_MultipleChoice = optionmultiples
                                        };
                                        ApplicationDbContext.MG_AnswerMultipleChoices.Add(AnswerOptionMultiple);
                                        ApplicationDbContext.SaveChanges();
                                    }
                                }
                                break;
                        }
                    }
                    reader.Close();
                    ModelState.AddModelError("File", "Preguntas cargadas con exito");
                    model = new AdmingameMassiveQuestion
                    {
                        Setting = setting,
                    };
                    return View(model);
                }
                else
                {
                    string[] a = next.Split(';');
                    string error = a[0];
                    TempData["Menssage"] = " Error en la carga: descripcion. " + error + " Por este motivo no se pueden cargar las preguntas,por favor verifique las respuestas y vuelvlo a intentar";
                    model.Setting = setting;
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("File", "No se a seleccionado un archivo");
                model = new AdmingameMassiveQuestion
                {
                    Setting = setting,
                };
                return View(model);
            }
        }

        private string VerifyQuestionsFields(DataSet result)
        {
            List<Categoria> Areas = ApplicationDbContext.Categorias.ToList();
            List<SubCategoria> Citys = ApplicationDbContext.SubCategorias.ToList();
            foreach (DataTable Table in result.Tables)
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    string AnswerOM = Table.Rows[i].ItemArray[2].ToString();
                    string[] a = AnswerOM.Split(';');
                    int cate = Int32.Parse(Table.Rows[i].ItemArray[4].ToString());
                    int subcate = Int32.Parse(Table.Rows[i].ItemArray[5].ToString());
                    for (int c = 0; c < a.Length; c++)
                    {
                        var m = a[c];
                        if (m == null)
                        {
                            return "Deben Haber respustas creadas y la respuesta no debe pasar de 107 caracteres";
                        }
                    }
                    if (Areas.Count(x => x.Cate_ID == cate) <= 0)
                    {
                        return "No hay categoria registrada;" + i;
                    }
                    if (Citys.Count(x => x.SubC_ID == subcate) <= 0)
                    {
                        return "No hay subcategoria registrada;" + i;
                    }
                }
            }
            return "success";
        }

        [HttpPost]
        public ActionResult TermsGame(AdminGameSetting model, HttpPostedFileBase upload)
        {
            var com = ApplicationDbContext.Companies.Find(model.companyId);
            if (upload != null && upload.ContentLength <= (1 * 1000000))
            {
                string[] allowedExtensions = new[] { ".pdf" };
                var imagen = Path.GetExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                foreach (var Ext in allowedExtensions)
                {
                    if (Ext.Contains(imagen))
                    {
                        imagen = (DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + upload.FileName).ToLower();
                        upload.SaveAs(Server.MapPath("~/TermsandConditions/" + imagen));
                        string ruta = imagen;
                        var terms = ApplicationDbContext.MG_SettingMps.Where(x => x.Company_Id == com.CompanyId).FirstOrDefault();
                        if (terms == null)
                        {
                            TempData["Info"] = "Por favor añade la configuración primero";
                            return RedirectToAction("Setting", new { id = com.CompanyId });
                        }
                        else
                        {
                            terms.Sett_Instruction = ruta;
                            ApplicationDbContext.SaveChanges();
                            return RedirectToAction("Setting", new { id = com.CompanyId });
                        }

                    }
                }
                TempData["Info"] = "El formato del archivo no es valido";
                return RedirectToAction("Setting", new { id = com.CompanyId });
            }
            else
            {
                TempData["Info"] = "No se ha seleccionado un archivo o el archivo es muy pesado";
                return RedirectToAction("Setting", new { id = com.CompanyId });
            }
        }

        [Authorize]
        public ActionResult Deletequest(int id)
        {
            var sett = ApplicationDbContext.MG_SettingMps.Find(id);

            var pre = ApplicationDbContext.MG_MultipleChoices.Where(x => x.Sett_Id == sett.Sett_Id).ToList();
            if (pre.Count != 0)
            {
                foreach (var item in pre)
                {
                    foreach (var item1 in item.MG_AnswerMultipleChoice.ToList())
                    {
                        var ans = ApplicationDbContext.MG_AnswerUsers.Where(x => x.AnMul_ID == item1.AnMul_ID).ToList();
                        if (ans.Count != 0)
                        {
                            foreach (var item2 in ans)
                            {
                                ApplicationDbContext.MG_AnswerUsers.Remove(item2);
                                ApplicationDbContext.SaveChanges();
                            }
                        }
                        ApplicationDbContext.MG_AnswerMultipleChoices.Remove(item1);
                        ApplicationDbContext.SaveChanges();
                    }
                    ApplicationDbContext.MG_MultipleChoices.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }
            TempData["Add"] = "Preguntas Eliminadas";
            return RedirectToAction("MgQuestions", new { id = sett.Sett_Id });

        }


    }
}