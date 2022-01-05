using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResturantApp.ViewModel
{
    public class StaffViewModel
    {
        public int StaffId { get; set; }

        public int CompanyId { get; set; }

        public string  FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        public string EmailAddress { get; set; }

        public string Address { get; set; }

        public string PaymentCode { get; set; }

        public decimal Balance { get; set; }

        public string Password { get; set; }

    }
}