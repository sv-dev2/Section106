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
    public interface IRequestReviewRepository
    {
        List<RequestVM> GetReviewerRequestsByRole(List<int> status, string userRole);
        bool SaveReviewerResponse(RequestVM model, bool IsReqAddInfo, string userId, string userRole);
    }
}
