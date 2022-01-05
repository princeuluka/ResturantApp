using ResturantApp.Models;
using ResturantApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResturantApp.Repositories
{
    public class PaymentRepository
    {
        public string orderNumber;
        private ResturantDBEntities objResturantDBEntities;
        //OrderRepository obj =  new OrderRepository();

        public PaymentRepository()
        {
            objResturantDBEntities = new ResturantDBEntities();
        }

        public bool AddPayment(PaymentViewModel objPaymentViewModel)
        {
            Payment objpayment = new Payment();
            objpayment.PaymentCode = objPaymentViewModel.PaymentCode;
            objpayment.Amount = objPaymentViewModel.Amount;
            objpayment.PaymentDate = DateTime.Now;
            objpayment.OrderNumber = orderNumber;
            objResturantDBEntities.Payments.Add(objpayment);
            objResturantDBEntities.SaveChanges();
            return true;
        }
    }

    
}