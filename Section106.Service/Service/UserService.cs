using Section106.Models.Models;
using Section106.Repository.IRepository;
using Section106.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Section106.Models;

namespace Section106.Service.Service
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository UserRepository)
        {
            _userRepository = UserRepository;
        }

        public bool CreateUser(RegisterViewModel model, string userId)
        {
            return _userRepository.CreateUser(model, userId);
        }

        public ContactVM GetSubmittingContactByUserId(string userId)
        {
            return _userRepository.GetSubmittingContactByUserId(userId);
        }

        public bool UpdateUser(ContactVM model)
        {
            return _userRepository.UpdateUser(model);
        }
    }
}
