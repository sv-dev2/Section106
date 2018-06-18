using Section106.Models.Models;
using Section106.Repository.DataBase;
using Section106.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Section106.Models;
using Section106.Models.Enums;
using System.Data.Entity;
using System.Web;
using System.Web.Services.Description;
using System.Drawing;
using System.IO;

namespace Section106.Repository.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private Section106Entities _context;
        private ICommonRepository _commonRepository;
        private IUserRepository _userRepository;
        public RequestRepository(Section106Entities Context, ICommonRepository CommonRepository, IUserRepository UserRepository)
        {
            _context = Context;
            _commonRepository = CommonRepository;
            _userRepository = UserRepository;
        }

        public bool SaveRequest(RequestVM model)
        {
            // Saving Applicant Contact Info
            bool isContactSame = model.IsApplicantSameAsSubmitting;
            long applicantId = 0;
            if (!isContactSame)
            {
                var applicant = new Applicant()
                {
                    Name = model.ApplicantContact.Name,
                    Company = model.ApplicantContact.Company,
                    Email = model.ApplicantContact.Email,
                    Address1 = model.ApplicantContact.Address1,
                    Address2 = model.ApplicantContact.Address2,
                    StateId = model.ApplicantContact.StateId,
                    City = model.ApplicantContact.City,
                    Zip = model.ApplicantContact.Zip,
                    MobilePhone = model.ApplicantContact.MobileNumber != null ? model.ApplicantContact.MobileNumber : "",
                    OfficePhone = model.ApplicantContact.OfficeNumber,
                    Fax = model.ApplicantContact.Fax,
                    CountyId=model.ApplicantContact.CountyId
                };

                _context.Applicants.Add(applicant);
                _context.SaveChanges();
                applicantId = applicant.ApplicantId;
            }

            var randomProjectLogNumber = _commonRepository.GenerateRandomNumber();


            // Saving Request
            var request = new Request()
            {
                Status = model.Status,

                ApplicantContactId = applicantId,
                IsApplicantSameAsSubmitting = model.IsApplicantSameAsSubmitting,
                UserId = model.UserId,

                Name = model.ProjectName,
                Description = model.ProjectDescription,
                //LogNumber = model.IsNewSubmission ? randomProjectLogNumber : model.ProjectLogNumber,
                LogNumber = randomProjectLogNumber,
                Date = model.ProjectDate,
                Address1 = model.ProjectAddress1 == null ? "" : model.ProjectAddress1,
                Address2 = model.ProjectAddress2,
                CityId = model.ProjectCityId == null ? 0 : (int)model.ProjectCityId,
                CountyId = model.ProjectCountyId,
                Zip = model.ProjectZip,
                LongitudeDD = model.Longitude,
                LatitudeDD = model.Latitude,
                Township = model.TownShip,
                Range = model.Range,
                Section = model.Section,

                IsFederalProperty = model.IsFederalProperty,
                IsStateProperty = model.IsStateProperty,
                IsMunicipalProperty = model.IsMunicipalProperty,
                IsPrivateProperty = model.IsPrivateProperty,
                AgencyId = model.AgencyId,
                AgencyProjectNumber = model.AgencyProjectNumber,

                IsConstructionExcavation = model.IsConstructionExcavation,
                IsRehabilitation = model.IsRehabilitation,
                IsDemolition = model.IsDemolition,
                IsSaleOrTransfer = model.IsSaleOrTransfer,
                IsNonConstructionLoan = model.IsNonConstructionLoan,
                IsOther = model.IsOther,
                OtherDescription = model.IsOther ? model.OtherDescription : null,
                TotalProjectArea = model.TotalProjectArea,
                TotalGroundDisturbance = model.TotalGroundDisturbance,
                IsOldStructure = model.IsOldStructure,
                IsHistoricOrGovtProperty = model.IsHistoricOrGovtProperty,
                HistoricPropertyName = model.HistoricPropertyName,
                IsMississippiLandmarks=model.IsMississippiLandmarks,
                IsDocumentSentViaEmail = model.IsDocumentSentViaEmail
            };

            _context.Requests.Add(request);
            _context.SaveChanges();


            //Saving attachments 
            if (model.AttachmentList != null)
            {
                var attachments = model.AttachmentList.Split(';');
                if (attachments.Length > 0)
                {
                    foreach (var file in attachments)
                    {
                        var fileArr = file.Split(',');
                        byte[] fileBytes = Convert.FromBase64String(fileArr[0]);
                        var fileName = fileArr[1];
                        var fileType = fileArr[2];
                        var fileComment = fileArr[3];

                        //Save File to folder
                        string folderPath = HttpContext.Current.Server.MapPath("~/Attachments");
                        if (!Directory.Exists(folderPath))
                        {
                            //If Directory (Folder) does not exists. Create it.
                            Directory.CreateDirectory(folderPath);
                        }

                        string imgPath = Path.Combine(folderPath, request.RequestId + "_" + fileName);
                        File.WriteAllBytes(imgPath, fileBytes);


                        var attachment = new Attachment()
                        {
                            Name = fileName,
                            Type = fileType,
                            Comments = fileComment,
                            RequestId = request.RequestId
                        };

                        _context.Attachments.Add(attachment);
                        _context.SaveChanges();

                    }
                }
            }

            if (model.Status == Convert.ToInt32(RequestStatus.Submitted))
            {
                // Saving clock information
                var clock = new Clock()
                {
                    RequestId = request.RequestId,
                    ClockDate = DateTime.Now.AddDays(30),
                    ClockStatus = (Int16)(ClockStatus.Start)
                };
                _context.Clocks.Add(clock);
                _context.SaveChanges();
            }


            return true;
        }

        public bool UpdateRequest(RequestVM model)
        {
            // Updating Applicant Contact Info

            bool isContactSame = model.IsApplicantSameAsSubmitting;
            var requestDb = _context.Requests.Single(p => p.RequestId == model.RequestId);
            var requestStatus = requestDb.Status;
            var isPrevContactSame = requestDb.IsApplicantSameAsSubmitting == null ? false : requestDb.IsApplicantSameAsSubmitting.Value;
            long applicantId = 0;

            // Update same contact info to diff
            if (isPrevContactSame && !isContactSame)
            {
                var applicantContact = new Applicant()
                {
                    Name = model.ApplicantContact.Name,
                    Company = model.ApplicantContact.Company,
                    Email = model.ApplicantContact.Email,
                    Address1 = model.ApplicantContact.Address1,
                    Address2 = model.ApplicantContact.Address2,
                    StateId = model.ApplicantContact.StateId,
                    City = model.ApplicantContact.City,
                    Zip = model.ApplicantContact.Zip,
                    MobilePhone = model.ApplicantContact.MobileNumber,
                    OfficePhone = model.ApplicantContact.OfficeNumber,
                    Fax = model.ApplicantContact.Fax,
                    CountyId=model.ApplicantContact.CountyId
                };
                _context.Applicants.Add(applicantContact);
                _context.SaveChanges();

                applicantId = applicantContact.ApplicantId;
            }

            // Update diff contact info to same
            if (!isPrevContactSame && isContactSame)
            {
                var deleteApplicantContact = _context.Applicants.Single(p => p.ApplicantId == requestDb.ApplicantContactId);
                _context.Applicants.Remove(deleteApplicantContact);

                requestDb.IsApplicantSameAsSubmitting = isContactSame;
                requestDb.ApplicantContactId = 0;
                _context.SaveChanges();
            }

            // Update applicant contact info 
            if (!isPrevContactSame && !isContactSame)
            {
                var updateApplicantContact = _context.Applicants.Single(p => p.ApplicantId == requestDb.ApplicantContactId);
                applicantId = updateApplicantContact.ApplicantId;
                updateApplicantContact.Name = model.ApplicantContact.Name;
                updateApplicantContact.Company = model.ApplicantContact.Company;
                updateApplicantContact.Email = model.ApplicantContact.Email;
                updateApplicantContact.Address1 = model.ApplicantContact.Address1;
                updateApplicantContact.Address2 = model.ApplicantContact.Address2;
                updateApplicantContact.StateId = model.ApplicantContact.StateId;
                updateApplicantContact.City = model.ApplicantContact.City;
                updateApplicantContact.Zip = model.ApplicantContact.Zip;
                updateApplicantContact.MobilePhone = model.ApplicantContact.MobileNumber;
                updateApplicantContact.OfficePhone = model.ApplicantContact.OfficeNumber;
                updateApplicantContact.Fax = model.ApplicantContact.Fax;
                updateApplicantContact.CountyId = model.ApplicantContact.CountyId;
                _context.SaveChanges();
            }

            // updating Request
            requestDb.Status = model.Status;
            requestDb.ApplicantContactId = applicantId;
            requestDb.IsApplicantSameAsSubmitting = isContactSame;

            requestDb.Name = model.ProjectName;
            requestDb.Description = model.ProjectDescription;
            requestDb.Date = Convert.ToDateTime(model.ProjectDateStr);
            requestDb.Address1 = model.ProjectAddress1 != null ? model.ProjectAddress1 : "";
            requestDb.Address2 = model.ProjectAddress2;
            requestDb.CityId = model.ProjectCityId == null ? 0 : (int)model.ProjectCityId;
            requestDb.CountyId = model.ProjectCountyId;
            requestDb.Zip = model.ProjectZip;
            requestDb.LongitudeDD = model.Longitude;
            requestDb.LatitudeDD = model.Latitude;
            requestDb.Township = model.TownShip;
            requestDb.Range = model.Range;
            requestDb.Section = model.Section;

            requestDb.IsFederalProperty = model.IsFederalProperty;
            requestDb.IsStateProperty = model.IsStateProperty;
            requestDb.IsMunicipalProperty = model.IsMunicipalProperty;
            requestDb.IsPrivateProperty = model.IsPrivateProperty;
            requestDb.AgencyId = model.AgencyId;
            requestDb.AgencyProjectNumber = model.AgencyProjectNumber;

            requestDb.IsConstructionExcavation = model.IsConstructionExcavation;
            requestDb.IsRehabilitation = model.IsRehabilitation;
            requestDb.IsDemolition = model.IsDemolition;
            requestDb.IsSaleOrTransfer = model.IsSaleOrTransfer;
            requestDb.IsNonConstructionLoan = model.IsNonConstructionLoan;
            requestDb.IsOther = model.IsOther;
            requestDb.OtherDescription = model.IsOther ? model.OtherDescription : null;
            requestDb.TotalProjectArea = model.TotalProjectArea;
            requestDb.TotalGroundDisturbance = model.TotalGroundDisturbance;
            requestDb.IsOldStructure = model.IsOldStructure;
            requestDb.IsHistoricOrGovtProperty = model.IsHistoricOrGovtProperty;
            requestDb.HistoricPropertyName = model.HistoricPropertyName;
            requestDb.IsMississippiLandmarks = model.IsMississippiLandmarks;

            requestDb.IsDocumentSentViaEmail = model.IsDocumentSentViaEmail;
            _context.SaveChanges();


            //Updating attachments 
            if (model.AttachmentList != null)
            {
                var attachments = model.AttachmentList.Split(';');
                if (attachments.Length > 0)
                {
                    foreach (var file in attachments)
                    {
                        var fileArr = file.Split(',');

                        //adding new files
                        if (fileArr[0].ToLower() == "true")
                        {
                            byte[] fileBytes = Convert.FromBase64String(fileArr[1]);
                            var fileName = fileArr[2];
                            var fileType = fileArr[3];
                            var fileComment = fileArr[4];

                            //Save File to folder
                            string folderPath = HttpContext.Current.Server.MapPath("~/Attachments");
                            if (!Directory.Exists(folderPath))
                            {
                                //If Directory (Folder) does not exists. Create it.
                                Directory.CreateDirectory(folderPath);
                            }

                            string imgPath = Path.Combine(folderPath, model.RequestId + "_" + fileName);
                            File.WriteAllBytes(imgPath, fileBytes);


                            var attachment = new Attachment()
                            {
                                Name = fileName,
                                Type = fileType,
                                Comments = fileComment,
                                RequestId = model.RequestId ?? 0
                            };

                            _context.Attachments.Add(attachment);
                            _context.SaveChanges();
                        }
                        else
                        {
                            //updating files

                            var attachmentId = Convert.ToInt64(fileArr[2]);
                            var comment = fileArr[1];

                            var attachmentDb = _context.Attachments.Single(p => p.AttachmentId == attachmentId);
                            attachmentDb.Comments = comment;
                            _context.SaveChanges();
                        }

                    }
                }
            }

            if (model.Status == Convert.ToInt32(RequestStatus.Submitted))
            {
                var requestClock = _context.Clocks.SingleOrDefault(p => p.RequestId == model.RequestId);
                if (requestClock == null)
                {
                    // Saving clock information
                    var clock = new Clock()
                    {
                        RequestId = model.RequestId ?? 0,
                        ClockDate = DateTime.Now.AddDays(30),
                        ClockStatus = (Int16)(ClockStatus.Start)
                    };
                    _context.Clocks.Add(clock);
                    _context.SaveChanges();
                }

                // Start clock and set clock date according to the remaining days
                if (requestStatus == Convert.ToInt32(RequestStatus.Returned))
                {
                    requestClock.ClockStatus = Convert.ToInt16(ClockStatus.Start);
                    requestClock.ClockDate = DateTime.Now.AddDays(Convert.ToInt32(requestClock.RemainingDays));
                    requestClock.RemainingDays = null;
                    _context.SaveChanges();
                }
            }


            return true;
        }

        public RequestVM GetRequestByRequestId(long requestId)
        {
            RequestVM requestVM = new RequestVM();
            ContactVM contactVM = new ContactVM();

            try
            {
                var request = _context.Requests.Single(r => r.RequestId == requestId);
                var isRequestAssignedAlready = _context.RequestAssignments.Any(p => p.RequestId == requestId);
                var year = request.Date.Value.Year;
                var month = request.Date.Value.Month;
                var day = request.Date.Value.Day;

                requestVM = new RequestVM()
                {
                    Status = request.Status,
                    RequestId = requestId,
                    ApplicantContactId = request.ApplicantContactId,
                    IsApplicantSameAsSubmitting = request.IsApplicantSameAsSubmitting ?? false,
                    UserId = request.UserId,
                  
                    ProjectName = request.Name,
                    ProjectDescription = request.Description,
                    ProjectLogNumber = request.LogNumber,
                    ProjectDate = request.Date,
                    ProjectDateStr = year + "-" + (month <= 9 ? "0" + month : month.ToString()) + "-" + (day <= 9 ? "0" + day : day.ToString()), //"2018-01-08"
                    ProjectAddress1 = request.Address1,
                    ProjectAddress2 = request.Address2,
                    ProjectCityId = request.CityId,
                    ProjectCountyId = request.CountyId,
                    ProjectZip = request.Zip,
                    Longitude = request.LongitudeDD,
                    Latitude = request.LatitudeDD,
                    TownShip = request.Township,
                    Range = request.Range,
                    Section = request.Section,
                    AgencyName = request.Agency.Name,
                    IsFederalProperty = request.IsFederalProperty ?? false,
                    IsStateProperty = request.IsStateProperty ?? false,
                    IsMunicipalProperty = request.IsMunicipalProperty ?? false,
                    IsPrivateProperty = request.IsPrivateProperty ?? false,
                    AgencyId = request.AgencyId,
                    AgencyProjectNumber = request.AgencyProjectNumber,

                    IsConstructionExcavation = request.IsConstructionExcavation ?? false,
                    IsRehabilitation = request.IsRehabilitation ?? false,
                    IsDemolition = request.IsDemolition ?? false,
                    IsSaleOrTransfer = request.IsSaleOrTransfer ?? false,
                    IsNonConstructionLoan = request.IsNonConstructionLoan ?? false,
                    IsOther = request.IsOther ?? false,
                    OtherDescription = request.OtherDescription,
                    TotalProjectArea = request.TotalProjectArea,
                    TotalGroundDisturbance = request.TotalGroundDisturbance,
                    IsOldStructure = request.IsOldStructure ?? false,
                    IsHistoricOrGovtProperty = Convert.ToInt16(request.IsHistoricOrGovtProperty),
                    IsMississippiLandmarks=request.IsMississippiLandmarks??false,
                    HistoricPropertyName = request.HistoricPropertyName,
                    IsDocumentSentViaEmail = request.IsDocumentSentViaEmail ?? false,

                    IsRequestAssignedAlready = isRequestAssignedAlready
                };

                if (requestVM.IsApplicantSameAsSubmitting)
                {
                    contactVM = _userRepository.GetSubmittingContactByUserId(request.UserId);

                    requestVM.SubmittingContact = contactVM;
                    requestVM.ApplicantContact = contactVM;
                }
                else
                {
                    requestVM.SubmittingContact = _userRepository.GetSubmittingContactByUserId(request.UserId);
                    requestVM.ApplicantContact = GetApplicantContactById(requestVM.ApplicantContactId ?? 0);
                }

                requestVM.Attachments = _context.Attachments.Where(p => p.RequestId == requestId).Select(p => new AttachmentVM()
                {
                    AttachmentId = p.AttachmentId,
                    Name = p.Name,
                    Type = p.Type,
                    Comment = p.Comments
                }).ToList();

                requestVM.CorrespondenceList = _context.Correspondences.ToList().Where(p => p.RequestId == requestId).Select(p => new CorrespondenceVM()
                {
                    CorrespondenceId = p.CorrespondenceId,
                    CorrespondenceTypeId = p.CorrespondenceTypeId,
                    CorrespondenceTypeName = p.CorrespondenceType.Description,
                    Body = p.Body,
                    Date = p.Date.ToString("MM/dd/yyyy")
                }).ToList();

                if ((RequestStatus)request.Status == RequestStatus.Assigned || requestVM.IsRequestAssignedAlready.Value)
                {
                    var requestAssignment = _context.RequestAssignments.Single(p => p.RequestId == request.RequestId);
                    requestVM.RequestAssignment = new RequestAssignmentVM()
                    {
                        RequestAssignmentId = requestAssignment.RequestAssignmentId,
                        RequestId = requestAssignment.RequestId,
                        FederalOrState = requestAssignment.FederalOrState,
                        ProjectNumber = requestAssignment.ProjectNumber,
                        RespondDate = requestAssignment.RespondDate,
                        RespondDateStr = requestAssignment.RespondDate.Value.Year + "-" + (requestAssignment.RespondDate.Value.Month <= 9 ? "0" + requestAssignment.RespondDate.Value.Month : requestAssignment.RespondDate.Value.Month.ToString()) + "-" + (requestAssignment.RespondDate.Value.Day <= 9 ? "0" + requestAssignment.RespondDate.Value.Day : requestAssignment.RespondDate.Value.Day.ToString()), //"2018-01-08"
                        IsAssignToArchitect = requestAssignment.IsAssignToArchitect,
                        IsAssignToArchaelogical = requestAssignment.IsAssignToArchaelogical,
                        IsAssignToTechnical = requestAssignment.IsAssignToTechnical,
                        IsAssignToLandMarks=requestAssignment.IsAssignToLandMarks,
                        ArchitechStatus = (ReviewerRequestStatus)requestAssignment.ArchitechStatus,
                        ArchaelogicalStatus = (ReviewerRequestStatus)requestAssignment.ArchaelogicalStatus,
                        TechnicalStatus = (ReviewerRequestStatus)requestAssignment.TechnicalStatus,
                        LandMarksStatus= (ReviewerRequestStatus)requestAssignment.LandMarksStatus,
                    };

                    var requestResponse = _context.RequestResponses.SingleOrDefault(p => p.RequestId == request.RequestId && p.RequestAssignmentId == requestAssignment.RequestAssignmentId);
                    if (requestResponse != null)
                    {
                        requestVM.RequestResponse = new RequestResponseVM()
                        {
                            RequestResponseId = requestResponse.RequestResponseId,
                            RequestAssignmentId = requestResponse.RequestAssignmentId,
                            RequestId = requestResponse.RequestId,
                            IsNoHistoricProperty = requestResponse.IsNoHistoricProperty == null ? false : requestResponse.IsNoHistoricProperty ?? false,
                            IsHistoricProperty = requestResponse.IsHistoricProperty == null ? false : (bool)requestResponse.IsHistoricProperty,
                            //IsNoEffect = requestResponse.IsNoEffect == null ? false : requestResponse.IsNoEffect ?? false,
                            //IsNoAdverseEffect = requestResponse.IsNoAdverseEffect == null ? false : requestResponse.IsNoAdverseEffect ?? false,
                            //IsNoAdverseEffectWithCondition = requestResponse.IsNoAdverseEffectWithCondition == null ? false : requestResponse.IsNoAdverseEffectWithCondition ?? false,
                            ArchitectResponse = requestResponse.ArchitectResponse == null ? (short)0 : (short)0 ,
                            //ArchitectResponse = requestResponse.ArchitectResponse == null ? (short)0 : requestResponse.ArchitectResponse ?? 0,
                            //ArchitectComment = null,
                            ArchitectComment = requestResponse?.ArchitectComment,
                            ArchitectUserId = requestResponse.ArchitectUserId,


                            IsSurveyRequired = requestResponse.IsSurveyRequired == null ? false : requestResponse.IsSurveyRequired ?? false,
                            IsMoreInfoRequired = requestResponse.IsMoreInfoRequired == null ? false : requestResponse.IsMoreInfoRequired ?? false,
                            IsArchaeologyNoHistoricProperty = requestResponse.IsArchaeologyNoHistoricProperty == null ? false : requestResponse.IsArchaeologyNoHistoricProperty ?? false,
                            IsArchaeologyHistoricProperty = requestResponse.IsArchaeologyHistoricProperty == null ? false : requestResponse.IsArchaeologyHistoricProperty ?? false,

                            IsArchaeologyAdverseEffect = requestResponse.IsArchaeologyAdverseEffect == null ? false : (bool)requestResponse.IsArchaeologyAdverseEffect,
                            IsArchaeologyNoAdverseEffect = requestResponse.IsArchaeologyNoAdverseEffect == null ? false : (bool)requestResponse.IsArchaeologyNoAdverseEffect,
                            IsArchaeologyNoEffect = requestResponse.IsArchaeologyNoEffect == null ? false : (bool)requestResponse.IsArchaeologyNoEffect,

                            ArchaelogicalResponse = requestResponse.ArchaelogicalResponse == null ? (short)0 : requestResponse.ArchaelogicalResponse ?? 0,
                            ArchaelogicalComment = requestResponse.ArchaelogicalComment,
                            ArchaelogicalUserId = requestResponse.ArchaelogicalUserId,

                            LandMarksResponse = requestResponse.LankMarksResponse == null ? (short)0 : (short)0,
                            //ArchitectResponse = requestResponse.ArchitectResponse == null ? (short)0 : requestResponse.ArchitectResponse ?? 0,
                            //ArchitectComment = null,
                            LandmarksComment = requestResponse?.LankMarksComment,
                            LandmarksUserId = requestResponse.LankMarksUserId,

                            IsTechnicalAdverseEffect = requestResponse.IsTechnicalAdverseEffect == null ? false : (bool)requestResponse.IsTechnicalAdverseEffect,
                            IsTechnicalNoAdverseEffect = requestResponse.IsTechnicalNoAdverseEffect == null ? false : (bool)requestResponse.IsTechnicalNoAdverseEffect,
                            IsTechnicalNoEffect = requestResponse.IsTechnicalNoEffect == null ? false : (bool)requestResponse.IsTechnicalNoEffect,

                            TechnicalResponse = requestResponse.TechnicalResponse == null ? (short)0 : (short)0,
                            //TechnicalResponse = requestResponse.TechnicalResponse == null ? (short)0 : requestResponse.TechnicalResponse ?? 0,
                            TechnicalComment = null,
                            //TechnicalComment = requestResponse.TechnicalComment,
                            TechnicalUserId = requestResponse.TechnicalUserId,
                            EligibleProperties=requestResponse.ArchitectEligibleProperties,
                            InEligibleProperties=requestResponse.ArchitectInEligibleProperties,
                            UnknownProperties=requestResponse.ArchitectUnknownProperties
                        };
                    }
                }

                return requestVM;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<RequestVM> GetUserRequestsByStatus(string userId, List<int> status)
        {
            string statusString = String.Join(",", status);

            //var requiredRequests = _context.Requests.Where(p => p.UserId == userId && status.Contains(p.Status) && p.IsDeleted == false);
            //return requiredRequests.ToList().Select(p => new RequestVM()
            //{
            //    RequestId = p.RequestId,
            //    UserId = p.UserId,
            //    ProjectLogNumber = p.LogNumber,
            //    ProjectName = p.Name,
            //    ProjectDate = p.Date,
            //    Status = p.Status,
            //  // v  AssignmentStatus=p.
            //    StatusStr = ((RequestStatus)p.Status).ToString(),
            //    RemainingDays = GetRemainingDays(p.RequestId)
            //}).ToList();
            var requiredRequests = _context.GetSubmitterRequestsByStatus(statusString, userId).ToList();

            var data = requiredRequests.ToList().Select(p => new RequestVM()
            {
                RequestId = p.RequestId,
                UserId = p.UserId,
                ProjectLogNumber = p.LogNumber,
                ProjectName = p.Name,
                ProjectDate = p.Date,
                Status = p.Status,
                StatusStr = GetPublicStatus(p.Status),
                RemainingDays = GetRemainingDays(p.RequestId),
                AgencyName = _context.Agencies.Single(q => q.AgencyId == p.AgencyId).Name
            }).ToList();
            return data;
        }

        private string GetPublicStatus(int status)
        {
            var statusStr = ((PublicStatus)status).ToString();
            if (statusStr == "UnderArchitectureReview" || statusStr == "UnderArchaeologyReview" || statusStr == "UnderTechnicalReview")
            {
                return "Under Review";
            }
            return statusStr;
        }

        public ContactVM GetApplicantContactById(long Id)
        {
            var contact = _context.Applicants.FirstOrDefault(p => p.ApplicantId == Id);
            return new ContactVM()
            {
                Id = contact.ApplicantId,
                Name = contact.Name,
                Company = contact.Company,
                Address1 = contact.Address1,
                Address2 = contact.Address2,
                StateId = contact.StateId,
                City = contact.City,
                Email = contact.Email,
                Zip = contact.Zip,
                MobileNumber = contact.MobilePhone,
                OfficeNumber = contact.OfficePhone,
                Fax = contact.Fax,
                StateName = contact.State.Name,
                CountyName =contact.County.Name,
                CountyId=contact.CountyId
            };
        }

        public bool DeleteUserRequestByRequestId(long RequestId)
        {
            var request = _context.Requests.Single(p => p.RequestId == RequestId);
            request.IsDeleted = true;
            _context.SaveChanges();
            return true;
        }

        public bool DeleteAttachment(long Id)
        {
            var attachment = _context.Attachments.Single(p => p.AttachmentId == Id);

            // Remove file from Folder
            string folderPath = HttpContext.Current.Server.MapPath("~/Attachments");
            string imgPath = Path.Combine(folderPath, attachment.RequestId + "_" + attachment.Name);
            if ((System.IO.File.Exists(imgPath)))
            {
                System.IO.File.Delete(imgPath);
            }

            // Remove file from DB
            _context.Attachments.Remove(attachment);
            _context.SaveChanges();
            return true;
        }

        public string GetRemainingDays(long RequestId)
        {
            var request = _context.Requests.Single(p => p.RequestId == RequestId);
            if (request.Status == Convert.ToInt32(RequestStatus.Submitted) || request.Status == Convert.ToInt32(RequestStatus.Returned) || request.Status == Convert.ToInt32(RequestStatus.Assigned))
            {
                var clock = _context.Clocks.SingleOrDefault(p => p.RequestId == RequestId);
                if (clock != null)
                {
                    if (clock.ClockStatus == Convert.ToInt16(ClockStatus.Stop))
                    {
                        return clock.RemainingDays.ToString();
                    }
                    else
                    {
                        var daysDifference = clock.ClockDate.Value.Date.Subtract(DateTime.Now.Date).TotalDays;
                        return daysDifference.ToString();
                    }
                }
            }
            return null;
        }

        public List<CoverSheetVM> GetCoverSheetUsers()
        {
         
            try
            {
                var requestUsers1 = _context.Requests.ToList();
                List<CoverSheetVM> requestUsers = _context.Requests.ToList().Select(p => new CoverSheetVM
                {
                    ProjectLogNumber = p.LogNumber,
                    Date = p.Date,
                    ProjectName = p.Name,
                    ProjectDescription = p.Description,
                    LeadLegacy = p.Agency.Name,
                    ApplicantContact = _commonRepository.GetApplicantDetail(p.ApplicantContactId)
                }) .ToList();
                
                
                return requestUsers;
            }
            catch (Exception ex)
            {
                throw;
            }
        }




    }
}
