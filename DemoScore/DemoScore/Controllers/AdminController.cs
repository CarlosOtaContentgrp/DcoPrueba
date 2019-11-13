using DemoScore.Models;
using DemoScore.Models.Game;
using DemoScore.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DemoScore.Controllers
{
    public class AdminController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        protected UserManager<ApplicationUser> UserManager { get; set; }

        public AdminController()
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
        // GET: Admin
        public ActionResult Perfil()
        {
            
            return PartialView("_Perfil");
        }
        public ActionResult Companies()
        {
            List<Company> Companies = ApplicationDbContext.Companies.ToList();
            List<infocompany> listcompany = new List<infocompany>();
            if (Companies.Count!=0)
            {

           
            foreach (var item in Companies)
            {
                    int user = ApplicationDbContext.Users.Where(X => X.CompanyId == item.CompanyId).ToList().Count();
                listcompany.Add(new infocompany
                {
                    CompanyId = item.CompanyId,
                    CompanyName = item.CompanyName,
                    CompanyUser=user
                });
            }
            }
            AdminGeneralViewModel model = new AdminGeneralViewModel
            {         
                ListCompanies = listcompany
            };
            return View(model);
        }

        public ActionResult CreateCompany(AdminGeneralViewModel model)
        {

            if (ModelState.IsValid)
            {
                Company NewComapany = new Company
                {
                    CompanyName = model.CompanyName
                };
                ApplicationDbContext.Companies.Add(NewComapany);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "Se ha creado la compañía con éxito";
                return RedirectToAction("Companies");
            }
            TempData["Menssage"] = "hubo problema al crear la compañia por favor intente de nuevo";
            return RedirectToAction("Companies");
        }

        public ActionResult Company()
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
        public ActionResult Report(int CompanyId)
        {
            AdminReports model = new AdminReports
            {
                company_Id=CompanyId
            };
            return View("Report", model);
        }

        public ActionResult ReporteCategoria(int id)
        {
          
            var c= (from k in ApplicationDbContext.Categorias.Where(x => x.Company_Id == id)
                    select new
                    {
                        Código = k.Cate_ID,
                        Categoria = k.Cate_Description,

                    }).OrderBy(x => x.Categoria).ToList();

            var AM = (from k in ApplicationDbContext.SubCategorias.Where(x => x.Company_Id == id)
                      select new
                      {
                          Código=k.SubC_ID,
                          Subcategoria = k.SubC_Description,
                        
                      }).OrderBy(x => x.Subcategoria).ToList();

            AdminReports model = new AdminReports
            {
            };

            if (AM.Count != 0)
            {
              
                var gv = new GridView();
                gv.DataSource = AM;
                gv.DataBind();
                var gv1 = new GridView();
                gv1.DataSource = c;
                gv1.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE CATEGORIAS Y SUBCATEGORIAS.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                StringWriter objStringWriter1 = new StringWriter();
                HtmlTextWriter objHtmlTextWriter1 = new HtmlTextWriter(objStringWriter1);
                gv1.RenderControl(objHtmlTextWriter1);
                Response.Output.Write("<H3>REPORTE CATEGORIAS Y SUBCATEGORIAS </H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write("<center><H4> <b>LISTADO CATEGORIAS </H4></center>");
                Response.Output.Write("<center>" + objStringWriter1.ToString() + "</center>");
                Response.Output.Write("<br>");
                Response.Output.Write("<center>" + objStringWriter.ToString() + "</center>");
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {
              
                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report",new { CompanyId= id });
        }

        public ActionResult ReporteGeneral(int id)
        {

            /*

            List<Resultadosusuario> listarnuevosdatos = new List<Resultadosusuario>();
            List<Resultadosusuario> listausuariofinal = new List<Resultadosusuario>();
            var area = ApplicationDbContext.Areas.Where(x => x.CompanyId == id).ToList();

            foreach (var recorrerarea in area)
            {

                var ubicacion = ApplicationDbContext.Ubicaciones.Where(x => x.CompanyId == id).ToList();
                foreach (var ubi in ubicacion)
                {

                    var datos = ApplicationDbContext.MG_AnswerUsers.Where(x => x.ApplicationUser.CompanyId == id).ToList();
               
                    foreach (var dat in datos)
                    {

                        var person = ApplicationDbContext.Users.Where(x => x.CompanyId == id).ToList();

                        foreach (var per in person) {

                            listarnuevosdatos.Add(new Resultadosusuario
                    {

                        areanam = recorrerarea.AreaName,
                        loca_descri = ubi.Loca_Description,
                        Resultado = dat.Respuesta,
                        Nivel = dat.Attemps,
                        Respuesta = per.Email,


                        });
                }
                }
                }

            }

            var itemlist = (from h in listarnuevosdatos
                            select new
                            {
                                Area_Nombre = h.areanam,
                                Descripcion_Ubicacion = h.loca_descri,
                                Correo=h.Respuesta,
                                pregunta=h.Pregunta,
                                categories=h.Categoria,
                                Resultado = h.Resultado,
                                Nivel = h.Nivel,
                            }).Distinct().OrderBy(x => x.Area_Nombre).ToList();

            AdminReports model = new AdminReports
            {
            };
            if (itemlist.Count != 0)
            {
                var gv1 = new GridView();
                gv1.DataSource = itemlist;
                gv1.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE PROGRESO GENERAL.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter1 = new StringWriter();
                HtmlTextWriter objHtmlTextWriter1 = new HtmlTextWriter(objStringWriter1);
                gv1.RenderControl(objHtmlTextWriter1);
                Response.Output.Write("<H3>REPORTE CATEGORIAS Y SUBCATEGORIAS </H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write("<center><H4> <b>LISTADO CATEGORIAS </H4></center>");
                Response.Output.Write("<center>" + objStringWriter1.ToString() + "</center>");
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {

                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report", new { CompanyId = id });
        }
        */

            //Realizamos la consulta Lambda y Linq c# para realizar el doble inner join Para sacar los datos de los usuarios para el reporte de excel, 
            //Creamos 3 variables (k,U,A) y cada una se le asigna una tabla de la base de datos, realizamos las relaciones de acuerdo al UserId 
            //Y lo asignamos a un Select y asigno los datos en el reporte del excel.
            var c = (from k in ApplicationDbContext.MG_AnswerUsers
                     join U in ApplicationDbContext.Ubicaciones on k.ApplicationUser.LocationId equals U.Loca_Id
                     join A in ApplicationDbContext.Areas on k.ApplicationUser.AreaId equals A.AreaId
                     where (k.ApplicationUser.CompanyId == id)
                     select new
                     {

                         Nombre = k.ApplicationUser.FirstName + k.ApplicationUser.LastName,
                         Correo = k.ApplicationUser.Email,
                         Pregunta = k.MG_AnswerMultipleChoice.MG_MultipleChoice.MuCh_Description,
                         Categoria = k.MG_AnswerMultipleChoice.MG_MultipleChoice.Categoria.Cate_Description,
                         Respuesta = k.MG_AnswerMultipleChoice.AnMul_Description,
                         Resultado = k.Respuesta,
                         Nivel = k.Attemps,

                         Locacion = U.Loca_Description,
                         Area = A.AreaName
                     }).OrderBy(k => k.Categoria).ToList();

            AdminReports model = new AdminReports
          {
          };
          if (c.Count != 0)
          {
              var gv1 = new GridView();
              gv1.DataSource = c;
              gv1.DataBind();
              Response.ClearContent();
              Response.Buffer = true;
              Response.AddHeader("content-disposition", "attachment; filename=REPORTE PROGRESO GENERAL.xls");
              Response.ContentType = "application/ms-excel";
              Response.Charset = "";
              StringWriter objStringWriter1 = new StringWriter();
              HtmlTextWriter objHtmlTextWriter1 = new HtmlTextWriter(objStringWriter1);
              gv1.RenderControl(objHtmlTextWriter1);
              Response.Output.Write("<H3>REPORTE CATEGORIAS Y SUBCATEGORIAS </H3>");
              Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
              Response.Output.Write("<center><H4> <b>LISTADO CATEGORIAS </H4></center>");
              Response.Output.Write("<center>" + objStringWriter1.ToString() + "</center>");
              Response.Flush();
              Response.End();
              return RedirectToAction("Report");
          }
          else
          {

              TempData["Menssages"] = "No hay datos para mostrar";
          }
          return RedirectToAction("Report",new { CompanyId = id});
      }
      
        public ActionResult ReporteResultadosusuario(int id)
        {
            
            var cate = ApplicationDbContext.Categorias.Where(x => x.Company_Id == id).ToList();
            var ubi = ApplicationDbContext.Ubicaciones.Where(x => x.CompanyId == id).ToList();
            var are = ApplicationDbContext.Areas.Where(x => x.CompanyId == id).ToList();


            List<Resultadosusuario> listausuario = new List<Resultadosusuario>();
            List<Resultadosusuario> listausuariofinal = new List<Resultadosusuario>();

            foreach (var item in cate)
            {
                foreach (var item3 in ubi)
                {
                    foreach (var item4 in are)
                    {

                        var ans = ApplicationDbContext.MG_AnswerUsers.Where(x => x.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id == item.Cate_ID).ToList();

                    var user = ApplicationDbContext.Users.Where(x => x.CompanyId == id).ToList();




                    foreach (var item1 in ans)
                    {

                        var co = ApplicationDbContext.MG_AnswerUsers.Where(x => x.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id == item.Cate_ID && x.Respuesta == RESPUESTA.Correcta && x.User_Id == item1.User_Id).ToList();
                        var anss = ApplicationDbContext.MG_AnswerUsers.Where(x => x.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id == item.Cate_ID && x.User_Id == item1.User_Id).ToList();
                        var ubbi = ApplicationDbContext.Ubicaciones.Where(x => x.Loca_Id == item1.ApplicationUser.LocationId && item3.Loca_Id == item1.ApplicationUser.LocationId && x.Loca_Id == item3.Loca_Id).ToList();
                        var area = ApplicationDbContext.Areas.Where(x => x.AreaId == item1.ApplicationUser.AreaId && item4.AreaId == item1.ApplicationUser.AreaId && x.AreaId == item4.AreaId ).ToList();


                            listausuario.Add(new Resultadosusuario
                        {
                            User_Id = item1.User_Id,
                            Nombre = item1.ApplicationUser.FirstName + item1.ApplicationUser.LastName,
                            Categoria = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Categoria.Cate_Description,
                            Correo = item1.ApplicationUser.Email,
                            totalpreguntas = anss.Count,
                            PreguntasCorrectas = co.Count,
                            cate_id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id,
                            ubicacion = item3.Loca_Description,
                            areanam=item4.AreaName,



                        });

                    }
                }
            }
        }

            var itemlist = (from c in listausuario
                            select new
                            {
                                User_Id = c.User_Id,
                                Nombre = c.Nombre,
                                Categoria =c.Categoria,
                                Correo = c.Correo,
                                totalpreguntas =c.totalpreguntas,
                                PreguntasCorrectas = c.PreguntasCorrectas,
                                Ubicacion=c.ubicacion,
                                Areas =c.areanam,
                            }).Distinct().OrderBy(x =>x.Nombre ).ToList();

            AdminReports model = new AdminReports
            {
            };
            if (itemlist.Count != 0)
            {
                var gv1 = new GridView();
                gv1.DataSource = itemlist;
                gv1.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE PROGRESO GENERAL.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter1 = new StringWriter();
                HtmlTextWriter objHtmlTextWriter1 = new HtmlTextWriter(objStringWriter1);
                gv1.RenderControl(objHtmlTextWriter1);
                Response.Output.Write("<H3>REPORTE PROGRESO GENERAL </H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write("<center><H4> <b>LISTADO PROGRESO GENERAL </H4></center>");
                Response.Output.Write("<center>" + objStringWriter1.ToString() + "</center>");
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {

                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report", new { CompanyId = id });
        }

        public ActionResult Reporteresultadoscategorias(int id)
        {

            var cate = ApplicationDbContext.Categorias.Where(x => x.Company_Id == id).ToList();
            List<Resultadoscategorias> listacategorias = new List<Resultadoscategorias>();
            List<Resultadoscategorias> listacategoriasfinal = new List<Resultadoscategorias>();
            foreach (var item in cate)
            {
                var ans = ApplicationDbContext.MG_AnswerUsers.Where(x => x.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id == item.Cate_ID).ToList();
                foreach (var item1 in ans)
                {
                    var co = ApplicationDbContext.MG_AnswerUsers.Where(x => x.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id == item.Cate_ID).ToList();
                    var anss = ApplicationDbContext.MG_AnswerUsers.Where(x => x.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id == item.Cate_ID && x.Respuesta == RESPUESTA.Correcta).ToList();

                    listacategorias.Add(new Resultadoscategorias
                    {
                        Categoria = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Categoria.Cate_Description,
                        totalpreguntas = co.Count,
                        PreguntasCorrectas = anss.Count,
                        cate_id = item1.MG_AnswerMultipleChoice.MG_MultipleChoice.Cate_Id
                    });
                }
            }

            var itemlist = (from c in listacategorias
                            select new
                            {
                             
                                Categoria = c.Categoria,
                                totalpreguntas = c.totalpreguntas,
                                PreguntasCorrectas = c.PreguntasCorrectas,
                            }).Distinct().OrderBy(x => x.Categoria).ToList();

            AdminReports model = new AdminReports
            {
            };
            if (itemlist.Count != 0)
            {
                var gv1 = new GridView();
                gv1.DataSource = itemlist;
                gv1.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=REPORTE PROGRESO CATEGORIAS.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter1 = new StringWriter();
                HtmlTextWriter objHtmlTextWriter1 = new HtmlTextWriter(objStringWriter1);
                gv1.RenderControl(objHtmlTextWriter1);
                Response.Output.Write("<H3>REPORTE PROGRESO GENERAL CATEGORIAS </H3>");
                Response.Output.Write("<H4> <b>FECHA DE REPORTE : " + DateTime.Now);
                Response.Output.Write("<center><H4> <b>LISTADO PROGRESO GENERAL CATEGORIAS </H4></center>");
                Response.Output.Write("<center>" + objStringWriter1.ToString() + "</center>");
                Response.Flush();
                Response.End();
                return RedirectToAction("Report");
            }
            else
            {

                TempData["Menssages"] = "No hay datos para mostrar";
            }
            return RedirectToAction("Report", new { CompanyId = id });
        }

    }
}