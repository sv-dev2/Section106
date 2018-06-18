using iTextSharp.text;
using Section106.Models.Enums;
using Section106.Models.Models;
using Section106.Service.IService;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Section106.Controllers
{
    public class SubmitterController : Controller
    {
        private ICommonService _commonService;
        private IRequestService _requestService;
        private IUserService _userService;
        private IAdminService _adminService;

        public SubmitterController(ICommonService CommonService, IRequestService RequestService, IUserService UserService,IAdminService AdminService)
        {
            _commonService = CommonService;
            _requestService = RequestService;
            _userService = UserService;
            _adminService = AdminService;
        }

        public ActionResult Index()
        {
            //var requestListVM = new List<RequestVM>();
            //var loggedInUserId = _commonService.GetUserIdByUserName(User.Identity.Name);
            //var statusList = new List<int>() { Convert.ToInt16(RequestStatus.Saved), Convert.ToInt16(RequestStatus.Submitted) };
            //requestListVM = _requestService.GetUserRequestsByStatus(loggedInUserId, statusList);
            return View();
        }

        [HttpGet]
        public ActionResult GetUserRequests()
        {
            var draw = Request.QueryString["draw"];
            var start = Request.QueryString["start"];
            var length = Request.QueryString["length"];

            ////Find Order Column
            //var sortColumn = Request.QueryString["columns[" + Request.QueryString["order[0][column]"] + "][name]"];
            //var sortColumnDir = Request.QueryString["order[0][dir]"];
            //var searchString = Request.QueryString["search[value]"];

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var requestListVM = new List<RequestVM>();
            var loggedInUserId = _commonService.GetUserIdByUserName(User.Identity.Name);
            var statusList = new List<int>() { Convert.ToInt16(RequestStatus.Saved), Convert.ToInt16(RequestStatus.Submitted), Convert.ToInt16(RequestStatus.Returned), Convert.ToInt16(RequestStatus.Assigned), Convert.ToInt16(RequestStatus.Rejected), Convert.ToInt16(RequestStatus.Completed) };
            requestListVM = _requestService.GetUserRequestsByStatus(loggedInUserId, statusList);

            recordsTotal = requestListVM.Count();
            var data = requestListVM.Skip(skip).Take(pageSize).ToList();
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Create()
        //{
        //    var requestVM = new RequestVM();

        //    requestVM.States = _commonService.GetStates();
        //    requestVM.Cities = _commonService.GetCities();
        //    requestVM.Counties = _commonService.GetCounties();
        //    requestVM.Agencies = _commonService.GetAgencies();
        //    var loggedInUserId = _commonService.GetUserIdByUserName(User.Identity.Name);
        //    requestVM.SubmittingContact = _userService.GetSubmittingContactByUserId(loggedInUserId);
        //    return View(requestVM);
        //}

        public ActionResult Edit(long RequestId)
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
        //public ActionResult SaveCorrespondence(CorrespondenceVM model, long RequestId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        long correspondenceId = _commonService.SaveCorrespondence(model, RequestId);
        //        var correspondenceTypeName = _commonService.GetCorrespondenceTypeNameById(model.CorrespondenceTypeId);
        //        try
        //        {
        //            CreatePdf(correspondenceId, correspondenceTypeName, model.Body);
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        return Json(correspondenceId, JsonRequestBehavior.AllowGet);
        //    }
        //    return RedirectToAction("Edit", RequestId);
        //}
        //public ActionResult UpdateCorrespondence(CorrespondenceVM model, long RequestId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        bool result = _commonService.UpdateCorrespondence(model, RequestId);
        //        var correspondenceTypeName = _commonService.GetCorrespondenceTypeNameById(model.CorrespondenceTypeId);
        //        try
        //        {
        //            CreatePdf(model.CorrespondenceId ?? 0, correspondenceTypeName, model.Body);
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    return RedirectToAction("Edit", RequestId);
        //}

        //public ActionResult DeleteCorrespondence(long correspondenceId, string correspondenceType, long RequestId)
        //{
        //    if (correspondenceId > 0)
        //    {
        //        bool result = _commonService.DeleteCorrespondence(correspondenceId);
        //        string FilePathPdf = Server.MapPath("/Correspondence/" + correspondenceId + "_" + correspondenceType.Trim() + ".pdf");
        //        if ((System.IO.File.Exists(FilePathPdf)))
        //        {
        //            System.IO.File.Delete(FilePathPdf);
        //        }
        //        string FilePathDocx = Server.MapPath("/Correspondence/" + correspondenceId + "_" + correspondenceType.Trim() + ".docx");
        //        if ((System.IO.File.Exists(FilePathDocx)))
        //        {
        //            System.IO.File.Delete(FilePathDocx);
        //        }
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    return RedirectToAction("Edit", RequestId);
        //}
        //public bool CreatePdf(long correspondenceId, string correspondenceTypeName, string body)
        //{
        //    iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 88f, 88f, 10f, 10f);
        //    iTextSharp.text.Font NormalFont = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.DARK_GRAY);
        //    try
        //    {
        //        string FilePath = Server.MapPath("/Correspondence/" + correspondenceId + "_" + correspondenceTypeName + ".pdf");
        //        document.Open();

        //        HtmlToPdf converter = new HtmlToPdf();

        //        // set converter options

        //        converter.Options.PdfPageSize = PdfPageSize.A4;
        //        converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
        //        //converter.Options.WebPageWidth = webPageWidth;
        //        //converter.Options.WebPageHeight = webPageHeight;

        //        // create a new pdf document converting an url
        //        SelectPdf.PdfDocument doc = converter.ConvertHtmlString(body, null);

        //        byte[] pdf = doc.Save();
        //        FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
        //        fs.Write(pdf, 0, pdf.Length);
        //        fs.Close();
        //        //doc.Save(FilePath);

        //        document.Close();
        //        return true;
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return false;
        //}

     



        //public ActionResult View(long RequestId)
        //{
        //    var requestVM = new RequestVM();
        //    requestVM = _requestService.GetRequestByRequestId(RequestId);            
        //    requestVM.States = _commonService.GetStates();
        //    requestVM.Cities = _commonService.GetCities();
        //    requestVM.Counties = _commonService.GetCounties();
        //    requestVM.Agencies = _commonService.GetAgencies();
        //    return View(requestVM);
        //}

        //public ActionResult SaveRequest(RequestVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (model.ApplicantContact != null)
        //        {
        //            if (model.ApplicantContact.MobileNumber != null)
        //                model.ApplicantContact.MobileNumber = model.ApplicantContact.MobileNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //            if (model.ApplicantContact.OfficeNumber != null)
        //                model.ApplicantContact.OfficeNumber = model.ApplicantContact.OfficeNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //            if (model.ApplicantContact.Fax != null)
        //                model.ApplicantContact.Fax = model.ApplicantContact.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //        }
        //        if (model.SubmittingContact != null)
        //        {
        //            if (model.SubmittingContact.MobileNumber != null)
        //                model.SubmittingContact.MobileNumber = model.SubmittingContact.MobileNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //            if (model.SubmittingContact.OfficeNumber != null)
        //                model.SubmittingContact.OfficeNumber = model.SubmittingContact.OfficeNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //            if (model.SubmittingContact.Fax != null)
        //                model.SubmittingContact.Fax = model.SubmittingContact.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //        }
        //        bool result = _requestService.SaveRequest(model);
        //        if (result)
        //        {
        //            TempData["SuccessMessage"] = "Request saved successfully.";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    TempData["ErrorMessage"] = "Request not saved.";
        //    return RedirectToAction("Create");
        //}

        //public ActionResult UpdateRequest(RequestVM model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (model.ApplicantContact.MobileNumber != null)
        //            model.ApplicantContact.MobileNumber = model.ApplicantContact.MobileNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //        if (model.ApplicantContact.OfficeNumber != null)
        //            model.ApplicantContact.OfficeNumber = model.ApplicantContact.OfficeNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //        if (model.ApplicantContact.Fax != null)
        //            model.ApplicantContact.Fax = model.ApplicantContact.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //        _requestService.UpdateRequest(model);

        //        TempData["SuccessMessage"] = "Request updated successfully.";
        //        return RedirectToAction("Index");
        //    }
        //    TempData["ErrorMessage"] = "Request not updated.";
        //    return RedirectToAction("Create");
        //}

        //public ActionResult DeleteAttachment(long Id)
        //{
        //    bool result = _requestService.DeleteAttachment(Id);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Delete(Int64 RequestId)
        //{
        //    var result = _requestService.DeleteUserRequestByRequestId(RequestId);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //    //if (result)
        //    //{
        //    //    //TempData["SuccessMessage"] = "Request deleted successfully.";
        //    //    //return RedirectToAction("Index");

        //    //}
        //    //TempData["ErrorMessage"] = "Request not deleted.";
        //    //return RedirectToAction("Index");
        //}
    }
}