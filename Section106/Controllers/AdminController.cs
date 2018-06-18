using iTextSharp.text;
using iTextSharp.text.pdf;
using Section106.Models.Enums;
using Section106.Models.Models;
using Section106.Service.IService;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Xceed.Words.NET;
using Microsoft.Office.Interop.Word;
using static System.Net.Mime.MediaTypeNames;

namespace Section106.Controllers
{
    public class AdminController : Controller
    {
        private IAdminService _adminService;
        private ICommonService _commonService;
        private IRequestService _requestService;
        private IUserService _userService;

        public AdminController(IAdminService AdminService, ICommonService CommonService, IRequestService RequestService, IUserService UserService)
        {
            _adminService = AdminService;
            _commonService = CommonService;
            _requestService = RequestService;
            _userService = UserService;
        }

        #region "Request"

        public ActionResult Index()
        {
            ViewBag.StatusList = _commonService.GetAdminRequestStatus();
            if (Session["SelectedStatus"] != null)
            {
                ViewBag.SelectedStatus = Convert.ToInt64(Session["SelectedStatus"].ToString());
            }
            else
            {
                ViewBag.SelectedStatus = (long)RequestStatus.Submitted;
            }
            return View();
        }

        public ActionResult View(long RequestId)
        {
            var requestVM = new RequestVM();
            requestVM = _requestService.GetRequestByRequestId(RequestId);
            requestVM.States = _commonService.GetStates();
            requestVM.Cities = _commonService.GetCities();
            requestVM.Counties = _commonService.GetCounties();
            requestVM.Agencies = _commonService.GetAgencies();
            return View(requestVM);
        }
        public ActionResult Create()
        {
            var requestVM = new RequestVM();
            requestVM.States = _commonService.GetStates();
            requestVM.Cities = _commonService.GetCities();
            requestVM.Counties = _commonService.GetCounties();
            requestVM.Agencies = _commonService.GetAgencies();
            requestVM.SubmittingContact = new ContactVM();
            // var loggedInUserId = _commonService.GetUserIdByUserName(User.Identity.Name);
            //requestVM.SubmittingContact = _userService.GetSubmittingContactByUserId(loggedInUserId);
            return View(requestVM);
        }
        public ActionResult Edit(long RequestId)
        {
            var requestVM = new RequestVM();
            requestVM = _requestService.GetRequestByRequestId(RequestId);
            requestVM.States = _commonService.GetStates();
            requestVM.Cities = _commonService.GetCities();
            requestVM.Counties = _commonService.GetCounties();
            requestVM.Agencies = _commonService.GetAgencies();
            return View(requestVM);
        }
        public ActionResult UpdateRequest(RequestVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.ApplicantContact.MobileNumber != null)
                    model.ApplicantContact.MobileNumber = model.ApplicantContact.MobileNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                if (model.ApplicantContact.OfficeNumber != null)
                    model.ApplicantContact.OfficeNumber = model.ApplicantContact.OfficeNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                if (model.ApplicantContact.Fax != null)
                    model.ApplicantContact.Fax = model.ApplicantContact.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                _requestService.UpdateRequest(model);

