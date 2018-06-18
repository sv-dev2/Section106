using Section106.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Service.IService
{
    public interface ICommonService
    {
        List<DictionaryVM> GetStates();
        List<DictionaryVM> GetCities();
        List<DictionaryVM> GetCounties();
        List<DictionaryVM> GetAgencies();
        string GetUserIdByUserName(string userName);
        string GetUserRoleByUserName(string userName);
        List<DictionaryVM> GetAdminRequestStatus();
        List<DictionaryVM> GetAgencyTypes();
        List<DictionaryVM> GetCorrespondenceTypes();
        string GetCorrespondenceTypeNameById(long Id);
        Task<string> SendEmail(string body, string subject, string to);
        bool checkDuplicateEmail(string email);
        ContactVM GetApplicantDetail(long id);
        Int64 SaveCorrespondence(CorrespondenceVM model, long RequestId);
        bool UpdateCorrespondence(CorrespondenceVM model, long RequestId);
        bool DeleteCorrespondence(long correspondenceId);
    }
}
