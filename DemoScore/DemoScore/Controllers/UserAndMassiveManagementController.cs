using DemoScore.Models;
using DemoScore.Models.Game;
using DemoScore.Models.ViewModel;
using Excel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace DemoScore.Controllers
{
    public class UserAndMassiveManagementController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private ApplicationSignInManager _signInManager;
        public UserAndMassiveManagementController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));

        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        // GET: UserAndMassiveManagement

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
        public ActionResult ManagementUser(int id,int? page)
        {
            var companyId = ApplicationDbContext.Companies.Find(id);
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId.CompanyId).ToList().ToPagedList(page ?? 1, 6);


            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(companyId.CompanyId),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(companyId.CompanyId),
                PositionTheCompany = GetPosition(companyId.CompanyId),
                CompanyId=id       
            };           
            return View(model);
        }
        

        public ActionResult SearchUserManager(UserAndMassiveManagementViewModel model, int? page)
        {
            var companyId = model.CompanyId;
            if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrWhiteSpace(model.UserId))
            {
              
                return RedirectToAction("ManagementUser", new {id=model.CompanyId });
            }
            else
            {
                IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId && (x.FirstName.Contains(model.UserId) || x.LastName.Contains(model.UserId))).ToList().ToPagedList(page ?? 1, 6);
                model = new UserAndMassiveManagementViewModel
                {
                    UserOfCompany = ListOfUser,
                    AreasOfTheCompany = GetArea(companyId),
                    CityOfTheCompany = GetCityOfTheCompany(),
                    LocationOfTheCompany = GetLocationOfTheCompany(companyId),
                    PositionTheCompany = GetPosition(companyId),
                    CompanyId=companyId
                };
                return View("ManagementUser", model);
            }
        }
        
        public IEnumerable<SelectListItem> GetArea(int CompanyId)
        {
            List<Area> AreaOfMyCompany = ApplicationDbContext.Areas.Where(x => x.Company.CompanyId == CompanyId).ToList();
            IEnumerable<SelectListItem> Areas = AreaOfMyCompany.Select(x =>
           new SelectListItem
           {
               Value = x.AreaId.ToString(),
               Text = x.AreaName
           });
            return new SelectList(Areas, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetPosition(int CompanyId)
        {
            List<Cargo> PositionTheCompany = ApplicationDbContext.Cargos.Where(x => x.Company.CompanyId == CompanyId).ToList();
            IEnumerable<SelectListItem> Positions = PositionTheCompany.Select(x =>
           new SelectListItem
           {
               Value = x.Posi_id.ToString(),
               Text = x.Posi_Description
           });
            return new SelectList(Positions, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetCityOfTheCompany()
        {
            List<Ciudad> CityOfTheCompany = ApplicationDbContext.Ciudades.ToList();
            IEnumerable<SelectListItem> Citys = CityOfTheCompany.Select(x =>
           new SelectListItem
           {
               Value = x.City_Id.ToString(),
               Text = x.City_Name
           });
            return new SelectList(Citys, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetLocationOfTheCompany(int CompanyId)
        {
            List<Ubicacion> LocationOfTheCompany = ApplicationDbContext.Ubicaciones.Where(x => x.Company.CompanyId == CompanyId).ToList();
            IEnumerable<SelectListItem> Locations = LocationOfTheCompany.Select(x =>
           new SelectListItem
           {
               Value = x.Loca_Id.ToString(),
               Text = x.Loca_Description
           });
            return new SelectList(Locations, "Value", "Text");
        }
        public ApplicationUser GetActualUserId()
        {
            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            return user;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(UserAndMassiveManagementViewModel UserObtainedOfTheView)
        {
            if (ModelState.IsValid)

            {
                var company = ApplicationDbContext.Companies.Find(UserObtainedOfTheView.CompanyId);
                var ubi = ApplicationDbContext.Ubicaciones.Find(UserObtainedOfTheView.LocationId);
                ApplicationUser UserToCreate = new ApplicationUser
                {
                    UserName = UserObtainedOfTheView.UserName,
                    FirstName = UserObtainedOfTheView.FirstName,
                    LastName = UserObtainedOfTheView.LastName,
                    Email = UserObtainedOfTheView.Email,
                    Document = UserObtainedOfTheView.Document,
                    Role = (ROLES)UserObtainedOfTheView.Role,
                    AreaId = UserObtainedOfTheView.AreaId,
                    Company = company,
                    CompanyId=company.CompanyId,
                    CityId = UserObtainedOfTheView.CityId,
                    PositionId = UserObtainedOfTheView.PositionId,
                    LocationId = UserObtainedOfTheView.LocationId,                 
                    TermsandConditions = Terms_and_Conditions.No,
                    //Metodo creado para la asignación del dato tipo string al mensaje
                    Location_Desc = ubi.Loca_Description,
                };
                string next = VerifyUserFields(UserToCreate);

                if (next == "success")
                {
                    IdentityResult result = await UserManager.CreateAsync(UserToCreate, UserObtainedOfTheView.UserName);
                    if (result.Succeeded)
                    {
                        SendEmail(UserToCreate.FirstName + " " + UserToCreate.LastName, UserToCreate.Email, UserToCreate.UserName, UserToCreate.Company.CompanyName, UserToCreate.Location_Desc);
                        TempData["Menssage"] = "Usuario creado con éxito";
                       
                        return RedirectToAction("ManagementUser", new { id=UserToCreate.CompanyId});
                    }
                    TempData["Warning"] = result.Errors.First();
                }
                else
                {
                    string[] a = next.Split(';');
                    string error = a[0];
                    TempData["Menssage"] = " Error en la carga: descripcion. " + error + " favor verifica";
                    return RedirectToAction("ManagementUser", new { id = UserToCreate.CompanyId });
                }

            }
            return RedirectToAction("ManagementUser", new { id = UserObtainedOfTheView.CompanyId });
        }
        private string VerifyUserFields(ApplicationUser result)
        {
            

            List<Area> Areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == result.CompanyId).ToList();
            List<Ciudad> Citys = ApplicationDbContext.Ciudades.ToList();
            List<Ubicacion> locations = ApplicationDbContext.Ubicaciones.Where(x => x.CompanyId == result.CompanyId).ToList();
            List<Cargo> positions = ApplicationDbContext.Cargos.Where(x => x.CompanyId == result.CompanyId).ToList();

            if (result.Email == null || result.Email.Length > 60)
            {
                return "no hay Email o excede el tamaño;";
            }
            if (result.FirstName == null || result.FirstName.Length > 15)
            {
                return "no hay nombres o excede el tamaño;";
            }
            if (result.LastName == null || result.LastName.Length > 15)
            {
                return "no hay apellidos o excede el tamaño;";
            }
            if (Areas.Count(x => x.AreaId == result.AreaId) <= 0)
            {
                return "no hay area registrada;";
            }
            if (Citys.Count(x => x.City_Id == result.CityId) <= 0)
            {
                return "no hay ciudad registrada;";
            }
            if (locations.Count(x => x.Loca_Id == result.LocationId) <= 0)
            {
                return "no hay ubicacion registrada;";
            }
            if (positions.Count(x => x.Posi_id == result.PositionId) <= 0)
            {
                return "no hay cargo registrado;";
            }
            return "success";
        }
        private void SendEmail(string NameUser, string Email, string Usuario, string Company, string Location_Desc)
        {
            MailMessage solicitud = new MailMessage();
            solicitud.Subject = "Bienvenido a la plataforma de Diagnóstico de Conocimiento Organizacional para "+ Location_Desc;
            solicitud.Body =  "<br/>" +
                "Cordial saludo," + "<br/>" +
                "Sr(a). " + NameUser + "<br/>" +
                "<br/>" + Location_Desc +
                ",  Velasco & Calle D'Aleman te ha registrado en la plataforma de conocimiento  de nuestro aliado en gestión de conocimiento Gfactory." +
               "<br/>" +
                "<br/>" + "A través de esta plataforma podremos diagnosticar el grado de conocimiento que en materia de protección de datos personales tiene el personal registrado, esto con el fin de continuar avanzando en el cumplimiento del principio de responsabilidad demostrada adoptado por el Régimen de Protección de Datos Personales consagrado en la Ley Estatutaria 1581 de 2012. " +
               "<br/>" +
                "<br/>" + "Cualquier duda puedes consultarla con el Oficial de Privacidad de tu organización/institución " + Location_Desc + "."+
   
            "<br/>" +
            "<br/>" + "Gracias por tu valiosa participación en esta iniciativa que nos permitirá conocer las brechas de conocimiento en donde debemos hacer mayor énfasis." +
        
           "<br/>" +
               "<br/>" + "Tus datos de accesos son los siguientes" +
               "<br/>" +
               "<br/>" + "Usuario: " + Usuario +
               "<br/>" + "Contraseña: " + Usuario +
            "<br/>" + "link:" + "https://www.aprendeyavanza2.com.co/DCOSCORE/"+
                "<br/>" + "Para una correcta visualización de los contenidos use el navegador Chrome"+"<br/>"+
            "<br/>" + "Velasco & Calle D´Aleman  - Gfactory" +
            "<br/>" + "<img src='https://ci5.googleusercontent.com/proxy/5wZ2THypj0ySs_63zJcJaDI6hEia6xluKGD3iGsLN-pcd0AxJ3G_JjZ7evJdfcmhsK8iTkf7J-e_OytAW5HNiTIEKA1JpjB8b7x54pR3sKQMpOuCtaHD_rFdUmYL0cvlVxGZ4uI0cYkxgt57jS_97_eQ452t6LPw21yXlZ2OVTWmVev-UFArLgQ4OI3jRqfVWetZh3iJARgsiT7YDuKnsw=s0-d-e1-ft#https://docs.google.com/uc?export=download&id=11Q-QCcMRI229d8Tlbq380oJhx22M8TsP&revid=0B1zktT-wNG2udnhleGl6ZTIycGptRyt0STJwVW5MbmMvNWE4PQ' width='45%' />"
            +"<br/>"+ "Para una correcta visualización de los contenidos use el navegador Chrome"
     ; 


            solicitud.To.Add(Email);
            solicitud.IsBodyHtml = true;
            var smtp2 = new SmtpClient();
            smtp2.Send(solicitud);
        }

        [HttpGet]
        public ActionResult UpdateUserCurrent(string IdUserToModified, int? page)
        {
            var user = ApplicationDbContext.Users.Find(IdUserToModified);
            int companyId = user.Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == user.Company.CompanyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel UserToModified = new UserAndMassiveManagementViewModel
            {
                UserId = IdUserToModified,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Document = user.Document,              
                Role = (ROLES)user.Role,
                AreaId = user.AreaId,
                CityId = user.CityId,
                PositionId = user.PositionId,
                LocationId = user.LocationId,
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(companyId),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(companyId),
                PositionTheCompany = GetPosition(companyId)
            };
            TempData["UpdateUserCurrent"] = "Actualizar";
            return View("ManagementUser", UserToModified);
        }

        [HttpPost]
        public ActionResult UpdateUserCurrent(UserAndMassiveManagementViewModel UserToModified)
        {
            ApplicationUser user = UserManager.FindById(UserToModified.UserId);

            user.UserName = UserToModified.UserName;
            user.FirstName = UserToModified.FirstName;
            user.LastName = UserToModified.LastName;
            user.Email = UserToModified.Email;
            user.Document = UserToModified.Document;
            user.Role = (ROLES)UserToModified.Role;
            user.AreaId = UserToModified.AreaId;
            user.CityId = UserToModified.CityId;
            user.PositionId = UserToModified.PositionId;
            user.LocationId = UserToModified.LocationId;
            UserManager.Update(user);
            TempData["Menssage"] = "Usuario modificado con éxito";
            return RedirectToAction("ManagementUser",new {id=user.Company.CompanyId});
        }

        public ActionResult DeleteUser(string IdUserToDelete)
        {
            ApplicationUser user = UserManager.FindById(IdUserToDelete);
            int com = user.Company.CompanyId;
            if (IdUserToDelete != GetActualUserId().Id)
            {                          
                Boolean veruser = VeriUserDelete(user.Id);              
                if (veruser == true)
                {
                    UserManager.Delete(user);
                    TempData["Menssage"] = "Usuario elimado con éxito";
                    return RedirectToAction("ManagementUser", new {id= com});
                }
                else
                {
                    TempData["Menssage"] = "Problemas al elimar el usuario";
                    return RedirectToAction("ManagementUser", new { id = com});
                }
            }
            else
            {
                TempData["Menssage"] = "No puedes eliminar el usuario con el que esta logeado";
                return RedirectToAction("ManagementUser", new { id = com});
            }

        }

        public Boolean VeriUserDelete(string User_id)
        {
            ApplicationUser user = UserManager.FindById(User_id);
            var ans = ApplicationDbContext.MG_AnswerUsers.Where(x => x.User_Id == user.Id).ToList();
            if (ans.Count != 0)
            {
                foreach (var item in ans)
                {
                    ApplicationDbContext.MG_AnswerUsers.Remove(item);
                    ApplicationDbContext.SaveChanges();
                }
            }      
            return true;
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddArea(UserAndMassiveManagementViewModel model)
        {
            if (model.AreaName != null)
            {
                var company = ApplicationDbContext.Companies.Find(model.CompanyId);
                Area areaToCreate = new Area { AreaName = model.AreaName, CompanyId = model.CompanyId,Company=company };
                ApplicationDbContext.Areas.Add(areaToCreate);
                ApplicationDbContext.SaveChanges();

                return RedirectToAction("ManagementUser", new { id = company.CompanyId });
            }
            else
            {
                TempData["Menssage"] = "el area no puede estar vacia";
                return RedirectToAction("ManagementUser", new { id = model.CompanyId });
            }
        }

        [Authorize]
        public ActionResult DeleteArea(int id)
        {
            int listUserWithThisArea = ApplicationDbContext.Users.Where(x => x.AreaId == id).ToList().Count();
            Area areaTodelete = ApplicationDbContext.Areas.Find(id);
            int com = areaTodelete.Company.CompanyId;
            if (listUserWithThisArea <= 0)
            {                          
                ApplicationDbContext.Areas.Remove(areaTodelete);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "El area ha sido eliminada";
                return RedirectToAction("ManagementUser", new { id = com });
            }
            else
            {
                TempData["Menssage"] = "el área no puede ser elimanada porque tiene usuarios asignados";
                return RedirectToAction("ManagementUser", new { id = com });
            }
        }

        [HttpGet]
        public ActionResult UpdateArea(int id, int? page)
        {            
            Area area = ApplicationDbContext.Areas.Find(id);
            int companyId = area.Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                AreaId = area.AreaId,
                AreaName = area.AreaName,
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(companyId),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(companyId),
                PositionTheCompany = GetPosition(companyId),
            };
            TempData["Updatearea"] = "Actualizar";
            return View("ManagementUser", model);
        }

        [HttpPost]
        public ActionResult UpdateArea(UserAndMassiveManagementViewModel model)
        {
            Area area = ApplicationDbContext.Areas.Find(model.AreaId);
            area.AreaName = model.AreaName;
            ApplicationDbContext.SaveChanges();
            TempData["Menssage"] = "Área modificada con éxito";
            return RedirectToAction("ManagementUser",new {id=area.Company.CompanyId});
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddLocation(UserAndMassiveManagementViewModel model)
        {
            if (model.LocationName != null)
            {
                var company = ApplicationDbContext.Companies.Find(model.CompanyId);
                var LocationToCreate = new Ubicacion { Loca_Description = model.LocationName, CompanyId = company.CompanyId,Company=company };
                ApplicationDbContext.Ubicaciones.Add(LocationToCreate);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("ManagementUser", new { id = company.CompanyId });
            }
            else
            {
                TempData["Menssage"] = "el Locacion no puede estar vacia";
                return RedirectToAction("ManagementUser", new { id = model.CompanyId });
            }
        }

        [HttpGet]
        public ActionResult UpdateLocation(int id, int? page)
        {       
            Ubicacion location = ApplicationDbContext.Ubicaciones.Find(id);
            int companyId = location.Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                LocationId = location.Loca_Id,
                LocationName = location.Loca_Description,
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(companyId),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(companyId),
                PositionTheCompany = GetPosition(companyId),
            };
            TempData["UpdateLocations"] = "Actualizar";
            return View("ManagementUser", model);
        }

        [HttpPost]
        public ActionResult Updatelocation(UserAndMassiveManagementViewModel model)
        {
            Ubicacion location = ApplicationDbContext.Ubicaciones.Find(model.LocationId);
            location.Loca_Description = model.LocationName;
            ApplicationDbContext.SaveChanges();
            TempData["Menssage"] = "Ubicación modificada con éxito";
            return RedirectToAction("ManagementUser",new {id=location.Company.CompanyId});
        }

        [Authorize]
        public ActionResult DeleteLocation(int id)
        {
            Ubicacion locationTodelete = ApplicationDbContext.Ubicaciones.Find(id);
            int com = locationTodelete.Company.CompanyId;
            int listUserWithThisLocation = ApplicationDbContext.Users.Where(x => x.LocationId == id).ToList().Count();
            if (listUserWithThisLocation <= 0)
            {
                ApplicationDbContext.Ubicaciones.Remove(locationTodelete);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "La ubicación ha sido eliminada";
                return RedirectToAction("ManagementUser",new {id=com});
            }
            else
            {
                TempData["Menssage"] = "La ubicación no puede ser elimanada porque hay usuarios asignados";
                return RedirectToAction("ManagementUser", new { id = com});
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddCitys(UserAndMassiveManagementViewModel model)
        {
            if (model.CityName != null)
            {
                var cityToCreate = new Ciudad { City_Name = model.CityName };
                ApplicationDbContext.Ciudades.Add(cityToCreate);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("ManagementUser", new { id = model.CompanyId });
            }
            else
            {
                TempData["Menssage"] = "el campo ciudad puede estar vacia";
                return RedirectToAction("ManagementUser", new { id = model.CompanyId });
            }
        }

        [HttpGet]
        public ActionResult UpdateCity(int id, int com, int? page)
        {
        
            Ciudad city = ApplicationDbContext.Ciudades.Find(id);
            int companyId = com;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                CityId = city.City_Id,
                CityName = city.City_Name,
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(companyId),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(companyId),
                PositionTheCompany = GetPosition(companyId),
                CompanyId=companyId
            };
            TempData["Updatecity"] = "Actualizar";
            return View("ManagementUser", model);
        }
        [HttpPost]
        public ActionResult UpdateCity(UserAndMassiveManagementViewModel model)
        {
            Ciudad city = ApplicationDbContext.Ciudades.Find(model.CityId);
            city.City_Name = model.CityName;
            ApplicationDbContext.SaveChanges();
            TempData["Menssage"] = "Ciudad modificada con éxito";
            return RedirectToAction("ManagementUser",new {id=model.CompanyId });
        }
        [Authorize]
        public ActionResult DeleteCitys(int id,int com)
        {
            int listUserWithThisCity = ApplicationDbContext.Users.Where(x => x.CityId == id).ToList().Count();
            if (listUserWithThisCity <= 0)
            {
                Ciudad cityTodelete = ApplicationDbContext.Ciudades.Find(id);
                ApplicationDbContext.Ciudades.Remove(cityTodelete);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "Ciudad elimanada con éxito";
                return RedirectToAction("ManagementUser",new {id=com});
            }
            else
            {
                TempData["Menssage"] = "La ciudad no puede ser elimanada porque hay usuarios asignados";
                return RedirectToAction("ManagementUser",new {id =com});
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddPosition(UserAndMassiveManagementViewModel model)
        {
            if (model.PositionName != null)
            {
                var company = ApplicationDbContext.Companies.Find(model.CompanyId);
                var positionToCreate = new Cargo { Posi_Description = model.PositionName, CompanyId =company.CompanyId, Company=company };
                ApplicationDbContext.Cargos.Add(positionToCreate);
                ApplicationDbContext.SaveChanges();
                return RedirectToAction("ManagementUser", new { id = company.CompanyId });
            }
            else
            {
                TempData["Menssage"] = "el campo Cargo no puede estar vacia";
                return RedirectToAction("ManagementUser", new { id = model.CompanyId });
            }
        }

        [HttpGet]
        public ActionResult UpdatePosition(int idPosition, int? page)
        {           
            Cargo position = ApplicationDbContext.Cargos.Find(idPosition);
            int companyId = position.Company.CompanyId;
            IPagedList<ApplicationUser> ListOfUser = ApplicationDbContext.Users.OrderBy(x => x.UserName).Where(x => x.CompanyId == companyId).ToList().ToPagedList(page ?? 1, 6);
            UserAndMassiveManagementViewModel model = new UserAndMassiveManagementViewModel
            {
                PositionId = position.Posi_id,
                PositionName = position.Posi_Description,
                UserOfCompany = ListOfUser,
                AreasOfTheCompany = GetArea(companyId),
                CityOfTheCompany = GetCityOfTheCompany(),
                LocationOfTheCompany = GetLocationOfTheCompany(companyId),
                PositionTheCompany = GetPosition(companyId),
            };
            TempData["Updateposition"] = "Actualizar";
            return View("ManagementUser", model);
        }
        [HttpPost]
        public ActionResult UpdatePosition(UserAndMassiveManagementViewModel model)
        {
            Cargo position = ApplicationDbContext.Cargos.Find(model.PositionId);
            position.Posi_Description = model.PositionName;
            ApplicationDbContext.SaveChanges();
            TempData["Menssage"] = "Cargo modificado con éxito";
            return RedirectToAction("ManagementUser",new {id=position.Company.CompanyId});
        }

        [Authorize]
        public ActionResult DeletePosition(int idPosition)
        {
            Cargo positionTodelete = ApplicationDbContext.Cargos.Find(idPosition);
            int com = positionTodelete.Company.CompanyId;
            int listUserWithThisCity = ApplicationDbContext.Users.Where(x => x.PositionId == idPosition).ToList().Count();
            if (listUserWithThisCity <= 0)
            {                     
                ApplicationDbContext.Cargos.Remove(positionTodelete);
                ApplicationDbContext.SaveChanges();
                TempData["Menssage"] = "Cargo elimanado con éxito";
                return RedirectToAction("ManagementUser",new {id=com});
            }
            else
            {
                TempData["Menssage"] = "El cargo no puede ser elimanado porque hay usuarios asignados";
                return RedirectToAction("ManagementUser",new {id=com});
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult MassiveRegister(int id)
        {
            LogoUserMasive model = new LogoUserMasive
            {
             companyId=id
            };
     
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> MassiveRegister(HttpPostedFileBase excelUpload, LogoUserMasive model)
        {
          
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
                    return View(model);
                }

                reader.IsFirstRowAsColumnNames = true;

                DataSet result = reader.AsDataSet();
                string next = VerifyUserFields(result,model.companyId);
                if (next == "success")
                {
                    var CompanyId = ApplicationDbContext.Companies.Find(model.companyId);
                    foreach (DataTable table in result.Tables)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            ApplicationUser user = new ApplicationUser
                            {
                                UserName = table.Rows[i].ItemArray[0].ToString(),
                                Email = table.Rows[i].ItemArray[1].ToString(),
                                FirstName = table.Rows[i].ItemArray[2].ToString(),
                                LastName = table.Rows[i].ItemArray[3].ToString(),
                                Document = table.Rows[i].ItemArray[4].ToString(),
                                AreaId = Int32.Parse(table.Rows[i].ItemArray[5].ToString()),
                                CityId = Int32.Parse(table.Rows[i].ItemArray[6].ToString()),
                                PositionId = Int32.Parse(table.Rows[i].ItemArray[7].ToString()),
                                LocationId = Int32.Parse(table.Rows[i].ItemArray[8].ToString()),
                                Location_Desc = table.Rows[i].ItemArray[9].ToString(),
                                CompanyId = CompanyId.CompanyId,
                                Company = CompanyId,
                                TermsandConditions = Terms_and_Conditions.No
                            };
                            IdentityResult results = await UserManager.CreateAsync(user, user.UserName);
                            AddErrors(results);
                            if (results.Succeeded == true)
                            {
                                SendEmail(user.FirstName + " " + user.LastName, user.Email, user.UserName, user.Company.CompanyName, user.Location_Desc);                               
                            }
                        }
                        reader.Close();
                        LogoUserMasive n = new LogoUserMasive
                        {
                            data = result.Tables[0]
                        };
                        n.companyId=CompanyId.CompanyId;
                        return View(n);
                    }
                }
                else
                {
                    string[] a = next.Split(';');
                    string error = a[0];
                    string line = Convert.ToString(Int32.Parse(a[1]) + 2);
                    TempData["Menssage"] = " Error en la carga: descripcion. " + error + " favor verifica la linea:" + line;               
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("File", "Por favor selecciona un formato valido");             
            }
            return View(model);
        }

        private string VerifyUserFields(DataSet result, int id)
        {
           

            List<Area> Areas = ApplicationDbContext.Areas.Where(x => x.CompanyId == id).ToList();
            List<Ciudad> Citys = ApplicationDbContext.Ciudades.ToList();
            List<Ubicacion> locations = ApplicationDbContext.Ubicaciones.Where(x => x.CompanyId == id).ToList();
            List<Cargo> positions = ApplicationDbContext.Cargos.Where(x => x.CompanyId == id).ToList();

            foreach (DataTable Table in result.Tables)
            {
                for (int i = 0; i < Table.Rows.Count; i++)
                {
                    string username = Table.Rows[i].ItemArray[0].ToString();
                    string email = Table.Rows[i].ItemArray[1].ToString();
                    string firstname = Table.Rows[i].ItemArray[2].ToString();
                    string lastname = Table.Rows[i].ItemArray[3].ToString();
                    string document = Table.Rows[i].ItemArray[4].ToString();
                    int Area = Int32.Parse(Table.Rows[i].ItemArray[5].ToString());
                    int city = Int32.Parse(Table.Rows[i].ItemArray[6].ToString());
                    int position = Int32.Parse(Table.Rows[i].ItemArray[7].ToString());
                    int location = Int32.Parse(Table.Rows[i].ItemArray[8].ToString());
                    string Location_Desc = Table.Rows[i].ItemArray[9].ToString();
                    if (username == null || username.Length > 60)
                    {
                        return "No hay username o excede los 60 caracteres;" + i;
                    }
                    if (email == null || email.Length > 60)
                    {
                        return "nNo hay Email o excede los 60 caracteres;" + i;
                    }
                    if (firstname == null || firstname.Length > 60)
                    {
                        return "No hay nombre o excede los 60 caracteres;" + i;
                    }
                    if (lastname == null || lastname.Length > 60)
                    {
                        return "No hay apellidos o excede los 60 caracteres;" + i;
                    }
                    if (Areas.Count(x => x.AreaId == Area) <= 0)
                    {
                        return "No hay área registrada;" + i;
                    }
                    if (Citys.Count(x => x.City_Id == city) <= 0)
                    {
                        return "No hay ciudad registrada;" + i;
                    }
                    if (locations.Count(x => x.Loca_Id == location) <= 0)
                    {
                        return "No hay ubicación registrada;" + i;
                    }
                    if (positions.Count(x => x.Posi_id == position) <= 0)
                    {
                        return "No hay cargo registrado;" + i;
                    }
                    if (Location_Desc == null || Location_Desc.Length > 60)
                    {
                        return "No hay Ciudades o excede los 60 caracteres;" + i;
                    }
                }
            }
            return "success";
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }



    }
}