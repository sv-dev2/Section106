using Section106.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Section106.Models
{
    public class CommonFunc
    {
        private ICommonService _commonService;

        public CommonFunc(ICommonService CommonService)
        {
            _commonService = CommonService;
        }

        public SelectList GetStates()
        {
            return new SelectList(_commonService.GetStates(), "StateId", "Name");
            //_commonService.GetStates().Select(p => new SelectListItem()
            //{

            //});
        }
    }
}