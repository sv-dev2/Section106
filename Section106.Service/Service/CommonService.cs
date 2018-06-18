using Section106.Models.Models;
using Section106.Repository.IRepository;
using Section106.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Service.Service
{
    public class CommonService : ICommonService 
    {
        private ICommonRepository _commonRepository;

        public CommonService(ICommonRepository CommonRepository)
        {
            _commonRepository = CommonRepository;
        }


        public List<DictionaryVM> GetStates()
        {
            return _commonRepository.GetStates();
        }

        public List<DictionaryVM> GetCities()
        {
            return _commonRepository.GetCities();
        }

        public List<DictionaryVM> GetCounties()
        {
            return _commonRepository.GetCounties();
        }

        public List<DictionaryVM> GetAgencies()
        {
            return _commonRepository.GetAgencies();
        }

        public string GetUserIdByUserName(string userName)
        {
            return _commonRepository.GetUserIdByUserName(userName);
        }

        public List<DictionaryVM> GetAdminRequestStatus()
        {
            return _commonRepository.GetAdminRequestStatus();
        }

        public List<DictionaryVM> GetAgencyTypes()
        {
            return _commonRepository.GetAgencyTypes();
        }

        public List<DictionaryVM> GetCorrespondenceTypes()
        {
            return _commonRepository.GetCorrespondenceTypes();
        }

        public string GetUserRoleByUserName(string userName)
        {
            return _commonRepository.GetUserRoleByUserName(userName);
        }

        public string GetCorrespondenceTypeNameById(long Id)
        {
            return _commonRepository.GetCorrespondenceTypeNameById(Id);
        }

        public async Task<string> SendEmail(string body, string subject, string to)
        {
            return await _commonRepository.SendEmail(body, subject, to);
        }

        public bool checkDuplicateEmail(string email)
        {
            //ICommonRepository _commonRepository;
            return _commonRepository.checkDuplicateEmail(email);
        }
        public ContactVM GetApplicantDetail(long id)
        {
            return _commonRepository.GetApplicantDetail(id);
        }
        public Int64 SaveCorrespondence(CorrespondenceVM model, long RequestId)
        {
            return _commonRepository.SaveCorrespondence(model, RequestId);
        }

        public bool UpdateCorrespondence(CorrespondenceVM model, long RequestId)
        {
            return _commonRepository.UpdateCorrespondence(model, RequestId);
        }

        public bool DeleteCorrespondence(long correspondenceId)
        {
            return _commonRepository.DeleteCorrespondence(correspondenceId);
        }
    }

}
