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
    public class RequestReviewRepository : IRequestReviewRepository
    {
        private Section106Entities _context;
        private ICommonRepository _commonRepository;

        public RequestReviewRepository(Section106Entities Context, ICommonRepository CommonRepository)
        {
            _context = Context;
            _commonRepository = CommonRepository;
        }

        public List<RequestVM> GetReviewerRequestsByRole(List<int> status, string userRole)
        {
            var requiredRequestIds = new List<long>();
            var moreIds = new List<long>();

            if (userRole == "Architectural")
            {
                requiredRequestIds = _context.RequestAssignments.Where(p =>
                                    status.Contains(p.Request.Status) &&
                                    p.Request.IsDeleted == false &&
                                    p.IsAssignToArchitect == true &&
                                    (ReviewerRequestStatus)p.ArchitechStatus == ReviewerRequestStatus.MoreInfoRequired).Select(p => p.RequestId).ToList();

                //if (requiredRequestIds.Count < 3)
                //{
                //    int restRecords = 3 - requiredRequestIds.Count;
                moreIds = _context.RequestAssignments.Where(p => status.Contains(p.Request.Status) &&
                            p.Request.IsDeleted == false &&
                            p.IsAssignToArchitect == true &&
                            ((ReviewerRequestStatus)p.ArchitechStatus == ReviewerRequestStatus.Pending || (ReviewerRequestStatus)p.ArchitechStatus == ReviewerRequestStatus.SurveyRequired)).Select(p => p.RequestId).ToList();
                requiredRequestIds.AddRange(moreIds);
                //}
            }

            else if (userRole == "Archaeology")
            {
                requiredRequestIds = _context.RequestAssignments.Where(p =>
                                    status.Contains(p.Request.Status) &&
                                    p.Request.IsDeleted == false &&
                                    p.IsAssignToArchaelogical == true &&
                                    (ReviewerRequestStatus)p.ArchaelogicalStatus == ReviewerRequestStatus.MoreInfoRequired &&
                                    (p.IsAssignToArchitect == true ? (ReviewerRequestStatus)p.ArchitechStatus == ReviewerRequestStatus.Approved : true)).Select(p => p.RequestId).ToList();

                //if (requiredRequestIds.Count < 3)
                //{
                // int restRecords = 3 - requiredRequestIds.Count;
                moreIds = _context.RequestAssignments.Where(p =>
                            status.Contains(p.Request.Status) &&
                            p.Request.IsDeleted == false &&
                            // (p.IsAssignToArchitect == true &&
                            // ((ReviewerRequestStatus)p.ArchitechStatus== ReviewerRequestStatus.MoreInfoRequired)) ||
                            (p.IsAssignToArchaelogical == true &&
                            ((ReviewerRequestStatus)p.ArchaelogicalStatus == ReviewerRequestStatus.Pending || (ReviewerRequestStatus)p.ArchaelogicalStatus == ReviewerRequestStatus.SurveyRequired || (ReviewerRequestStatus)p.ArchaelogicalStatus == ReviewerRequestStatus.MoreInfoRequired)) &&
                            (p.IsAssignToArchitect == true ? ((ReviewerRequestStatus)p.ArchitechStatus == ReviewerRequestStatus.Approved || (ReviewerRequestStatus)p.ArchitechStatus == ReviewerRequestStatus.MoreInfoRequired) : true)).Select(p => p.RequestId).ToList();

                //    requiredRequestIds.AddRange(moreIds);
                //}
            }

            else if (userRole == "Technical Assistance")
            {
                requiredRequestIds = _context.RequestAssignments.Where(p =>
                status.Contains(p.Request.Status) &&
                p.Request.IsDeleted == false &&
                p.IsAssignToTechnical == true &&
                (ReviewerRequestStatus)p.TechnicalStatus == ReviewerRequestStatus.MoreInfoRequired &&
                (p.IsAssignToArchitect == true ? (ReviewerRequestStatus)p.ArchitechStatus == ReviewerRequestStatus.Approved : true) &&
                (p.IsAssignToArchaelogical == true ? (ReviewerRequestStatus)p.ArchaelogicalStatus == ReviewerRequestStatus.Approved : true)).Select(p => p.RequestId).ToList();

                //if (requiredRequestIds.Count < 3)
                //{
                // int restRecords = 3 - requiredRequestIds.Count;
                moreIds = _context.RequestAssignments.Where(p =>
                  status.Contains(p.Request.Status) &&
                  p.Request.IsDeleted == false &&
                  p.IsAssignToTechnical == true &&
                  ((ReviewerRequestStatus)p.TechnicalStatus == ReviewerRequestStatus.Pending || (ReviewerRequestStatus)p.TechnicalStatus == ReviewerRequestStatus.SurveyRequired) &&
                  (p.IsAssignToArchitect == true ? ((ReviewerRequestStatus)p.ArchitechStatus == ReviewerRequestStatus.Approved || (ReviewerRequestStatus)p.ArchitechStatus == ReviewerRequestStatus.MoreInfoRequired) : true) &&
                  (p.IsAssignToArchaelogical == true ? ((ReviewerRequestStatus)p.ArchaelogicalStatus == ReviewerRequestStatus.Approved || (ReviewerRequestStatus)p.ArchaelogicalStatus == ReviewerRequestStatus.MoreInfoRequired) : true)).Select(p => p.RequestId).ToList();

                requiredRequestIds.AddRange(moreIds);
                //}
            }

            else if (userRole == "Landmarks")
            {
                requiredRequestIds = _context.RequestAssignments.Where(p =>
                status.Contains(p.Request.Status) &&
                p.Request.IsDeleted == false &&
                p.IsAssignToLandMarks == true &&
                (ReviewerRequestStatus)p.LandMarksStatus == ReviewerRequestStatus.MoreInfoRequired).Select(p => p.RequestId).ToList();

                //if (requiredRequestIds.Count < 3)
                //{
                // int restRecords = 3 - requiredRequestIds.Count;
                moreIds = _context.RequestAssignments.Where(p =>
                  status.Contains(p.Request.Status) &&
                  p.Request.IsDeleted == false &&
                  p.IsAssignToLandMarks == true &&
                  ((ReviewerRequestStatus)p.LandMarksStatus == ReviewerRequestStatus.Pending || (ReviewerRequestStatus)p.LandMarksStatus == ReviewerRequestStatus.SurveyRequired)).Select(p => p.RequestId).ToList();

                requiredRequestIds.AddRange(moreIds);
                //}
            }
            return _context.Requests.Where(p => requiredRequestIds.Contains(p.RequestId)).Select(p => new RequestVM()
            {
                RequestId = p.RequestId,
                UserId = p.UserId,
                ProjectLogNumber = p.LogNumber,
                ProjectName = p.Name,
                ProjectDate = p.Date,
                StatusStr = (ReviewerRequestStatus.Pending).ToString(),
                //}).ToList();
            }).OrderBy(r => r.ProjectDate).ToList();
        }

        public bool SaveReviewerResponse(RequestVM model, bool IsReqAddInfo, string userId, string userRole)
        {
            if (model.RequestId > 0 && model.RequestAssignment.RequestAssignmentId > 0)
            {
                if (userRole == "Architectural")
                {
                    return SaveArchitecturalResponse(model, IsReqAddInfo, userId);
                }
                else if (userRole == "Archaeology")
                {
                    return SaveArchaeologyResponse(model, IsReqAddInfo, userId);
                }
                else if (userRole == "Technical Assistance")
                {
                    return SaveTechnicalAssistanceResponse(model, userId);
                }
                else if (userRole == "Landmarks")
                {
                    return SaveLandmarksResponse(model, IsReqAddInfo, userId);
                }
            }
            return false;
        }
        private bool SaveArchitecturalResponse(RequestVM model, bool? IsReqAddInfo, string userId)
        {
            var responseDb = _context.RequestResponses.SingleOrDefault(p => p.RequestId == model.RequestId && p.RequestAssignmentId == model.RequestAssignment.RequestAssignmentId);
            if (responseDb != null)
            {
                if (IsReqAddInfo == true)
                {
                    responseDb.ArchitectResponse = Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation);
                }
                else
                {
                    responseDb.ArchitectResponse = Convert.ToInt16(ReviewerResponse.Eligible); // if request more information checkbox is not checked that means approved for now
                }

                responseDb.IsNoHistoricProperty = model.RequestResponse.IsNoHistoricProperty;
                responseDb.IsHistoricProperty = model.RequestResponse.IsHistoricProperty;
                //responseDb.IsNoEffect = model.RequestResponse.IsNoEffect;
                //responseDb.IsNoAdverseEffect = model.RequestResponse.IsNoAdverseEffect;
                //responseDb.IsNoAdverseEffectWithCondition = model.RequestResponse.IsNoAdverseEffectWithCondition;
                responseDb.ArchitectComment = model.RequestResponse.ArchitectComment;
                responseDb.ArchitectUserId = userId;
                responseDb.ArchitectUnknownProperties = model.RequestResponse.UnknownProperties;
                responseDb.ArchitectEligibleProperties = model.RequestResponse.EligibleProperties;
                responseDb.ArchitectInEligibleProperties = model.RequestResponse.InEligibleProperties;
                //responseDb.
                _context.SaveChanges();
            }
            else
            {
                var response = new RequestResponse()
                {
                    RequestId = model.RequestId ?? 0,
                    RequestAssignmentId = model.RequestAssignment.RequestAssignmentId ?? 0,

                    IsHistoricProperty = model.RequestResponse.IsHistoricProperty,
                    IsNoHistoricProperty = model.RequestResponse.IsNoHistoricProperty,
                    //IsNoEffect = model.RequestResponse.IsNoEffect,
                    //IsNoAdverseEffect = model.RequestResponse.IsNoAdverseEffect,
                    //IsNoAdverseEffectWithCondition = model.RequestResponse.IsNoAdverseEffectWithCondition,

                    ArchitectResponse = IsReqAddInfo == true ? Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation) : Convert.ToInt16(ReviewerResponse.Eligible),
                    ArchitectComment = model.RequestResponse.ArchitectComment,
                    ArchitectUserId = userId,
                    ArchitectEligibleProperties = model.RequestResponse.EligibleProperties,
                    ArchitectInEligibleProperties = model.RequestResponse.InEligibleProperties,
                    ArchitectUnknownProperties = model.RequestResponse.UnknownProperties
                };
                _context.RequestResponses.Add(response);
                _context.SaveChanges();
            }

            var requestAssignment = _context.RequestAssignments.Single(p => p.RequestAssignmentId == model.RequestAssignment.RequestAssignmentId);

            if (model.RequestResponse.ResponseType == ReviewerResponseType.SaveAndSubmit)
            {
                //if (model.RequestResponse.ArchitectResponse == Convert.ToInt16(ReviewerResponse.Eligible))
                //{
                //    requestAssignment.ArchitechStatus = Convert.ToInt32(ReviewerRequestStatus.Approved);
                //}
                //else if (model.RequestResponse.ArchitectResponse == Convert.ToInt16(ReviewerResponse.NotEligible))
                //{
                //    requestAssignment.ArchitechStatus = Convert.ToInt32(ReviewerRequestStatus.Rejected);
                //}
                if (IsReqAddInfo == true)
                {
                    requestAssignment.ArchitechStatus = Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired);
                }
                else
                {
                    requestAssignment.ArchitechStatus = Convert.ToInt32(ReviewerRequestStatus.Approved); // Approved for now if reviewer does not need more information.
                }

                if (model.RequestResponse.ArchitectResponse == Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation))
                {
                    requestAssignment.ArchitechStatus = Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired);
                    // Stop internal clock in case of reviewer need more information
                    var internalClock = _context.Clocks.Single(p => p.RequestId == model.RequestId);
                    internalClock.InternalClockStatus = Convert.ToInt16(ClockStatus.Stop);
                    var internalRemainingDays = internalClock.InternalClockDate.Value.Date.Subtract(DateTime.Now.Date).TotalDays;
                    internalClock.InternalRemainingDays = Convert.ToInt32(internalRemainingDays);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
            return true;
        }
        private bool SaveLandmarksResponse(RequestVM model, bool? IsReqAddInfo, string userId)
        {
            var responseDb = _context.RequestResponses.SingleOrDefault(p => p.RequestId == model.RequestId && p.RequestAssignmentId == model.RequestAssignment.RequestAssignmentId);
            if (responseDb != null)
            {
                if (IsReqAddInfo == true)
                {
                    responseDb.LankMarksResponse = Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation);
                }
                else
                {
                    responseDb.LankMarksResponse = Convert.ToInt16(ReviewerResponse.Eligible); // if request more information checkbox is not checked that means approved for now
                }

                responseDb.IsNoHistoricProperty = model.RequestResponse.IsNoHistoricProperty;
                responseDb.IsHistoricProperty = model.RequestResponse.IsHistoricProperty;
                //responseDb.IsNoEffect = model.RequestResponse.IsNoEffect;
                //responseDb.IsNoAdverseEffect = model.RequestResponse.IsNoAdverseEffect;
                //responseDb.IsNoAdverseEffectWithCondition = model.RequestResponse.IsNoAdverseEffectWithCondition;
                responseDb.LankMarksComment = model.RequestResponse.LandmarksComment;
                responseDb.LankMarksUserId = userId;
                responseDb.ArchitectUnknownProperties = model.RequestResponse.UnknownProperties;
                responseDb.ArchitectEligibleProperties = model.RequestResponse.EligibleProperties;
                responseDb.ArchitectInEligibleProperties = model.RequestResponse.InEligibleProperties;
                //responseDb.
                _context.SaveChanges();
            }
            else
            {
                var response = new RequestResponse()
                {
                    RequestId = model.RequestId ?? 0,
                    RequestAssignmentId = model.RequestAssignment.RequestAssignmentId ?? 0,

                    IsHistoricProperty = model.RequestResponse.IsHistoricProperty,
                    IsNoHistoricProperty = model.RequestResponse.IsNoHistoricProperty,
                    //IsNoEffect = model.RequestResponse.IsNoEffect,
                    //IsNoAdverseEffect = model.RequestResponse.IsNoAdverseEffect,
                    //IsNoAdverseEffectWithCondition = model.RequestResponse.IsNoAdverseEffectWithCondition,

                    LankMarksResponse = IsReqAddInfo == true ? Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation) : Convert.ToInt16(ReviewerResponse.Eligible),
                    LankMarksComment = model.RequestResponse.LandmarksComment,
                    LankMarksUserId = userId,
                    ArchitectEligibleProperties = model.RequestResponse.EligibleProperties,
                    ArchitectInEligibleProperties = model.RequestResponse.InEligibleProperties,
                    ArchitectUnknownProperties = model.RequestResponse.UnknownProperties
                };
                _context.RequestResponses.Add(response);
                _context.SaveChanges();
            }

            var requestAssignment = _context.RequestAssignments.Single(p => p.RequestAssignmentId == model.RequestAssignment.RequestAssignmentId);

            if (model.RequestResponse.ResponseType == ReviewerResponseType.SaveAndSubmit)
            {
               
                if (IsReqAddInfo == true)
                {
                    requestAssignment.LandMarksStatus = Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired);
                }
                else
                {
                    requestAssignment.LandMarksStatus = Convert.ToInt32(ReviewerRequestStatus.Approved); // Approved for now if reviewer does not need more information.
                }

                if (model.RequestResponse.LandMarksResponse == Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation))
                {
                    requestAssignment.LandMarksStatus = Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired);
                    // Stop internal clock in case of reviewer need more information
                    var internalClock = _context.Clocks.Single(p => p.RequestId == model.RequestId);
                    internalClock.InternalClockStatus = Convert.ToInt16(ClockStatus.Stop);
                    var internalRemainingDays = internalClock.InternalClockDate.Value.Date.Subtract(DateTime.Now.Date).TotalDays;
                    internalClock.InternalRemainingDays = Convert.ToInt32(internalRemainingDays);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
            return true;
        }

        private bool SaveArchaeologyResponse(RequestVM model, bool IsReqAddInfo, string userId)
        {
            var requestResponse = _context.RequestResponses.SingleOrDefault(p => p.RequestId == model.RequestId && p.RequestAssignmentId == model.RequestAssignment.RequestAssignmentId);
            if (requestResponse != null)
            {
                requestResponse.IsSurveyRequired = model.RequestResponse.IsSurveyRequired;
                requestResponse.IsMoreInfoRequired = model.RequestResponse.IsMoreInfoRequired;
                requestResponse.IsArchaeologyNoHistoricProperty = model.RequestResponse.IsArchaeologyNoHistoricProperty;
                requestResponse.IsArchaeologyHistoricProperty = model.RequestResponse.IsArchaeologyHistoricProperty;

                if (model.RequestResponse.IsArchaeologyHistoricProperty)
                {
                    requestResponse.IsArchaeologyNoEffect = model.RequestResponse.IsArchaeologyNoEffect;
                    requestResponse.IsArchaeologyNoAdverseEffect = model.RequestResponse.IsArchaeologyNoAdverseEffect;
                    requestResponse.IsArchaeologyAdverseEffect = model.RequestResponse.IsArchaeologyAdverseEffect;
                }
                else
                {
                    requestResponse.IsArchaeologyNoEffect = false;
                    requestResponse.IsArchaeologyNoAdverseEffect = false;
                    requestResponse.IsArchaeologyAdverseEffect = false;
                }

                if (IsReqAddInfo)
                {
                    requestResponse.ArchaelogicalResponse = Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation);
                }
                else
                {
                    requestResponse.ArchaelogicalResponse = Convert.ToInt16(ReviewerResponse.Eligible);
                }
                requestResponse.ArchaelogicalComment = model.RequestResponse.ArchaelogicalComment;
                requestResponse.ArchaelogicalUserId = userId;
                requestResponse.ArchitectEligibleProperties = model.RequestResponse.EligibleProperties;
                requestResponse.ArchitectInEligibleProperties = model.RequestResponse.InEligibleProperties;
                requestResponse.ArchitectUnknownProperties = model.RequestResponse.UnknownProperties;
                _context.SaveChanges();
            }
            else
            {
                var response = new RequestResponse()
                {
                    RequestId = model.RequestId ?? 0,
                    RequestAssignmentId = model.RequestAssignment.RequestAssignmentId ?? 0,

                    IsSurveyRequired = model.RequestResponse.IsSurveyRequired,
                    IsMoreInfoRequired = model.RequestResponse.IsMoreInfoRequired,
                    IsArchaeologyNoHistoricProperty = model.RequestResponse.IsArchaeologyNoHistoricProperty,
                    IsArchaeologyHistoricProperty = model.RequestResponse.IsArchaeologyHistoricProperty,

                    IsArchaeologyNoEffect = model.RequestResponse.IsArchaeologyHistoricProperty ? model.RequestResponse.IsArchaeologyNoEffect : false,
                    IsArchaeologyNoAdverseEffect = model.RequestResponse.IsArchaeologyHistoricProperty ? model.RequestResponse.IsArchaeologyNoAdverseEffect : false,
                    IsArchaeologyAdverseEffect = model.RequestResponse.IsArchaeologyHistoricProperty ? model.RequestResponse.IsArchaeologyAdverseEffect : false,

                    ArchaelogicalResponse = IsReqAddInfo ? Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation) : Convert.ToInt16(ReviewerResponse.Eligible),
                    ArchaelogicalComment = model.RequestResponse.ArchaelogicalComment,
                    ArchaelogicalUserId = userId,
                    ArchitectEligibleProperties = model.RequestResponse.EligibleProperties,
                    ArchitectInEligibleProperties = model.RequestResponse.InEligibleProperties,
                    ArchitectUnknownProperties = model.RequestResponse.UnknownProperties
                };
                _context.RequestResponses.Add(response);
                _context.SaveChanges();
            }

            var requestAssignment = _context.RequestAssignments.Single(p => p.RequestAssignmentId == model.RequestAssignment.RequestAssignmentId);

            if (model.RequestResponse.ResponseType == ReviewerResponseType.SaveAndSubmit)
            {
                //if (model.RequestResponse.ArchaelogicalResponse == Convert.ToInt16(ReviewerResponse.Eligible))
                //{
                //    requestAssignment.ArchaelogicalStatus = Convert.ToInt32(ReviewerRequestStatus.Approved);
                //}
                //else if (model.RequestResponse.ArchaelogicalResponse == Convert.ToInt16(ReviewerResponse.NotEligible))
                //{
                //    requestAssignment.ArchaelogicalStatus = Convert.ToInt32(ReviewerRequestStatus.Rejected);
                //}
                //if (model.RequestResponse.ArchaelogicalResponse == Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation))
                //{
                //    requestAssignment.ArchaelogicalStatus = Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired);
                //}
                if (IsReqAddInfo)
                {
                    requestAssignment.ArchaelogicalStatus = Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired);
                }
                else
                {
                    requestAssignment.ArchaelogicalStatus = Convert.ToInt32(ReviewerRequestStatus.Approved);
                }

                if (model.RequestResponse.ArchaelogicalResponse == Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation) ||
                    model.RequestResponse.ArchaelogicalResponse == Convert.ToInt16(ReviewerResponse.SurveyRequired))
                {
                    // Stop internal clock in case of reviewer need more information or survey required
                    var internalClock = _context.Clocks.Single(p => p.RequestId == model.RequestId);
                    internalClock.InternalClockStatus = Convert.ToInt16(ClockStatus.Stop);
                    var internalRemainingDays = internalClock.InternalClockDate.Value.Date.Subtract(DateTime.Now.Date).TotalDays;
                    internalClock.InternalRemainingDays = Convert.ToInt32(internalRemainingDays);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
            return true;
        }
        private bool SaveTechnicalAssistanceResponse(RequestVM model, string userId)
        {
            var requestResponse = _context.RequestResponses.SingleOrDefault(p => p.RequestId == model.RequestId && p.RequestAssignmentId == model.RequestAssignment.RequestAssignmentId);
            if (requestResponse != null)
            {
                requestResponse.IsTechnicalAdverseEffect = model.RequestResponse.IsTechnicalAdverseEffect;
                requestResponse.IsTechnicalNoAdverseEffect = model.RequestResponse.IsTechnicalNoAdverseEffect;
                requestResponse.IsTechnicalNoEffect = model.RequestResponse.IsTechnicalNoEffect;

                requestResponse.TechnicalResponse = Convert.ToInt16(model.RequestResponse.TechnicalResponse);
                requestResponse.TechnicalComment = model.RequestResponse.TechnicalComment;
                requestResponse.TechnicalUserId = userId;
                _context.SaveChanges();
            }
            else
            {
                var response = new RequestResponse()
                {
                    RequestId = model.RequestId ?? 0,
                    RequestAssignmentId = model.RequestAssignment.RequestAssignmentId ?? 0,

                    IsTechnicalAdverseEffect = model.RequestResponse.IsTechnicalAdverseEffect,
                    IsTechnicalNoAdverseEffect = model.RequestResponse.IsTechnicalNoAdverseEffect,
                    IsTechnicalNoEffect = model.RequestResponse.IsTechnicalNoEffect,

                    TechnicalResponse = Convert.ToInt16(model.RequestResponse.TechnicalResponse),
                    TechnicalComment = model.RequestResponse.TechnicalComment,
                    TechnicalUserId = userId
                };
                _context.RequestResponses.Add(response);
                _context.SaveChanges();
            }

            var requestAssignment = _context.RequestAssignments.Single(p => p.RequestAssignmentId == model.RequestAssignment.RequestAssignmentId);

            if (model.RequestResponse.ResponseType == ReviewerResponseType.SaveAndSubmit)
            {
                if (model.RequestResponse.TechnicalResponse == Convert.ToInt16(ReviewerResponse.Eligible))
                {
                    requestAssignment.TechnicalStatus = Convert.ToInt32(ReviewerRequestStatus.Approved);
                }
                else if (model.RequestResponse.TechnicalResponse == Convert.ToInt16(ReviewerResponse.NotEligible))
                {
                    requestAssignment.TechnicalStatus = Convert.ToInt32(ReviewerRequestStatus.Rejected);
                }
                else if (model.RequestResponse.TechnicalResponse == Convert.ToInt16(ReviewerResponse.RequestAdditionalInformation))
                {
                    requestAssignment.TechnicalStatus = Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired);

                    //Stop internal clock in case of reviewer need more information
                    var internalClock = _context.Clocks.Single(p => p.RequestId == model.RequestId);
                    internalClock.InternalClockStatus = Convert.ToInt16(ClockStatus.Stop);
                    var internalRemainingDays = internalClock.InternalClockDate.Value.Date.Subtract(DateTime.Now.Date).TotalDays;
                    internalClock.InternalRemainingDays = Convert.ToInt32(internalRemainingDays);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
            return true;
        }
    }
}
