using Section106.Models.Models;
using Section106.Repository.IRepository;
using Section106.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Section106.Models;
using Section106.Models.Enums;

namespace Section106.Service.Service
{
    public class AdminService : IAdminService
    {
        private IAdminRepository _adminRepository;

        public AdminService(IAdminRepository AdminRepository)
        {
            _adminRepository = AdminRepository;
        }

        public bool SaveRequestAssignment(RequestVM model)
        {
            return _adminRepository.SaveRequestAssignment(model);
        }
        public bool UpdateRequestAssignment(RequestVM model)
        {
            return _adminRepository.UpdateRequestAssignment(model);
        }

        public bool SaveAdminResponse(RequestVM model)
        {
            return _adminRepository.SaveAdminResponse(model);
        }

        //public Int64 SaveCorrespondence(CorrespondenceVM model, long RequestId)
        //{
        //    return _adminRepository.SaveCorrespondence(model, RequestId);
        //}

        //public bool UpdateCorrespondence(CorrespondenceVM model, long RequestId)
        //{
        //    return _adminRepository.UpdateCorrespondence(model, RequestId);
        //}

        //public bool DeleteCorrespondence(long correspondenceId)
        //{
        //    return _adminRepository.DeleteCorrespondence(correspondenceId);
        //}

        public bool ResetClock(long RequestId, string Reason)
        {
            return _adminRepository.ResetClock(RequestId, Reason);
        }

        #region "Requests"

        public List<RequestVM> GetUserRequestsByStatus(List<int> status)
        {
            return _adminRepository.GetUserRequestsByStatus(status);
        }

        #endregion "Requests"

        #region "Agency"

        public List<AgencyVM> GetAgencies()
        {
            return _adminRepository.GetAgencies();
        }
        public AgencyVM GetAgencyById(long agencyId)
        {
            return _adminRepository.GetAgencyById(agencyId);
        }

        public bool SaveAgency(AgencyVM model)
        {
            return _adminRepository.SaveAgency(model);
        }

        public bool UpdateAgency(AgencyVM model)
        {
            return _adminRepository.UpdateAgency(model);
        }

        public int DeleteAgency(long agencyId)
        {
            return _adminRepository.DeleteAgency(agencyId);
        }


        #endregion "Agency"

        #region "Agency Types"

        public List<AgencyTypeVM> GetAgencyTypes()
        {
            return _adminRepository.GetAgencyTypes();
        }
        public AgencyTypeVM GetAgencyTypeById(long agencyTypeId)
        {
            return _adminRepository.GetAgencyTypeById(agencyTypeId);
        }

        public bool SaveAgencyType(AgencyTypeVM model)
        {
            return _adminRepository.SaveAgencyType(model);
        }

        public bool UpdateAgencyType(AgencyTypeVM model)
        {
            return _adminRepository.UpdateAgencyType(model);
        }

        public int DeleteAgencyType(long agencyTypeId)
        {
            return _adminRepository.DeleteAgencyType(agencyTypeId);
        }


        #endregion "Agency Types"

        #region "Correspondence Types"

        public List<CorrespondenceTypeVM> GetCorrespondenceTypes()
        {
            return _adminRepository.GetCorrespondenceTypes();
        }
        public CorrespondenceTypeVM GetCorrespondenceTypeById(long correspondenceTypeId)
        {
            return _adminRepository.GetCorrespondenceTypeById(correspondenceTypeId);
        }

        public bool SaveCorrespondenceType(CorrespondenceTypeVM model)
        {
            return _adminRepository.SaveCorrespondenceType(model);
        }

        public bool UpdateCorrespondenceType(CorrespondenceTypeVM model)
        {
            return _adminRepository.UpdateCorrespondenceType(model);
        }

        public int DeleteCorrespondenceType(long correspondenceTypeId)
        {
            return _adminRepository.DeleteCorrespondenceType(correspondenceTypeId);
        }


        #endregion "Correspondence Types"

        #region "Reviewers"

        public List<ContactVM> GetReviewers()
        {
            return _adminRepository.GetReviewers();
        }



        #endregion "Reviewers"

        #region "Submitters"
        public List<ContactVM> GetSubmittersBySearch(string Prefix)
        {
            return _adminRepository.GetSubmittersBySearch(Prefix);
        }

        #endregion "Submitters"
        public List<ContactVM> GetAccountUsers(int type)
        {
            return _adminRepository.GetAccountUsers(type);
        }

        public List<ReportVM> GetReportData(DateTime? startDate, DateTime? endDate)
        {
            return _adminRepository.GetReportData(startDate,endDate);
        }
    }
}
