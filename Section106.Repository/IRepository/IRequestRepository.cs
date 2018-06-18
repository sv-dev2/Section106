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
    public interface IRequestRepository
    {
        List<RequestVM> GetUserRequestsByStatus(string userId, List<int> status);
        bool SaveRequest(RequestVM model);
        bool UpdateRequest(RequestVM model);
        RequestVM GetRequestByRequestId(long requestId);
        ContactVM GetApplicantContactById(Int64 Id);
        bool DeleteUserRequestByRequestId(Int64 RequestId);
        bool DeleteAttachment(long Id);
        string GetRemainingDays(long RequestId);
        List<CoverSheetVM> GetCoverSheetUsers();
    }
}