                TempData["SuccessMessage"] = "Request updated successfully.";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Request not updated.";
            return RedirectToAction("Create");
        }
        public ActionResult Delete(Int64 RequestId)
        {
            var result = _requestService.DeleteUserRequestByRequestId(RequestId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetUserRequestsByStatus(string status)
        {
            var draw = Request.QueryString["draw"];
            var start = Request.QueryString["start"];
            var length = Request.QueryString["length"];

            var search = Request.QueryString["search[value]"];
            if (!string.IsNullOrEmpty(search))
            {
                Session["SelectedStatus"] = search;
            }

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var requestListVM = new List<RequestVM>();
            var statusList = new List<int>();
            if (string.IsNullOrEmpty(search) && (Session["SelectedStatus"] == null))
            {
                statusList = new List<int>() { Convert.ToInt16(RequestStatus.Saved), Convert.ToInt16(RequestStatus.Submitted) };
                requestListVM = _adminService.GetUserRequestsByStatus(statusList);
                //requestListVM = _adminService.GetUserRequestsByStatus(Convert.ToInt32(RequestStatus.Saved));
            }
            else
            {
                if (Convert.ToInt32(Session["SelectedStatus"]) == 2)
                {
                    statusList = new List<int>() { Convert.ToInt16(RequestStatus.Saved), Convert.ToInt16(RequestStatus.Submitted) };
                    requestListVM = _adminService.GetUserRequestsByStatus(statusList);
                }
                else
                {
                    statusList = new List<int>() { Convert.ToInt32(Session["SelectedStatus"]) };
                    requestListVM = _adminService.GetUserRequestsByStatus(statusList);
                }
                //requestListVM = _adminService.GetUserRequestsByStatus(Convert.ToInt32(Session["SelectedStatus"]));
            }

            recordsTotal = requestListVM.Count();
            var data = requestListVM.Skip(skip).Take(pageSize).ToList();
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Assign(long RequestId)
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
        public ActionResult Reports()
        {
            return View();
        }
        public ActionResult SaveRequestAssignment(RequestVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.ApplicantContact != null)
                {
                    if (model.ApplicantContact.MobileNumber != null)
                        model.ApplicantContact.MobileNumber = model.ApplicantContact.MobileNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    if (model.ApplicantContact.OfficeNumber != null)
                        model.ApplicantContact.OfficeNumber = model.ApplicantContact.OfficeNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    if (model.ApplicantContact.Fax != null)
                        model.ApplicantContact.Fax = model.ApplicantContact.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                }
                if (model.SubmittingContact != null)
                {
                    if (model.SubmittingContact.MobileNumber != null)
                        model.SubmittingContact.MobileNumber = model.SubmittingContact.MobileNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    if (model.SubmittingContact.OfficeNumber != null)
                        model.SubmittingContact.OfficeNumber = model.SubmittingContact.OfficeNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    if (model.SubmittingContact.Fax != null)
                        model.SubmittingContact.Fax = model.SubmittingContact.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                }
                bool result = _adminService.SaveRequestAssignment(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Request assigned successfully.";
                    return RedirectToAction("Index");
                }
            }
            TempData["ErrorMessage"] = "Request not assigned.";
            return RedirectToAction("Assign", new { RequestId = model.RequestId });
        }

        [ValidateInput(false)]
        public ActionResult SaveCorrespondence(CorrespondenceVM model, long RequestId)
        {
            if (ModelState.IsValid)
            {
                long correspondenceId = _commonService.SaveCorrespondence(model, RequestId);
                var correspondenceTypeName = _commonService.GetCorrespondenceTypeNameById(model.CorrespondenceTypeId);
                try
                {
                    CreatePdf(correspondenceId, correspondenceTypeName, model.Body);
                    CreateDoc(correspondenceId, correspondenceTypeName, model.PlainBody);
                }
                catch (Exception ex)
                {
                }
                return Json(correspondenceId, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Assign", new { RequestId = RequestId });
        }

        public ActionResult UpdateCorrespondence(CorrespondenceVM model, long RequestId)
        {
            if (ModelState.IsValid)
            {
                bool result = _commonService.UpdateCorrespondence(model, RequestId);
                var correspondenceTypeName = _commonService.GetCorrespondenceTypeNameById(model.CorrespondenceTypeId);
                try
                {
                    CreatePdf(model.CorrespondenceId ?? 0, correspondenceTypeName, model.Body);
                    CreateDoc(model.CorrespondenceId ?? 0, correspondenceTypeName, model.PlainBody);
                }
                catch (Exception ex)
                {
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Assign", new { RequestId = RequestId });
        }

        public ActionResult DeleteCorrespondence(long correspondenceId, string correspondenceType, long RequestId)
        {
            if (correspondenceId > 0)
            {
                bool result = _commonService.DeleteCorrespondence(correspondenceId);
                string FilePathPdf = Server.MapPath("/Correspondence/" + correspondenceId + "_" + correspondenceType.Trim() + ".pdf");
                if ((System.IO.File.Exists(FilePathPdf)))
                {
                    System.IO.File.Delete(FilePathPdf);
                }
                string FilePathDocx = Server.MapPath("/Correspondence/" + correspondenceId + "_" + correspondenceType.Trim() + ".docx");
                if ((System.IO.File.Exists(FilePathDocx)))
                {
                    System.IO.File.Delete(FilePathDocx);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Assign", new { RequestId = RequestId });
        }

        public ActionResult Response(long RequestId)
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

        public ActionResult SaveAdminResponse(RequestVM model)
        {
            if (ModelState.IsValid)
            {
                bool result = _adminService.SaveAdminResponse(model);

                bool aa = _adminService.UpdateRequestAssignment(model);

                if (result)
                {
                    if (model.Status == Convert.ToInt32(RequestStatus.Rejected))
                    {
                        TempData["SuccessMessage"] = "Request rejected successfully.";
                    }
                    if (model.Status == Convert.ToInt32(RequestStatus.Completed))
                    {
                        TempData["SuccessMessage"] = "Request completed successfully.";
                    }
                    return RedirectToAction("Index");
                }
            }
            TempData["ErrorMessage"] = "Request response not saved.";
            return RedirectToAction("Response", new { RequestId = model.RequestId });
        }

        public ActionResult ResetClock(long RequestId, string Reason)
        {
            if (RequestId != 0 && !string.IsNullOrEmpty(Reason))
            {
                bool result = _adminService.ResetClock(RequestId, Reason);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        #endregion "Request"

        #region "Agency Type"

        public ActionResult AgencyType()
        {
            return View();
        }

        public ActionResult GetAgencyTypes()
        {
            var draw = Request.QueryString["draw"];
            var start = Request.QueryString["start"];
            var length = Request.QueryString["length"];

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var agencyTypeVM = new List<AgencyTypeVM>();
            agencyTypeVM = _adminService.GetAgencyTypes();

            recordsTotal = agencyTypeVM.Count();
            var data = agencyTypeVM.Skip(skip).Take(pageSize).ToList();
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateAgencyType()
        {
            return View();
        }

        public ActionResult SaveAgencyType(AgencyTypeVM model)
        {
            if (ModelState.IsValid)
            {
                bool result = _adminService.SaveAgencyType(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Agency Type saved successfully.";
                    return RedirectToAction("AgencyType");
                }
            }
            TempData["ErrorMessage"] = "Agency Type not saved.";
            return RedirectToAction("CreateAgencyType");
        }

        public ActionResult EditAgencyType(long agencyTypeId)
        {
            var agencyTypeVM = _adminService.GetAgencyTypeById(agencyTypeId);
            return View(agencyTypeVM);
        }

        public ActionResult UpdateAgencyType(AgencyTypeVM model)
        {
            if (ModelState.IsValid)
            {
                bool result = _adminService.UpdateAgencyType(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Agency Type updated successfully.";
                    return RedirectToAction("AgencyType");
                }
            }
            TempData["ErrorMessage"] = "Agency Type not updated.";
            return RedirectToAction("EditAgencyType");
        }

        public ActionResult DeleteAgencyType(long agencyTypeId)
        {
            if (agencyTypeId > 0)
            {
                var useCount = _adminService.DeleteAgencyType(agencyTypeId);
                return Json(useCount, JsonRequestBehavior.AllowGet);
                //if (useCount == 0)
                //{
                //    TempData["SuccessMessage"] = "Agency Type deleted successfully.";
                //    return Json(useCount, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    TempData["SuccessMessage"] = "You can not delete this agency type because this is being used in " + useCount + " agencies.";
                //    return Json(useCount, JsonRequestBehavior.AllowGet);
                //}
            }
            else
            {
                TempData["ErrorMessage"] = "Agency Type not deleted.";
                return RedirectToAction("AgencyType");
            }
        }

        #endregion "Agency Type"

        #region "Correspondence Type"

        public ActionResult CorrespondenceType()
        {
            return View();
        }

        public ActionResult GetCorrespondenceTypes()
        {
            var draw = Request.QueryString["draw"];
            var start = Request.QueryString["start"];
            var length = Request.QueryString["length"];

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var correspondenceTypeVM = new List<CorrespondenceTypeVM>();
            correspondenceTypeVM = _adminService.GetCorrespondenceTypes();

            recordsTotal = correspondenceTypeVM.Count();
            var data = correspondenceTypeVM.Skip(skip).Take(pageSize).ToList();
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCorrespondenceTypeById(string CorrespondenceTypeId)
        {
            var correspondenceTypeVM = new List<CorrespondenceTypeVM>();
            var data = _adminService.GetCorrespondenceTypeById(Convert.ToInt32(CorrespondenceTypeId));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateCorrespondenceType()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult SaveCorrespondenceType(CorrespondenceTypeVM model)
        {
            if (ModelState.IsValid)
            {
                bool result = _adminService.SaveCorrespondenceType(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Correspondence Type saved successfully.";
                    return RedirectToAction("CorrespondenceType");
                }
            }
            TempData["ErrorMessage"] = "Correspondence Type not saved.";
            return RedirectToAction("CreateCorrespondenceType");
        }

        public ActionResult EditCorrespondenceType(long correspondenceTypeId)
        {
            var correspondenceTypeVM = _adminService.GetCorrespondenceTypeById(correspondenceTypeId);
            return View(correspondenceTypeVM);
        }
        [ValidateInput(false)]
        public ActionResult UpdateCorrespondenceType(CorrespondenceTypeVM model)
        {
            if (ModelState.IsValid)
            {
                bool result = _adminService.UpdateCorrespondenceType(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Correspondence Type updated successfully.";
                    return RedirectToAction("CorrespondenceType");
                }
            }
            TempData["ErrorMessage"] = "Correspondence Type not updated.";
            return RedirectToAction("EditCorrespondenceType");
        }

        public ActionResult DeleteCorrespondenceType(long correspondenceTypeId)
        {
            if (correspondenceTypeId > 0)
            {
                var useCount = _adminService.DeleteCorrespondenceType(correspondenceTypeId);
                return Json(useCount, JsonRequestBehavior.AllowGet);
                //if (useCount == 0)
                //{
                //    TempData["SuccessMessage"] = "Agency Type deleted successfully.";
                //    return Json(useCount, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    TempData["SuccessMessage"] = "You can not delete this agency type because this is being used in " + useCount + " agencies.";
                //    return Json(useCount, JsonRequestBehavior.AllowGet);
                //}
            }
            else
            {
                TempData["ErrorMessage"] = "Correspondence Type not deleted.";
                return RedirectToAction("CorrespondenceType");
            }

            //var result = _adminService.DeleteCorrespondenceType(correspondenceTypeId);
            //if (result)
            //{
            //    TempData["SuccessMessage"] = "Correspondence Type deleted successfully.";
            //    return RedirectToAction("CorrespondenceType");
            //}
            //TempData["ErrorMessage"] = "Correspondence Type not deleted.";
            //return RedirectToAction("CorrespondenceType");
        }

        #endregion "Correspondence Type"

        #region "Agency"

        public ActionResult Agency()
        {
            return View();
        }

        public ActionResult GetAgencies()
        {
            var draw = Request.QueryString["draw"];
            var start = Request.QueryString["start"];
            var length = Request.QueryString["length"];

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var agencyVM = new List<AgencyVM>();
            agencyVM = _adminService.GetAgencies();

            recordsTotal = agencyVM.Count();
            var data = agencyVM.Skip(skip).Take(pageSize).ToList();
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateAgency()
        {
            var agencyVM = new AgencyVM();
            agencyVM.AgencyTypeList = _commonService.GetAgencyTypes();
            return View(agencyVM);
        }

        public ActionResult SaveAgency(AgencyVM model)
        {
            if (ModelState.IsValid)
            {
                bool result = _adminService.SaveAgency(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Agency saved successfully.";
                    return RedirectToAction("Agency");
                }
            }
            TempData["ErrorMessage"] = "Agency not saved.";
            return RedirectToAction("CreateAgency");
        }

        public ActionResult EditAgency(long agencyId)
        {
            var agencyVM = _adminService.GetAgencyById(agencyId);
            agencyVM.AgencyTypeList = _commonService.GetAgencyTypes();
            return View(agencyVM);
        }

        public ActionResult UpdateAgency(AgencyVM model)
        {
            if (ModelState.IsValid)

            {
                bool result = _adminService.UpdateAgency(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Agency updated successfully.";
                    return RedirectToAction("Agency");
                }
            }
            TempData["ErrorMessage"] = "Agency not updated.";
            return RedirectToAction("EditAgency");
        }

        public ActionResult DeleteAgency(long agencyId)
        {
            if (agencyId > 0)
            {
                var useCount = _adminService.DeleteAgency(agencyId);
                return Json(useCount, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["ErrorMessage"] = "Agency not deleted.";
                return RedirectToAction("Agency");
            }

            //var result = _adminService.DeleteAgency(agencyId);
            //if (result)
            //{
            //    TempData["SuccessMessage"] = "Agency deleted successfully.";
            //    return RedirectToAction("Agency");
            //}
            //TempData["ErrorMessage"] = "Agency not deleted.";
            //return RedirectToAction("Agency");
        }

        #endregion "Agency"

        #region "Reviewers Account"

        public ActionResult ReviewerAccounts()
        {
            Session["SelectedType"] = 1;
            return View();
        }

        public ActionResult GetReviewers()
        {
            var draw = Request.QueryString["draw"];
            var start = Request.QueryString["start"];
            var length = Request.QueryString["length"];

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var reviewers = new List<ContactVM>();
            reviewers = _adminService.GetReviewers();

            recordsTotal = reviewers.Count();
            var data = reviewers.Skip(skip).Take(pageSize).ToList();
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

        #endregion "Reviewers Account"

        //public bool CreateDoc(long correspondenceId, string correspondenceTypeName, string body)
        //{
        //    Document document = new Document();
        //    return true;
        //}

        public bool CreatePdf(long correspondenceId, string correspondenceTypeName, string body)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 88f, 88f, 10f, 10f);
            iTextSharp.text.Font NormalFont = FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.DARK_GRAY);
            try
            {
                string FilePath = Server.MapPath("/Correspondence/" + correspondenceId + "_" + correspondenceTypeName + ".pdf");
                document.Open();

                HtmlToPdf converter = new HtmlToPdf();

                // set converter options

                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                //converter.Options.WebPageWidth = webPageWidth;
                //converter.Options.WebPageHeight = webPageHeight;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(body, null);

                byte[] pdf = doc.Save();
                FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate);
                fs.Write(pdf, 0, pdf.Length);
                fs.Close();
                //doc.Save(FilePath);

                document.Close();
                return true;
                //}

            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public bool CreateDoc(long correspondenceId, string correspondenceTypeName, string body)
        {
            //try
            //{
            //    Microsoft.Office.Interop.Word.Document document = new Microsoft.Office.Interop.Word.Document();

            //    document.LoadFromFile(@"D:\test.html", FileFormat.Html, XHTMLValidationType.None);
            //    document.SaveToFile("test.doc", FileFormat.Doc);

            //    Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            //    Microsoft.Office.Interop.Word.Document wordDoc = new Microsoft.Office.Interop.Word.Document();
            //    Object oMissing = System.Reflection.Missing.Value;

            //    wordDoc = word.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            //    word.Visible = false;

            //    Object filepath = "E:\\HtmlPage1.html";
            //    //Object filepath = body;
            //    Object confirmconversion = System.Reflection.Missing.Value;
            //    Object readOnly = false;
            //    //Object saveto = "c:\\doc.pdf";
            //    Object saveto = Server.MapPath("~/Correspondence/" + correspondenceId + "_" + correspondenceTypeName + ".docx");
            //    Object oallowsubstitution = System.Reflection.Missing.Value;

            //    wordDoc = word.Documents.Open(ref filepath, ref confirmconversion,
            //        ref readOnly, ref oMissing,
            //        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            //        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            //        ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            //    object fileFormat = WdSaveFormat.wdFormatDocument;
            //    wordDoc.SaveAs(ref saveto, ref fileFormat, ref oMissing, ref oMissing, ref oMissing,
            //        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            //        ref oMissing, ref oMissing, ref oMissing, ref oallowsubstitution, ref oMissing,
            //        ref oMissing);

            //    var pdfFile = Server.MapPath("~/Correspondence/" + correspondenceId + "_" + correspondenceTypeName + ".pdf");
            //    string wordFile = Path.ChangeExtension(pdfFile, ".docx");

            //    // Convert a PDF file to a Word file
            //    SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();

            //    f.OpenPdf(pdfFile);

            //    if (f.PageCount > 0)
            //    {
            //        // You may choose output format between Docx and Rtf.
            //        f.WordOptions.Format = SautinSoft.PdfFocus.CWordOptions.eWordDocument.Docx;

            //        int result = f.ToWord(wordFile);

            //        // Show the resulting Word document.
            //        if (result == 0)
            //        {
            //            System.Diagnostics.Process.Start(wordFile);
            //        }
            //    }


            //    DocX document = null;
            //    document = DocX.Create(Server.MapPath("~/Correspondence/" + correspondenceId + "_" + correspondenceTypeName + ".docx"), DocumentTypes.Document);
            //    var headLineFormat = new Formatting();
            //    headLineFormat.FontFamily = new Xceed.Words.NET.Font("Arial Black");// new System.Drawing.FontFamily("Arial Black");
            //    headLineFormat.Size = 18D;
            //    headLineFormat.Position = 12;

            //    string headlineText = correspondenceTypeName;
            //    document.InsertParagraph(headlineText, false, headLineFormat);
            //    var paraFormat = new Formatting();
            //    paraFormat.FontFamily = new Xceed.Words.NET.Font("Calibri");
            //    paraFormat.Size = 11.0f;
            //    paraFormat.CapsStyle = CapsStyle.none;

            //    // string p1TExt = @"Far far away, behind the word mountains, far from the countries Vokalia and Consonantia, there live the blind texts. Separated they live in Bookmarksgrove right at the coast of the Semantics, a large language ocean. A small river named Duden flows by their place and supplies it with the necessary regelialia. It is a paradisematic country.";
            //    document.InsertParagraph(body, false, paraFormat).Alignment = Alignment.left;
            //    document.InsertParagraph(" ");

            //    document.Save();
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
            return true;
        }

        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            cell.BorderColor = iTextSharp.text.BaseColor.WHITE;
            cell.VerticalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }

        [HttpPost]
        public bool checkDuplicateEmail(string email)
        {
            return _commonService.checkDuplicateEmail(email);
        }
        [HttpPost]
        public JsonResult GetUserBySearch(string Prefix)
        {
            var submitters = new List<ContactVM>();
            submitters = _adminService.GetSubmittersBySearch(Prefix);
            var submittersList = (from N in submitters
                                  where N.Name.StartsWith(Prefix)
                                  select new { N.Name, N.UserId });
            return Json(submittersList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveRequest(RequestVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.ApplicantContact != null)
                {
                    if (model.ApplicantContact.MobileNumber != null)
                        model.ApplicantContact.MobileNumber = model.ApplicantContact.MobileNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    if (model.ApplicantContact.OfficeNumber != null)
                        model.ApplicantContact.OfficeNumber = model.ApplicantContact.OfficeNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    if (model.ApplicantContact.Fax != null)
                        model.ApplicantContact.Fax = model.ApplicantContact.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                }
                if (model.SubmittingContact != null)
                {
                    if (model.SubmittingContact.MobileNumber != null)
                        model.SubmittingContact.MobileNumber = model.SubmittingContact.MobileNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    if (model.SubmittingContact.OfficeNumber != null)
                        model.SubmittingContact.OfficeNumber = model.SubmittingContact.OfficeNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    if (model.SubmittingContact.Fax != null)
                        model.SubmittingContact.Fax = model.SubmittingContact.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                }
                bool result = _requestService.SaveRequest(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Request saved successfully.";
                    return RedirectToAction("Index");
                }
            }
            TempData["ErrorMessage"] = "Request not saved.";
            return RedirectToAction("Create");
        }
        //public PartialViewResult GetSubmitters(string UserID)
        //{
        //    var requestVM = new RequestVM();
        //     //var loggedInUserId = _commonService.GetUserIdByUserName(User.Identity.Name);
        //    requestVM.SubmittingContact = _userService.GetSubmittingContactByUserId(UserID);
        //    return PartialView("_submitcontact", requestVM.SubmittingContact);
        //}
        public ActionResult GetSubmittingContactByUserID(string UserID)
        {
            var requestVM = new RequestVM();
            requestVM.SubmittingContact = _userService.GetSubmittingContactByUserId(UserID);
            return Json(requestVM, JsonRequestBehavior.AllowGet);
        }

        public FileResult GenerateAdminReportPdf(string date)
        {
            var tempHtmlTemplate = AppDomain.CurrentDomain.BaseDirectory + @"Content\AdminReport.html";
            var model = new List<ReportVM>();
            string[] startDateSplit = date.Split(',');
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (startDateSplit.Count() >= 2)
            {
                startDate = Convert.ToDateTime(startDateSplit[0]);
                endDate = (Convert.ToDateTime(startDateSplit[1])).AddHours(23).AddMinutes(59);
                //endDate = endDate.Value.AddHours(23).AddMinutes(59);
            }
            model = _adminService.GetReportData(startDate, endDate);

            string str = RenderPartialToString(this, "AdminReport", model, ViewData, TempData);
            StringBuilder sb = new StringBuilder();
            foreach (var item in model)
            {
                string htmlFormat =
                  $"<tr><td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{item.Request}</td>"
                    + $"<td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{item.EligibleProperties}</td>"
                    + $"<td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{item.InEligibleProperties}</td>"
                    + $"<td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{item.Unknown}</td>";
                sb.Append(htmlFormat);
            }
            str = str.Replace("#######", sb.ToString());
            System.IO.File.WriteAllText(tempHtmlTemplate, str);
            SelectPdf.HtmlToPdf pdfFile = new SelectPdf.HtmlToPdf();
            pdfFile.Options.MarginRight = 5;
            pdfFile.Options.MarginRight = 5;
            SelectPdf.PdfDocument document = pdfFile.ConvertUrl(tempHtmlTemplate);
            //Guid rand = Guid.NewGuid();
            //string pdf = Server.MapPath("/Content/test"+ rand+".pdf");
            byte[] pd = document.Save();
            document.Close();
            FileResult fileResult = new FileContentResult(pd, "application/pdf");
            fileResult.FileDownloadName = "Report" + DateTime.Now + ".pdf";
            return fileResult;
        }

        public FileResult GenerateProjectReportPdf()
        {
            var tempHtmlTemplate = AppDomain.CurrentDomain.BaseDirectory + @"Content\CoverSheet.html";
            var model = new List<CoverSheetVM>();
            model = _requestService.GetCoverSheetUsers();

            string str = RenderPartialToString(this, "ProjectReport", model, ViewData, TempData);
            StringBuilder sb = new StringBuilder();
            foreach (var item in model)
            {
                string htmlFormat = string.Format(
                  "<tr><td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{0}</td>"
                    + "<td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{1}</td>"
                    + "<td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{2}</td>"
                    + "<td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{3}</td>"
                    + "<td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{4}</td>"
                    + "<td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{5}</td>"
                    + "<td style='padding: 3px 15px; border: 1px solid #ddd; font-family:arial;'>{6}</td>"
                    , item.ProjectLogNumber, item.Date, item.ProjectName, item.ApplicantContact.Name, item.ApplicantContact.CountyName, item.LeadLegacy, item.ProjectDescription
                    );
                sb.Append(htmlFormat);
            }
            str = str.Replace("#######", sb.ToString());
            System.IO.File.WriteAllText(tempHtmlTemplate, str);
            SelectPdf.HtmlToPdf pdfFile = new SelectPdf.HtmlToPdf();
            pdfFile.Options.MarginRight = 5;
            pdfFile.Options.MarginRight = 5;
            SelectPdf.PdfDocument document = pdfFile.ConvertUrl(tempHtmlTemplate);
            //Guid rand = Guid.NewGuid();
            //string pdf = Server.MapPath("/Content/test"+ rand+".pdf");
            byte[] pd = document.Save();
            document.Close();
            FileResult fileResult = new FileContentResult(pd, "application/pdf");
            fileResult.FileDownloadName = "Document.pdf";
            return fileResult;
        }
        public static string RenderPartialToString(Controller controller, string partialViewName, object model, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            ViewEngineResult result = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialViewName);

            if (result.View != null)
            {
                controller.ViewData.Model = model;
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter output = new HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(controller.ControllerContext, result.View, viewData, tempData, output);
                        result.View.Render(viewContext, output);
                    }
                }

                return sb.ToString();
            }

            return String.Empty;
        }

        public string CoverPagePdf(long RequestId)
        {
            var requestVM = new RequestVM();
            requestVM = _requestService.GetRequestByRequestId(RequestId);
            string htmlFormat = string.Format(
                    "<div style='width:800px;margin: 0 auto;box-sizing:border-box;'>"
                    + "<table width = '100%' style='border-collapse:collapse'>"
                    + "<tr><td style='width: 150px; padding: 10px; font - family:arial;'><label>Project Log Number:</label></td><td>{0}</td></tr>"
                    + "<tr><td style='width: 150px; padding: 10px; font - family:arial;'><label>Date:</label></td><td>{1}</td></tr>"
                    + "<tr><td style='width: 150px; padding: 10px; font - family:arial;'><label>Applicant Name:</label></td><td>{2}</td></tr>"
                    + "<tr><td style='width: 150px; padding: 10px; font - family:arial;'><label>Project Name:</label></td><td>{3}</td></tr>"
                    + "<tr><td style='width: 150px; padding: 10px; font - family:arial;'><label>Project County:</label></td><td>{4}</td></tr>"
                    + "<tr><td style='width: 150px; padding: 10px; font - family:arial;'><label>Lead Agency:</label></td><td>{5}</td></tr>"
                    + "<tr><td colspan='2' style='padding: 10px; font - family:arial;'><label style='margin: 0 0 10px; display: inline-block; width: 100 %'>project Log Number:</label> <textarea style='width:100%; border: 1px solid #ccc; border-radius:3px; height:120px; padding:10px'>{6}</textarea></td></tr>"
                    + "</table>"
                    + "</div>"
                    , requestVM.ProjectLogNumber, requestVM.ProjectDate, requestVM.ApplicantContact.Name, requestVM.ProjectName, requestVM.ApplicantContact.CountyName, requestVM.AgencyName, requestVM.ProjectDescription
                    );
            return htmlFormat;
        }

        public ActionResult GetAccountUsers(string type)
        {
            var draw = Request.QueryString["draw"];
            var start = Request.QueryString["start"];
            var length = Request.QueryString["length"];
            var search = Request.QueryString["search[value]"];
            if (!string.IsNullOrEmpty(search))
            {
                Session["SelectedType"] = search;
            }
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var requestListVM = new List<ContactVM>();
            if (string.IsNullOrEmpty(search) && (Session["SelectedType"] == null))
            {
                requestListVM = _adminService.GetAccountUsers(1);
            }
            else
            {
                requestListVM = _adminService.GetAccountUsers(Convert.ToInt32(Session["SelectedType"]));
            }

            recordsTotal = requestListVM.Count();
            var data = requestListVM.Skip(skip).Take(pageSize).ToList();
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetReportData(string type)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var search = Request.QueryString["search[value]"];
            var filterDate = Request.Form.GetValues("filterDate").FirstOrDefault();
            if (!string.IsNullOrEmpty(search))
            {
                Session["SelectedType"] = search;
            }
            string[] startDateSplit = filterDate.Split(',');
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (startDateSplit.Count() >= 2)
            {
                startDate = Convert.ToDateTime(startDateSplit[0]);
                endDate = (Convert.ToDateTime(startDateSplit[1])).AddHours(23).AddMinutes(59);
                //endDate = endDate.Value.AddHours(23).AddMinutes(59);
            }
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var requestListVM = new List<ReportVM>();

            requestListVM = _adminService.GetReportData(startDate, endDate);


            recordsTotal = requestListVM.Count();
            var data = requestListVM.Skip(skip).Take(pageSize).ToList();
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }

    }
}