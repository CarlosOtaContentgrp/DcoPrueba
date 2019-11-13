using DemoScore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoScore.Controllers
{
    public class HomeController : Controller
    {
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private ApplicationSignInManager _signInManager;
        public HomeController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        public HomeController(ApplicationSignInManager signInManager)
        {
            SignInManager = signInManager;
            this.ApplicationDbContext = new ApplicationDbContext();
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
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var user = UserManager.FindById(userId);
                    indexMain model = new indexMain();
                model.Home = new Models.ViewModel.HomeViewModels { ActualRole = user.Role };
                model.Login = new LoginViewModel();
                model.TermsandConditions = new Models.ViewModel.HomeViewModels { terms = user.TermsandConditions };
                var company = user.CompanyId;
                var set = ApplicationDbContext.MG_SettingMps.FirstOrDefault(x => x.Company_Id == company);
                if (set != null)
                {
                    model.settin = set.Sett_Id;
                }
                return View(model);
          
            }
            else
            {
                indexMain model = new indexMain();
                model.Login = new LoginViewModel();
                return View(model);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}