using Section106.Models;
using Section106.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Section106.Service.IService
{
    public interface IUserService
    {
        ContactVM GetSubmittingContactByUserId(string userId);
        bool CreateUser(RegisterViewModel model, string userId);
        bool UpdateUser(ContactVM model);
    }
}
