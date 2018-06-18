// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Section106.DependencyResolution {
    using Microsoft.AspNet.Identity;
    using Repository.DataBase;
    using Repository.IRepository;
    using Repository.Repository;
    using Section106.Models;
    using Service.IService;
    using Service.Service;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
    using System.Web;

    public class DefaultRegistry : Registry {
        #region Constructors and Destructors
        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
            For<System.Data.Entity.DbContext>().Use(() => new Section106Entities());

            For<ICommonRepository>().Use<CommonRepository>();
            For<ICommonService>().Use<CommonService>();
            For<IUserRepository>().Use<UserRepository>();
            For<IUserService>().Use<UserService>();
            For<IRequestRepository>().Use<RequestRepository>();
            For<IRequestService>().Use<RequestService>();
            For<IAdminRepository>().Use<AdminRepository>();
            For<IAdminService>().Use<AdminService>();
            For<IRequestReviewRepository>().Use<RequestReviewRepository>();
            For<IRequestReviewService>().Use<RequestReviewService>();

            For<ApplicationUserManager>().Use<ApplicationUserManager>();
            For<ApplicationSignInManager>().Use<ApplicationSignInManager>();
            For<Microsoft.AspNet.Identity.IUserStore<ApplicationUser>>().Use<Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>>();
            For<System.Data.Entity.DbContext>().Use(() => new ApplicationDbContext());
            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            For<Microsoft.Owin.Security.IAuthenticationManager>().Use(() => new HttpContextWrapper(HttpContext.Current).GetOwinContext().Authentication);
            //For<IRegistrationRepository>().Use<RegistrationRepository>();
        }

        #endregion
    }
}