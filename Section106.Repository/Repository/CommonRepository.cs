using Section106.Models.Enums;
using Section106.Models.Models;
using Section106.Repository.DataBase;
using Section106.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace Section106.Repository.Repository
{
    public class CommonRepository : ICommonRepository
    {
        private Section106Entities _context;
        public CommonRepository(Section106Entities Context)
        {
            _context = Context;
        }

        public List<DictionaryVM> GetStates()
        {
            return _context.States.Select(p => new DictionaryVM()
            {
                Value = p.StateId,
                Text = p.Abbr + "-" + p.Name
            }).ToList();
        }

        public List<DictionaryVM> GetCities()
        {
            return _context.Cities.Select(p => new DictionaryVM()
            {
                Value = p.CityId,
                Text = p.Name
            }).ToList();
        }

        public List<DictionaryVM> GetCounties()
        {
            return _context.Counties.Select(p => new DictionaryVM()
            {
                Value = p.CountyId,
                Text = p.Name
            }).ToList();
        }

        public List<DictionaryVM> GetAgencies()
        {
            return _context.Agencies.Select(p => new DictionaryVM()
            {
                Value = p.AgencyId,
                Text = p.Name
            }).ToList();
        }

        public List<DictionaryVM> GetAgencyTypes()
        {
            return _context.AgencyTypes.Select(p => new DictionaryVM()
            {
                Value = p.AgencyTypeId,
                Text = p.Description
            }).ToList();
        }

        public List<DictionaryVM> GetCorrespondenceTypes()
        {
            return _context.CorrespondenceTypes.Select(p => new DictionaryVM()
            {
                Value = p.CorrespondenceTypeId,
                Text = p.Description
            }).ToList();
        }

        public List<DictionaryVM> GetAdminRequestStatus()
        {
            var requestAdminStatus = Enum.GetNames(typeof(RequestStatus)).Where(p => p != Enum.GetName(typeof(RequestStatus), RequestStatus.Saved)).Select(p => new DictionaryVM()
            {
                Text = p,
                Value = (int)Enum.Parse(typeof(RequestStatus), p)
            }).ToList();

            return requestAdminStatus;
        }

        public string GetUserIdByUserName(string userName)
        {
            return _context.AspNetUsers.Single(p => p.UserName == userName).Id;
        }

        public string GenerateRandomNumber()
        {
            //string allowedChars = "abcdefghijkmnopqrstuvwxyz";
            //char[] Smallchars = new char[3];
            Random rd = new Random();     
            string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            char[] Capschars = new char[3];
            rd = new Random();

            for (int i = 0; i < 3; i++)
            {
                Capschars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }


            //allowedChars = "@$";
            //char[] Symbolchars = new char[1];
            //rd = new Random();

            //for (int i = 0; i < 1; i++)
            //{
            //    Symbolchars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            //}


            allowedChars = "1234567890";
            char[] Numberschars = new char[6];
            rd = new Random();

            for (int i = 0; i < 6; i++)
            {
                Numberschars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            string RandomNumber = "P" + new string(Capschars) + new string(Numberschars);
            //string RandomNumber = new string(Smallchars) + new string(Capschars) + new string(Symbolchars) + new string(Numberschars);
            RandomNumber = RandomNumber.Substring(0, 10);
            return (RandomNumber);
        }

        public string GetUserRoleByUserName(string userName)
        {
            var user = _context.AspNetUsers.Single(p => p.UserName == userName);
            var userRole = user.AspNetRoles.FirstOrDefault();
            if (userRole != null)
            {
                return userRole.Name;
            }
            return string.Empty;
        }

        public string GetCorrespondenceTypeNameById(long Id)
        {
            if (Id > 0)
            {
                return _context.CorrespondenceTypes.Single(p => p.CorrespondenceTypeId == Id).Description;
            }
            return string.Empty;
        }

        public async Task<string> SendEmail(string body, string subject, string to)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
                client.Host = ConfigurationManager.AppSettings["Host"].ToString();
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailFrom"].ToString(), ConfigurationManager.AppSettings["EmailPassword"].ToString());

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"].ToString());
                mail.To.Add(new MailAddress(to));
                mail.Subject = subject; //"Reset Password";
                mail.Body = body;
                mail.IsBodyHtml = true;

                await client.SendMailAsync(mail);

                return "Success";
            }
            catch (Exception ex)
            {
                return "Error sending Email.";
            }
        }
        
        public bool checkDuplicateEmail(string email)
        {
            return _context.AspNetUsers.Any(p => p.Email == email);
            //if (user != null && user.Count() > 0)
            //{
            //    return true;
            //}
            //return false;
        }

        public ContactVM GetApplicantDetail(long id)
        {
            var applicant = _context.Applicants.Where(p => p.ApplicantId == id).FirstOrDefault();
            ContactVM applicantDetail = new ContactVM();
            if(applicant!=null)
            {
                applicantDetail.Name = applicant.Name;
                applicantDetail.CountyName = applicant.County.Name;
            }
            return applicantDetail;
        }

        public Int64 SaveCorrespondence(CorrespondenceVM model, long RequestId)
        {
            var correspondence = new Correspondence()
            {
                CorrespondenceTypeId = model.CorrespondenceTypeId,
                RequestId = RequestId,
                Body = model.Body,
                Date = DateTime.Now
            };
            _context.Correspondences.Add(correspondence);
            _context.SaveChanges();
            return correspondence.CorrespondenceId;
        }

        public bool UpdateCorrespondence(CorrespondenceVM model, long RequestId)
        {
            if (model.CorrespondenceId > 0)
            {
                var updateCorrespondence = _context.Correspondences.Single(p => p.CorrespondenceId == model.CorrespondenceId);
                updateCorrespondence.CorrespondenceTypeId = model.CorrespondenceTypeId;
                updateCorrespondence.Date = DateTime.Now;
                updateCorrespondence.Body = model.Body;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteCorrespondence(long correspondenceId)
        {
            var correspondence = _context.Correspondences.FirstOrDefault(p => p.CorrespondenceId == correspondenceId);
            if (correspondence != null)
            {
                _context.Correspondences.Remove(correspondence);
                _context.SaveChanges();
            }
            return true;
        }
    }
}
