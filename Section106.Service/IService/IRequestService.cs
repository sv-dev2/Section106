using Section106.Models;
using Section106.Models.Enums;
using Section106.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Service.IService
{
    public interface IRequestService
    {
        List<RequestVM> GetUserRequestsByStatus(string userId, List<int> status);
        bool SaveRequest(RequestVM model);
        bool UpdateRequest(RequestVM model);
        RequestVM GetRequestByRequestId(long requestId);
        bool DeleteUserRequestByRequestId(Int64 RequestId);
        bool DeleteAttachment(long Id);
        List<CoverSheetVM> GetCoverSheetUsers();
    }
}
