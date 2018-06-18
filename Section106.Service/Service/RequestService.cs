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
    public class RequestService : IRequestService
    {
        private IRequestRepository _requestRepository;

        public RequestService(IRequestRepository RequestRepository)
        {
            _requestRepository = RequestRepository;
        }

        public List<RequestVM> GetUserRequestsByStatus(string userId, List<int> status)
        {
            return _requestRepository.GetUserRequestsByStatus(userId, status);
        }

        public bool SaveRequest(RequestVM model)
        {
            return _requestRepository.SaveRequest(model);
        }

        public bool UpdateRequest(RequestVM model)
        {
            return _requestRepository.UpdateRequest(model);
        }

        public RequestVM GetRequestByRequestId(long requestId)
        {
            return _requestRepository.GetRequestByRequestId(requestId);
        }

        public bool DeleteUserRequestByRequestId(long RequestId)
        {
            return _requestRepository.DeleteUserRequestByRequestId(RequestId);
        }

        public bool DeleteAttachment(long Id)
        {
            return _requestRepository.DeleteAttachment(Id);
        }
        public List<CoverSheetVM> GetCoverSheetUsers()
        {
            return _requestRepository.GetCoverSheetUsers();
        }
    }
}
