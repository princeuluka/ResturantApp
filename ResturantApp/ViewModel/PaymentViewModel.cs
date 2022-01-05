using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResturantApp.ViewModel
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public string PaymentCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string OrderNumber { get; set; }
    }
}