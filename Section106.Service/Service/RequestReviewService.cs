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
    public class RequestReviewService : IRequestReviewService
    {
        private IRequestReviewRepository _requestReviewRepository;

        public RequestReviewService(IRequestReviewRepository RequestReviewRepository)
        {
            _requestReviewRepository = RequestReviewRepository;
        }

        public List<RequestVM> GetReviewerRequestsByRole(List<int> status, string userRole)
        {
            return _requestReviewRepository.GetReviewerRequestsByRole(status, userRole);
        }

        public bool SaveReviewerResponse(RequestVM model,bool IsReqAddInfo, string userId, string userRole)
        {
            return _requestReviewRepository.SaveReviewerResponse(model, IsReqAddInfo, userId, userRole);
        }

    }
}
