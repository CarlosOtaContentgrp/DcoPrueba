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
using DemoScore.App_Tool;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using DemoScore.dcodemoscoreDataSetBDTableAdapters;
using System.Data;


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

        public ActionResult CreateCompany(AdminGeneralViewModel model)
        {

            if (ModelState.IsValid)
            {
                Company NewComapany = new Company
                {
                    CompanyName = model.CompanyName,
                    qnivel1 = model.qnivel1,
                    qnivel2 = model.qnivel2,
                    qnivel3 = model.qnivel3
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
                company_Id = CompanyId
            };
            return View("Report", model);
        }

        public ActionResult ReporteCategoria(int id)
        {

            ReportViewer reportViewer =
                  new ReportViewer()
                  {
                      ProcessingMode = ProcessingMode.Local,
                      SizeToReportContent = true,
                      Width = Unit.Percentage(100),
                      Height = Unit.Percentage(100),
                  };
            dcodemoscoreDataSetBD.PROC_REP_CATEGORYDataTable data = new dcodemoscoreDataSetBD.PROC_REP_CATEGORYDataTable();
            PROC_REP_CATEGORYTableAdapter adapter = new PROC_REP_CATEGORYTableAdapter();

            adapter.Fill(data, id);
            if (data != null && data.Rows.Count > 0)
            {
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data.CopyToDataTable()));
                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\Rep_Category.rdlc";
                ViewBag.ReportViewer = reportViewer;
            }
            else
            {
                //Message("No se encontraron datos para el informe con los filtros utilizados, por favor utilice otros filtros", MessageType.Info);
            }
            ViewBag.reports = true;
            AdminReports model = new AdminReports
            {
                company_Id = id
            };

            return View("Report", model);


        }

        public ActionResult ReporteGeneral(int id)
        {

            ReportViewer reportViewer =
                  new ReportViewer()
                  {
                      ProcessingMode = ProcessingMode.Local,
                      SizeToReportContent = true,
                      Width = Unit.Percentage(100),
                      Height = Unit.Percentage(100),
                  };
            dcodemoscoreDataSetBD.DataByCompanyDataTable data = new dcodemoscoreDataSetBD.DataByCompanyDataTable();
            DataByCompanyTableAdapter adapter = new DataByCompanyTableAdapter();
            //ColpensionesDataSet.PROC_REP_POSTSUMMARIESDataTable data = new ColpensionesDataSet.PROC_REP_POSTSUMMARIESDataTable();
            //PROC_REP_POSTSUMMARIESTableAdapter adapter = new PROC_REP_POSTSUMMARIESTableAdapter();


            adapter.Fill(data, id);
            if (data != null && data.Rows.Count > 0)
            {
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data.CopyToDataTable()));
                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\RptDataByCompany.rdlc";
                ViewBag.ReportViewer = reportViewer;
            }
            else
            {

                //Message("No se encontraron datos para el informe con los filtros utilizados, por favor utilice otros filtros", MessageType.Info);
            }
            ViewBag.reports = true;
            AdminReports model = new AdminReports
            {
                company_Id = id
            };


            return View("Report", model);
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
                            var area = ApplicationDbContext.Areas.Where(x => x.AreaId == item1.ApplicationUser.AreaId && item4.AreaId == item1.ApplicationUser.AreaId && x.AreaId == item4.AreaId).ToList();


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
                                areanam = item4.AreaName,



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
                                Categoria = c.Categoria,
                                Correo = c.Correo,
                                totalpreguntas = c.totalpreguntas,
                                PreguntasCorrectas = c.PreguntasCorrectas,
                                Ubicacion = c.ubicacion,
                                Areas = c.areanam,
                            }).Distinct().OrderBy(x => x.Nombre).ToList();

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

        public ActionResult ReportByCompany(int id)
        {

            ReportViewer reportViewer =
                  new ReportViewer()
                  {
                      ProcessingMode = ProcessingMode.Local,
                      SizeToReportContent = true,
                      Width = Unit.Percentage(100),
                      Height = Unit.Percentage(100),
                  };
            dcodemoscoreDataSetBD.DataByCompanyDataTable data = new dcodemoscoreDataSetBD.DataByCompanyDataTable();
            DataByCompanyTableAdapter adapter = new DataByCompanyTableAdapter();
            //ColpensionesDataSet.PROC_REP_POSTSUMMARIESDataTable data = new ColpensionesDataSet.PROC_REP_POSTSUMMARIESDataTable();
            //PROC_REP_POSTSUMMARIESTableAdapter adapter = new PROC_REP_POSTSUMMARIESTableAdapter();


            adapter.Fill(data, id);
            if (data != null && data.Rows.Count > 0)
            {
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data.CopyToDataTable()));
                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\RptDataByCompany.rdlc";
                ViewBag.ReportViewer = reportViewer;
            }
            else
            {

                //Message("No se encontraron datos para el informe con los filtros utilizados, por favor utilice otros filtros", MessageType.Info);
            }
            ViewBag.reports = true;
            AdminReports model = new AdminReports
            {
                company_Id = id
            };
            return View("Report", model);

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