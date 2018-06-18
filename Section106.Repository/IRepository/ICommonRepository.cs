using Section106.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Repository.IRepository
{
    public interface ICommonRepository
    {
        List<DictionaryVM> GetStates();
        List<DictionaryVM> GetCities();
        List<DictionaryVM> GetCounties();
        List<DictionaryVM> GetAgencies();
        string GetUserIdByUserName(string userName);
        string GenerateRandomNumber();
        List<DictionaryVM> GetAdminRequestStatus();
        List<DictionaryVM> GetAgencyTypes();
        List<DictionaryVM> GetCorrespondenceTypes();
        string GetUserRoleByUserName(string userName);
        string GetCorrespondenceTypeNameById(long Id);

        Task<string> SendEmail(string body, string subject, string to);

        bool checkDuplicateEmail(string email);
        ContactVM GetApplicantDetail(long id);

        Int64 SaveCorrespondence(CorrespondenceVM model, long RequestId);
        bool UpdateCorrespondence(CorrespondenceVM model, long RequestId);
        bool DeleteCorrespondence(long correspondenceId);
    }

}
