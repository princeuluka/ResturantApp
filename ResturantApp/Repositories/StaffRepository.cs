using ResturantApp.Models;
using ResturantApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResturantApp.Repositories
{
    public class StaffRepository
    {
        private ResturantDBEntities objResturantDBEntities;

        public StaffRepository()
        {
            objResturantDBEntities = new ResturantDBEntities();
        }

        public IEnumerable<SelectListItem> GetAllStaffs()
        {
            var objSelectListItems = new List<SelectListItem>();
            objSelectListItems = (from obj in objResturantDBEntities.Staffs
                                  select new SelectListItem()
                                  {
                                      Text = obj.PaymentCode,
                                      Value = obj.StaffId.ToString(),
                                      Selected = true
                                  }).ToList();

            return objSelectListItems;
        }

       
    }
}