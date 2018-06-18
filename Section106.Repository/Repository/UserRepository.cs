using Section106.Models.Models;
using Section106.Repository.DataBase;
using Section106.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Section106.Models;
using System.Data.Entity.Validation;

namespace Section106.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        private Section106Entities _context;
        public UserRepository(Section106Entities Context)
        {
            _context = Context;
        }

        public bool CreateUser(RegisterViewModel model, string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var entity = new Entity()
                {
                    UserId = userId,
                    Name = model.Name,
                    Company = model.Company,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    StateId = model.StateId,
                    City = model.City,
                    Zip = model.Zip,
                    Fax = model.Fax,
                    MobilePhone = model.MobilePhone,
                    OfficePhone = model.OfficePhone,
                    ContactedBy=model.ContactedBy                    
                };
                _context.Entities.Add(entity);
                _context.SaveChanges();

                return true;
            }
            return false;
        }

        public ContactVM GetSubmittingContactByUserId(string userId)
        {
            var contact = _context.Entities.Single(p => p.UserId == userId);
            var user = _context.AspNetUsers.Single(p => p.Id == userId);
            return new ContactVM()
            {
                Id = contact.EntityId,
                UserId = contact.UserId,
                Name = contact.Name,
                UserName = user.UserName,
                Company = contact.Company,
                Address1 = contact.Address1,
                Address2 = contact.Address2,
                StateId = contact.StateId,
                City = contact.City,
                Email = user.Email,
                Zip = contact.Zip,
                MobileNumber = contact.MobilePhone,
                OfficeNumber = contact.OfficePhone,
                Fax = contact.Fax,
                StateName = contact.State.Name,
                CountyName=contact?.County?.Name,
                CountyId=contact?.CountyId
            };
        }

        public bool UpdateUser(ContactVM model)
        {
            try { 
            if (model.Id > 0)
            {
                var entity = _context.Entities.Single(p => p.EntityId == model.Id);
                entity.Name = model.Name;
                entity.Company = model.Company;
                entity.Address1 = model.Address1;
                entity.Address2 = model.Address2;
                entity.StateId = model.StateId;
                entity.City = model.City;
                entity.Zip = model.Zip;
                entity.Fax = model.Fax;
                entity.MobilePhone = model.MobileNumber;
                entity.OfficePhone = model.OfficeNumber;

                var user = _context.AspNetUsers.Single(p => p.Id == model.UserId);
                user.Email = model.Email;

                _context.SaveChanges();
                return true;
            }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return false;
        }
    }
}
