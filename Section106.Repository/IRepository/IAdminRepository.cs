using Section106.Models;
using Section106.Models.Enums;
using Section106.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Repository.IRepository
{
    public interface IAdminRepository
    {
        #region "Requests"

        List<RequestVM> GetUserRequestsByStatus(List<int> statusList);
        bool SaveRequestAssignment(RequestVM model);
        bool UpdateRequestAssignment(RequestVM model);
        bool SaveAdminResponse(RequestVM model);
        //Int64 SaveCorrespondence(CorrespondenceVM model, long RequestId);
        //bool UpdateCorrespondence(CorrespondenceVM model, long RequestId);
        //bool DeleteCorrespondence(long correspondenceId);
        bool ResetClock(long RequestId, string Reason);

        #endregion "Requests"


        #region "Agency"

        List<AgencyVM> GetAgencies();
        AgencyVM GetAgencyById(long agencyId);
        bool SaveAgency(AgencyVM model);
        bool UpdateAgency(AgencyVM model);
        int DeleteAgency(long agencyId);

        #endregion "Agency"


        #region "Agency Types"

        List<AgencyTypeVM> GetAgencyTypes();
        AgencyTypeVM GetAgencyTypeById(long agencyTypeId);
        bool SaveAgencyType(AgencyTypeVM model);
        bool UpdateAgencyType(AgencyTypeVM model);
        int DeleteAgencyType(long agencyTypeId);

        #endregion "Agency Types"


        #region "Correspondence Type"

        List<CorrespondenceTypeVM> GetCorrespondenceTypes();
        CorrespondenceTypeVM GetCorrespondenceTypeById(long correspondenceTypeId);
        bool SaveCorrespondenceType(CorrespondenceTypeVM model);
        bool UpdateCorrespondenceType(CorrespondenceTypeVM model);
        int DeleteCorrespondenceType(long correspondenceTypeId);

        #endregion "Correspondence Type"


        #region "Reviewers"

        List<ContactVM> GetReviewers();

        string GetInternalRemainingDays(long RequestId);

        #endregion "Reviewers"

        #region "Submitters"
        List<ContactVM> GetSubmittersBySearch(string Prefix);
        #endregion "Submitters"
        List<ContactVM> GetAccountUsers(int type);
        List<ReportVM> GetReportData(DateTime? startDate, DateTime? endDate);
    }
}
