using Section106.Models.Enums;
using Section106.Models.Models;
using Section106.Service.IService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Section106.Controllers
{
    public class ReviewerController : Controller
    {
        private ICommonService _commonService;
        private IRequestReviewService _requestReviewService;
        private IRequestService _requestService;

        public ReviewerController(ICommonService CommonService, IRequestReviewService RequestReviewService, IRequestService RequestService)
        {
            _commonService = CommonService;
            _requestReviewService = RequestReviewService;
            _requestService = RequestService;
        }

        public ActionResult ReviewersDashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetReviewerRequestsByRole()
        {
            var draw = Request.QueryString["draw"];
            var start = Request.QueryString["start"];
            var length = Request.QueryString["length"];

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var userRole = _commonService.GetUserRoleByUserName(User.Identity.Name);

            var requestListVM = new List<RequestVM>();
            var statusList = new List<int>() { Convert.ToInt16(RequestStatus.Assigned) };
            requestListVM = _requestReviewService.GetReviewerRequestsByRole(statusList, userRole);

            recordsTotal = requestListVM.Count();
            var data = requestListVM.Skip(skip).Take(pageSize).ToList();
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Review(long RequestId)
        {

            var requestVM = new RequestVM();
            requestVM = _requestService.GetRequestByRequestId(RequestId);

            requestVM.States = _commonService.GetStates();
            requestVM.Cities = _commonService.GetCities();
            requestVM.Counties = _commonService.GetCounties();
            requestVM.Agencies = _commonService.GetAgencies();

            requestVM.CorrespondenceTypes = _commonService.GetCorrespondenceTypes();
    
            return View(requestVM);
        }

        private void LogError(long RequestId = 0,string text = "")
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += RequestId;
            message += Environment.NewLine;
            message += text;

            string path = Server.MapPath("~/ErrorLog/ErrorLog.txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }

        public ActionResult SaveReviewerResponse(RequestVM model,bool IsReqAddInfo)
        {
            if (ModelState.IsValid)
            {
                var loggedInUserId = _commonService.GetUserIdByUserName(User.Identity.Name);
                var userRole = _commonService.GetUserRoleByUserName(User.Identity.Name);
          
                bool result = _requestReviewService.SaveReviewerResponse(model, IsReqAddInfo, loggedInUserId, userRole);
                if (result)
                {
                    TempData["SuccessMessage"] = "Response saved successfully.";
                    return RedirectToAction("ReviewersDashboard");
                }
            }
            TempData["ErrorMessage"] = "Response not saved.";
            return RedirectToAction("Review",new { RequestId= model.RequestId } );
        }
    }
}