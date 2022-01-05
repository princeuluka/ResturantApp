using ResturantApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResturantApp.Repositories
{
    public class PaymentTypeRepository
    {

        private ResturantDBEntities objResturantDBEntities;

        public PaymentTypeRepository()
        {
            objResturantDBEntities = new ResturantDBEntities();
        }

        public IEnumerable<SelectListItem> GetAllPaymentType()
        {
         var objSelectListItems = new List<SelectListItem>();
                                    objSelectListItems = (from obj in objResturantDBEntities.PaymentTypes
                                  select new SelectListItem()
                                  { 
                                  Text = obj.PaymentTypeName,
                                  Value = obj.PaymentTypeId.ToString(),
                                  Selected =  true
                                  }).ToList();
             
            return objSelectListItems;
        }
    }
}