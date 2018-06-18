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
    public class AdminRepository : IAdminRepository
    {
        private Section106Entities _context;
        private ICommonRepository _commonRepository;
        private IUserRepository _userRepository;
        public AdminRepository(Section106Entities Context, ICommonRepository CommonRepository, IUserRepository UserRepository)
        {
            _context = Context;
            _commonRepository = CommonRepository;
            _userRepository = UserRepository;
        }

        #region "Requests"

        public List<RequestVM> GetUserRequestsByStatus(List<int> statusList)
        {
            var requiredRequests = _context.Requests.Where(p => statusList.Contains(p.Status) && p.IsDeleted == false);
            return requiredRequests.ToList().Select(p => new RequestVM()
            {
                RequestId = p.RequestId,
                UserId = p.UserId,
                UserName = _context.Entities.Single(q => q.UserId == p.UserId).Name,
                ProjectName = p.Name,
                ProjectDate = p.Date,
                Status = p.Status,
                StatusStr = ((RequestStatus)p.Status).ToString(),
                IsRequestAssignedAlready = _context.RequestAssignments.Any(q => q.RequestId == p.RequestId),
                InternalRemainingDays = GetInternalRemainingDays(p.RequestId)
            }).ToList();
        }

        public string GetInternalRemainingDays(long RequestId)
        {
            var request = _context.Requests.Single(p => p.RequestId == RequestId);
            if (request.Status == Convert.ToInt32(RequestStatus.Assigned) || request.Status == Convert.ToInt32(RequestStatus.Submitted) || request.Status == Convert.ToInt32(RequestStatus.Returned))
            {
                var clock = _context.Clocks.SingleOrDefault(p => p.RequestId == RequestId);
                if (clock != null && clock.InternalClockStatus != null)
                {
                    if (clock.InternalClockStatus == Convert.ToInt16(ClockStatus.Stop))
                    {
                        return clock.InternalRemainingDays.ToString();
                    }
                    else
                    {
                        var daysDifference = clock.InternalClockDate.Value.Date.Subtract(DateTime.Now.Date).TotalDays;
                        return daysDifference.ToString();
                    }
                }
            }
            return null;
        }

        public bool SaveRequestAssignment(RequestVM model)
        {
            if (model.RequestId > 0)
            {
                var request = _context.Requests.Single(p => p.RequestId == model.RequestId);

                // If request already assign
                if (model.IsRequestAssignedAlready == true)
                {
                    var requestAssignment = _context.RequestAssignments.Single(p => p.RequestId == model.RequestId);
                    var requestReviewerResponse = _context.RequestResponses.SingleOrDefault(p => p.RequestId == model.RequestId);

                    if (requestAssignment.IsAssignToArchitect)
                    {
                        if (requestAssignment.ArchitechStatus == Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired))
                        {
                            // Update Architect Response
                            if (requestReviewerResponse != null)
                            {
                                requestReviewerResponse.IsNoHistoricProperty = null;
                                requestReviewerResponse.IsHistoricProperty = null;
                                //requestReviewerResponse.IsNoEffect = null;
                                //requestReviewerResponse.IsNoAdverseEffect = null;
                                //requestReviewerResponse.IsNoAdverseEffectWithCondition = null;

                                requestReviewerResponse.ArchitectResponse = null;
                                requestReviewerResponse.ArchitectComment = null;
                                requestReviewerResponse.ArchitectUserId = null;
                            }

                            // Update Architect Assignment Status
                            requestAssignment.ArchitechStatus = Convert.ToInt32(ReviewerRequestStatus.Pending);
                        }
                    }
                    if (requestAssignment.IsAssignToArchaelogical)
                    {
                        if (requestAssignment.ArchaelogicalStatus == Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired) || requestAssignment.ArchaelogicalStatus == Convert.ToInt32(ReviewerRequestStatus.SurveyRequired))
                        {
                            // Update Archaelogical Response
                            if (requestReviewerResponse != null)
                            {
                                requestReviewerResponse.ArchaelogicalResponse = null;
                                requestReviewerResponse.ArchaelogicalComment = null;
                                requestReviewerResponse.ArchaelogicalUserId = null;
                            }
                            // Update Archaelogical Assignment Status
                            requestAssignment.ArchaelogicalStatus = Convert.ToInt32(ReviewerRequestStatus.Pending);
                        }
                    }
                    if (requestAssignment.IsAssignToTechnical)
                    {
                        if (requestAssignment.TechnicalStatus == Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired))
                        {
                            // Update Technical Response
                            if (requestReviewerResponse != null)
                            {
                                requestReviewerResponse.TechnicalResponse = null;
                                requestReviewerResponse.TechnicalComment = null;
                                requestReviewerResponse.TechnicalUserId = null;
                            }

                            // Update Technical Assignment Status
                            requestAssignment.TechnicalStatus = Convert.ToInt32(ReviewerRequestStatus.Pending);
                        }
                    }
                    if (requestAssignment.IsAssignToLandMarks)
                    {
                        if (requestAssignment.LandMarksStatus == Convert.ToInt32(ReviewerRequestStatus.MoreInfoRequired))
                        {
                            // Update Technical Response
                            if (requestReviewerResponse != null)
                            {
                                requestReviewerResponse.LankMarksResponse = null;
                                requestReviewerResponse.LankMarksComment = null;
                                requestReviewerResponse.LankMarksUserId = null;
                            }

                            // Update Technical Assignment Status
                            requestAssignment.LandMarksStatus = Convert.ToInt32(ReviewerRequestStatus.Pending);
                        }
                    }
                    // Start internal clock on request re-assign
                    var clock = _context.Clocks.Single(p => p.RequestId == model.RequestId);
                    clock.InternalClockDate = DateTime.Now.AddDays(Convert.ToInt32(clock.InternalRemainingDays));
                    clock.InternalClockStatus = Convert.ToInt16(ClockStatus.Start);
                    clock.InternalRemainingDays = null;
                    _context.SaveChanges();

                    // Update Request Status to Assigned
                    request.Status = Convert.ToInt32(RequestStatus.Assigned);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    if (model.RequestAssignment.IsAccept)
                    {
                        var assignment = new RequestAssignment()
                        {
                            RequestId = model.RequestId ?? 0,
                            FederalOrState = model.RequestAssignment.FederalOrState,
                            ProjectNumber = model.RequestAssignment.ProjectNumber,
                            RespondDate = model.RequestAssignment.RespondDate,

                            IsAssignToArchitect = model.RequestAssignment.IsAssignToArchitect,
                            IsAssignToArchaelogical = model.RequestAssignment.IsAssignToArchaelogical,
                            IsAssignToTechnical = model.RequestAssignment.IsAssignToTechnical,
                            IsAssignToLandMarks = model.RequestAssignment.IsAssignToLandMarks,

                            ArchitechStatus = model.RequestAssignment.IsAssignToArchitect ? Convert.ToInt32(ReviewerRequestStatus.Pending) : 0,
                            ArchaelogicalStatus = model.RequestAssignment.IsAssignToArchaelogical ? Convert.ToInt32(ReviewerRequestStatus.Pending) : 0,
                            TechnicalStatus = model.RequestAssignment.IsAssignToTechnical ? Convert.ToInt32(ReviewerRequestStatus.Pending) : 0,
                            LandMarksStatus= model.RequestAssignment.IsAssignToLandMarks ? Convert.ToInt32(ReviewerRequestStatus.Pending) : 0
                        };
                        _context.RequestAssignments.Add(assignment);
                        _context.SaveChanges();

                        // Update request status based on IsAccept
                        request.Status = Convert.ToInt32(RequestStatus.Assigned);

                        // Start internal clock
                        var clock = _context.Clocks.Single(p => p.RequestId == model.RequestId);
                        clock.InternalClockDate = DateTime.Now.AddDays(15);
                        clock.InternalClockStatus = Convert.ToInt16(ClockStatus.Start);
                        _context.SaveChanges();
                    }
                    else
                    {
                        request.Status = Convert.ToInt32(RequestStatus.Returned);
                    }
                    _context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        public bool UpdateRequestAssignment(RequestVM model)
        {
            if (model.RequestId > 0)
            {
                var request = _context.Requests.Single(p => p.RequestId == model.RequestId);

                // If request already assign

                var requestAssignment = _context.RequestAssignments.Single(p => p.RequestId == model.RequestId);
                var requestReviewerResponse = _context.RequestResponses.SingleOrDefault(p => p.RequestId == model.RequestId);
                if (requestAssignment != null)
                {

                    if (!model.RequestAssignment.IsAssignToArchitect)
                    {
                        requestAssignment.IsAssignToArchitect = false;
                        requestAssignment.ArchitechStatus = 0;

                        // Update Architect Response
                        if (requestReviewerResponse != null)
                        {
                            requestReviewerResponse.IsNoHistoricProperty = null;
                            requestReviewerResponse.IsHistoricProperty = null;
                            //requestReviewerResponse.IsNoEffect = null;
                            //requestReviewerResponse.IsNoAdverseEffect = null;
                            //requestReviewerResponse.IsNoAdverseEffectWithCondition = null;

                            requestReviewerResponse.ArchitectResponse = null;
                            requestReviewerResponse.ArchitectComment = null;
                            requestReviewerResponse.ArchitectUserId = null;
                        }

                        // Update Architect Assignment Status


                    }
                    if (!model.RequestAssignment.IsAssignToArchaelogical)
                    {
                        requestAssignment.IsAssignToArchaelogical = false;
                        requestAssignment.ArchaelogicalStatus = 0;
                        // Update Archaelogical Response
                        if (requestReviewerResponse != null)
                        {
                            requestReviewerResponse.ArchaelogicalResponse = null;
                            requestReviewerResponse.ArchaelogicalComment = null;
                            requestReviewerResponse.ArchaelogicalUserId = null;
                        }

                    }
                    if (!model.RequestAssignment.IsAssignToTechnical)
                    {
                        requestAssignment.IsAssignToTechnical = false;
                        requestAssignment.TechnicalStatus = 0;
                        // Update Technical Response
                        if (requestReviewerResponse != null)
                        {
                            requestReviewerResponse.TechnicalResponse = null;
                            requestReviewerResponse.TechnicalComment = null;
                            requestReviewerResponse.TechnicalUserId = null;
                        }
                    }
                    if (!model.RequestAssignment.IsAssignToLandMarks)
                    {
                        requestAssignment.IsAssignToLandMarks = false;
                        requestAssignment.LandMarksStatus = 0;
                        // Update Technical Response
                        if (requestReviewerResponse != null)
                        {
                            requestReviewerResponse.LankMarksResponse = null;
                            requestReviewerResponse.LankMarksComment = null;
                            requestReviewerResponse.LankMarksUserId = null;
                        }
                    }
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool SaveAdminResponse(RequestVM model)
        {
            if (model.RequestId > 0)
            {
                var request = _context.Requests.Single(p => p.RequestId == model.RequestId);
                request.Status = model.Status;
                if (model.IsProjectCompleted == true)
                {
                    request.IsProjectCompleted = model.IsProjectCompleted;
                    request.ProjectCompleteddate = DateTime.Now;
                }
                _context.SaveChanges();

                //Stop clock if Admin returned the request
                if (model.Status == Convert.ToInt32(RequestStatus.Returned))
                {
                    var clock = _context.Clocks.Single(p => p.RequestId == model.RequestId);
                    var remainingDays = clock.ClockDate.Value.Date.Subtract(DateTime.Now.Date).TotalDays;
                    clock.ClockStatus = Convert.ToInt16(ClockStatus.Stop);
                    clock.RemainingDays = Convert.ToInt32(remainingDays);

                    // Stop internal clock if internal clock is in running state.
                    if (clock.InternalClockStatus != null && clock.InternalClockStatus == Convert.ToInt16(ClockStatus.Start))
                    {
                        clock.InternalClockStatus = Convert.ToInt16(ClockStatus.Stop);
                        var internalRemainingDays = clock.InternalClockDate.Value.Date.Subtract(DateTime.Now.Date).TotalDays;
                        clock.InternalRemainingDays = Convert.ToInt32(internalRemainingDays);
                    }
                    _context.SaveChanges();
                }

                return true;
            }
            return false;
        }

        //public Int64 SaveCorrespondence(CorrespondenceVM model, long RequestId)
        //{
        //    var correspondence = new Correspondence()
        //    {
        //        CorrespondenceTypeId = model.CorrespondenceTypeId,
        //        RequestId = RequestId,
        //        Body = model.Body,
        //        Date = DateTime.Now
        //    };
        //    _context.Correspondences.Add(correspondence);
        //    _context.SaveChanges();
        //    return correspondence.CorrespondenceId;
        //}

        //public bool UpdateCorrespondence(CorrespondenceVM model, long RequestId)
        //{
        //    if (model.CorrespondenceId > 0)
        //    {
        //        var updateCorrespondence = _context.Correspondences.Single(p => p.CorrespondenceId == model.CorrespondenceId);
        //        updateCorrespondence.CorrespondenceTypeId = model.CorrespondenceTypeId;
        //        updateCorrespondence.Date = DateTime.Now;
        //        updateCorrespondence.Body = model.Body;
        //        _context.SaveChanges();
        //        return true;
        //    }
        //    return false;
        //}

        //public bool DeleteCorrespondence(long correspondenceId)
        //{
        //    var correspondence = _context.Correspondences.FirstOrDefault(p => p.CorrespondenceId == correspondenceId);
        //    if (correspondence != null)
        //    {
        //        _context.Correspondences.Remove(correspondence);
        //        _context.SaveChanges();
        //    }
        //    return true;
        //}

        public bool ResetClock(long RequestId, string Reason)
        {
            var clock = _context.Clocks.Single(p => p.RequestId == RequestId);
            clock.ClockDate = DateTime.Now.Date.AddDays(30);
            clock.ClockStatus = Convert.ToInt16(ClockStatus.Stop);
            clock.RemainingDays = 30;
            clock.Reason = Reason;

            var request = _context.Requests.Single(p => p.RequestId == RequestId);
            request.Status = Convert.ToInt32(RequestStatus.Returned);
            _context.SaveChanges();
            return true;
        }

        #endregion "Requests"

        #region "Agency"

        public List<AgencyVM> GetAgencies()
        {
            var agencies = _context.Agencies;
            return agencies.Select(p => new AgencyVM()
            {
                AgencyId = p.AgencyId,
                AgencyTypeId = p.AgencyTypeId,
                AgencyTypeName = p.AgencyType.Description,
                Name = p.Name,
                Number = p.Number
            }).ToList();
        }

        public AgencyVM GetAgencyById(long agencyId)
        {
            if (agencyId != 0)
            {
                var agency = _context.Agencies.Single(p => p.AgencyId == agencyId);
                return new AgencyVM()
                {
                    AgencyId = agency.AgencyId,
                    AgencyTypeId = agency.AgencyTypeId,
                    AgencyTypeName = agency.AgencyType.Description,
                    Name = agency.Name,
                    Number = agency.Number
                };
            }
            return new AgencyVM();
        }

        public bool SaveAgency(AgencyVM model)
        {
            var agency = new Agency()
            {
                AgencyTypeId = model.AgencyTypeId,
                Name = model.Name,
                Number = model.Number
            };
            _context.Agencies.Add(agency);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateAgency(AgencyVM model)
        {
            if (model.AgencyId != 0)
            {
                var agency = _context.Agencies.Single(p => p.AgencyId == model.AgencyId);
                agency.Name = model.Name;
                agency.Number = model.Number;
                agency.AgencyTypeId = model.AgencyTypeId;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public int DeleteAgency(long agencyId)
        {
            var useCount = _context.Requests.Where(p => p.AgencyId == agencyId).Count();
            if (useCount != 0)
            {
                return useCount;
            }
            else
            {
                var agency = _context.Agencies.Single(p => p.AgencyId == agencyId);
                _context.Agencies.Remove(agency);
                _context.SaveChanges();
                return 0;
            }

            //if (agencyId != 0)
            //{
            //    var agency = _context.Agencies.Single(p => p.AgencyId == agencyId);
            //    _context.Agencies.Remove(agency);
            //    _context.SaveChanges();
            //    return true;
            //}
            //return false;
        }


        #endregion "Agency"

        #region "Agency Types"

        public List<AgencyTypeVM> GetAgencyTypes()
        {
            var agencyTypes = _context.AgencyTypes;
            return agencyTypes.Select(p => new AgencyTypeVM()
            {
                AgencyTypeId = p.AgencyTypeId,
                Description = p.Description
            }).ToList();
        }

        public AgencyTypeVM GetAgencyTypeById(long agencyTypeId)
        {
            if (agencyTypeId != 0)
            {
                var agencyType = _context.AgencyTypes.Single(p => p.AgencyTypeId == agencyTypeId);
                return new AgencyTypeVM()
                {
                    AgencyTypeId = agencyType.AgencyTypeId,
                    Description = agencyType.Description
                };
            }
            return new AgencyTypeVM();
        }

        public bool SaveAgencyType(AgencyTypeVM model)
        {
            var agencyType = new AgencyType()
            {
                Description = model.Description
            };
            _context.AgencyTypes.Add(agencyType);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateAgencyType(AgencyTypeVM model)
        {
            if (model.AgencyTypeId != 0)
            {
                var agencyType = _context.AgencyTypes.Single(p => p.AgencyTypeId == model.AgencyTypeId);
                agencyType.Description = model.Description;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public int DeleteAgencyType(long agencyTypeId)
        {
            var useCount = _context.Agencies.Where(p => p.AgencyTypeId == agencyTypeId).Count();
            if (useCount != 0)
            {
                return useCount;
            }
            else
            {
                var agencyType = _context.AgencyTypes.Single(p => p.AgencyTypeId == agencyTypeId);
                _context.AgencyTypes.Remove(agencyType);
                _context.SaveChanges();
                return 0;
            }
        }


        #endregion "Agency Types"

        #region "Correspondence Types"

        public List<CorrespondenceTypeVM> GetCorrespondenceTypes()
        {
            var correspondenceTypes = _context.CorrespondenceTypes;
            return correspondenceTypes.Select(p => new CorrespondenceTypeVM()
            {
                CorrespondenceTypeId = p.CorrespondenceTypeId,
                Description = p.Description
            }).ToList();
        }

        public CorrespondenceTypeVM GetCorrespondenceTypeById(long correspondenceTypeId)
        {
            if (correspondenceTypeId != 0)
            {
                var correspondenceType = _context.CorrespondenceTypes.Single(p => p.CorrespondenceTypeId == correspondenceTypeId);
                return new CorrespondenceTypeVM()
                {
                    CorrespondenceTypeId = correspondenceType.CorrespondenceTypeId,
                    Description = correspondenceType.Description,
                    Template = correspondenceType.Template
                };
            }
            return new CorrespondenceTypeVM();
        }

        public bool SaveCorrespondenceType(CorrespondenceTypeVM model)
        {
            var correspondenceType = new CorrespondenceType()
            {
                Description = model.Description,
                Template = model.Template
            };
            _context.CorrespondenceTypes.Add(correspondenceType);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateCorrespondenceType(CorrespondenceTypeVM model)
        {
            if (model.CorrespondenceTypeId != 0)
            {
                var correspondenceType = _context.CorrespondenceTypes.Single(p => p.CorrespondenceTypeId == model.CorrespondenceTypeId);
                correspondenceType.Description = model.Description;
                correspondenceType.Template = model.Template;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public int DeleteCorrespondenceType(long correspondenceTypeId)
        {
            //if (correspondenceTypeId != 0)
            //{
            //    var correspondenceType = _context.CorrespondenceTypes.Single(p => p.CorrespondenceTypeId == correspondenceTypeId);
            //    _context.CorrespondenceTypes.Remove(correspondenceType);
            //    _context.SaveChanges();
            //    return true;
            //}
            //return false;

            var useCount = _context.Correspondences.Where(p => p.CorrespondenceTypeId == correspondenceTypeId).Count();
            if (useCount != 0)
            {
                return useCount;
            }
            else
            {
                var correspondenceType = _context.CorrespondenceTypes.Single(p => p.CorrespondenceTypeId == correspondenceTypeId);
                _context.CorrespondenceTypes.Remove(correspondenceType);
                _context.SaveChanges();
                return 0;
            }
        }

        #endregion "Correspondence Types"

        #region "Reviewers"

        public List<ContactVM> GetReviewers()
        {
            var reviewersVM = new List<ContactVM>();
            var reviewersRoleIds = _context.AspNetRoles.Where(p => p.Name != "Admin" && p.Name != "Submitter").Select(p => p.Id).ToList();
            var reviewerUserIds = _context.AspNetUsers.Where(p => p.AspNetRoles.Any(q => reviewersRoleIds.Contains(q.Id))).Select(p => p.Id);
            //var reviewerUserIds = _context.AspNetUsers.Where(p => reviewersRoleIds.Contains(p.AspNetRoles.Select(q => q.Id).ToString())).Select(p => p.Id).ToList();

            var reviewers = _context.Entities.Where(p => reviewerUserIds.Contains(p.UserId)).ToList();
            reviewersVM = reviewers.Select(q => new ContactVM()
            {
                Id = q.EntityId,
                UserId = q.UserId,
                Name = q.Name,
                UserName = q.AspNetUser.UserName,
                Company = q.Company,
                Address1 = q.Address1,
                Address2 = q.Address2,
                StateId = q.StateId,
                City = q.City,
                Zip = q.Zip,
                MobileNumber = q.MobilePhone,
                OfficeNumber = q.OfficePhone,
                Fax = q.Fax,
                StateName = q.State.Name,
                Role = _commonRepository.GetUserRoleByUserName(q.AspNetUser.UserName)
            }).ToList();

            return reviewersVM;
        }

        #endregion "Reviewers"



        #region "Submitters"
        public List<ContactVM> GetSubmittersBySearch(string Prefix)
        {
            var submitterVM = new List<ContactVM>();
            var submitterRoleIds = _context.AspNetRoles.Where(p => p.Name == "Submitter").Select(p => p.Id).ToList();
            var submitterUserIds = _context.AspNetUsers.Where(p => p.AspNetRoles.Any(q => submitterRoleIds.Contains(q.Id))).Select(p => p.Id);
            //var reviewerUserIds = _context.AspNetUsers.Where(p => reviewersRoleIds.Contains(p.AspNetRoles.Select(q => q.Id).ToString())).Select(p => p.Id).ToList();

            var submitters = _context.Entities.Where(p => submitterUserIds.Contains(p.UserId)).ToList();
            submitterVM = submitters.Where(p => p.Name.StartsWith(Prefix)).Select(q => new ContactVM()
            {
                //Id = q.EntityId,
                UserId = q.UserId,
                Name = q.Name,
                //UserName = q.AspNetUser.UserName,
                //Company = q.Company,
                //Address1 = q.Address1,
                //Address2 = q.Address2,
                //StateId = q.StateId,
                //City = q.City,
                //Zip = q.Zip,
                //MobileNumber = q.MobilePhone,
                //OfficeNumber = q.OfficePhone,
                //Fax = q.Fax,
                //StateName = q.State.Name,
                //Role = _commonRepository.GetUserRoleByUserName(q.AspNetUser.UserName)
            }).ToList();

            return submitterVM;
        }
        #endregion "Submitters"

        public List<ContactVM> GetAccountUsers(int type)
        {
            var UsersVM = new List<ContactVM>();
            List<string> RoleIds = new List<string>();
            if (type == 1)
            {
                RoleIds = _context.AspNetRoles.Where(p => p.Name == "Submitter").Select(p => p.Id).ToList();
            }
            else
            {
                RoleIds = _context.AspNetRoles.Where(p => p.Name != "Submitter" && p.Name != "Admin").Select(p => p.Id).ToList();
            }
            var UserIds = _context.AspNetUsers.Where(p => p.AspNetRoles.Any(q => RoleIds.Contains(q.Id))).Select(p => p.Id);
            //var reviewerUserIds = _context.AspNetUsers.Where(p => reviewersRoleIds.Contains(p.AspNetRoles.Select(q => q.Id).ToString())).Select(p => p.Id).ToList();

            var Users = _context.Entities.Where(p => UserIds.Contains(p.UserId)).ToList();
            UsersVM = Users.Select(q => new ContactVM()
            {
                Id = q.EntityId,
                UserId = q.UserId,
                Name = q.Name,
                UserName = q.AspNetUser.UserName,
                Company = q.Company,
                Address1 = q.Address1,
                Address2 = q.Address2,
                StateId = q.StateId,
                City = q.City,
                Zip = q.Zip,
                MobileNumber = q.MobilePhone,
                OfficeNumber = q.OfficePhone,
                Fax = q.Fax,
                StateName = q.State.Name,
                Role = _commonRepository.GetUserRoleByUserName(q.AspNetUser.UserName)
            }).ToList();

            return UsersVM;
        }


        public List<ReportVM> GetReportData(DateTime? startDate, DateTime? endDate)
        {
            var ReportVM = new List<ReportVM>();
            List<long> RequestIds = new List<long>();

            RequestIds = _context.Requests.Where(p => p.IsProjectCompleted == true ||( p.ProjectCompleteddate >= startDate && p.ProjectCompleteddate <= endDate)).Select(p => p.RequestId).ToList();


            //var reviewerUserIds = _context.AspNetUsers.Where(p => reviewersRoleIds.Contains(p.AspNetRoles.Select(q => q.Id).ToString())).Select(p => p.Id).ToList();

            var RequestResponses = _context.RequestResponses.Where(p => RequestIds.Contains(p.RequestId)).ToList();
            ReportVM = RequestResponses.Select(q => new ReportVM()
            {
                Id = q.RequestId,
                EligibleProperties = Add(q.ArchaelogicalEligibleProperties ?? 0, q.ArchitectEligibleProperties ?? 0),
                InEligibleProperties = Add(q.ArchaelogicalInEligibleProperties ?? 0, q.ArchitectInEligibleProperties ?? 0),
                Unknown = Add(q.ArchaelogicalUnknownProperties ?? 0, q.ArchitectUnknownProperties ?? 0),
                Request = q.Request.Name,

            }).ToList();

            return ReportVM;
        }

        private int Add(int A = 0, int B = 0)
        {
            return (A + B);
        }
    }
}
