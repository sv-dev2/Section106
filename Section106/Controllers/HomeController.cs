using Section106.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Section106.Controllers
{
    public class HomeController : Controller
    {
        private ICommonService _commonService;

        public HomeController(ICommonService CommonService)
        {
            _commonService = CommonService;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (User.IsInRole("Submitter"))
                {
                    return RedirectToAction("Index", "Submitter");
                }
                else if (User.IsInRole("Architectural"))
                {
                    return RedirectToAction("ReviewersDashboard", "Reviewer");
                }
                else if (User.IsInRole("Archaeology"))
                {
                    return RedirectToAction("ReviewersDashboard", "Reviewer");
                }
                else if (User.IsInRole("Technical Assistance"))
                {
                    return RedirectToAction("ReviewersDashboard", "Reviewer");
                }
                else if (User.IsInRole("Landmarks"))
                {
                    return RedirectToAction("ReviewersDashboard", "Reviewer");
                }
                return View();
            }
            return RedirectToAction("Login", "Account");
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