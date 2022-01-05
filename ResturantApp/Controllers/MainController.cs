using Microsoft.AspNetCore.Mvc;
using ResturantApp.Models;
using ResturantApp.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ResturantApp.Controllers
{
    public class MainController : Controller
    {
        ResturantDBEntities objResturantDBEntities;

        public MainController()
        {
            objResturantDBEntities = new ResturantDBEntities();
        }

        // GET: Main
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult CompanyLogin()
        {
            return View();
        }

        public ActionResult StaffLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StaffLogin(Models.Staff objStaff)
        {
            if (ModelState.IsValid)
            {
                using (ResturantDBEntities db = new ResturantDBEntities())
                {
                    var obj = db.Staffs.Where(a => a.PaymentCode.Equals(objStaff.PaymentCode) && a.Password.Equals(objStaff.Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.StaffId.ToString();
                        Session["PaymentCode"] = obj.PaymentCode.ToString();
                        Session["FirstName"] = obj.FirstName.ToString();
                        Session["LastName"] = obj.LastName.ToString();
                        Session["Balance"]= obj.Balance.ToString();
                        return RedirectToAction("ClientDashBoard");
                    }
                    else
                    {
                        ModelState.AddModelError("LogOnError", "The user name or password provided is incorrect.");
                    }
                }
            }
            return View(objStaff);
        }

        public ActionResult ClientDashBoard()
        {
            if (Session["UserID"] != null)
            {
                List<StaffTransaction> model = objResturantDBEntities.StaffTransactions.ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("StaffLogin");
            }
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.Login objUser)
        {
            if (ModelState.IsValid)
            {
                using (ResturantDBEntities db = new ResturantDBEntities())
                {
                    var obj = db.Logins.Where(a => a.UserName.Equals(objUser.UserName) && a.Password.Equals(objUser.Password)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserID"] = obj.LoginId.ToString();
                        Session["UserName"] = obj.UserName.ToString();
                        return RedirectToAction("UserDashBoard");
                    }
                }
            }
            return View(objUser);
        }

        public ActionResult UserDashBoard()  
        {  
            if (Session["UserID"] != null)  
            {
                List<Staff> model = objResturantDBEntities.Staffs.ToList();
                return View(model); 
            } else  
            {  
                return RedirectToAction("Login");  
            }  
        }

        [HttpGet]
        public ActionResult Create(int id = 0)
        {
            var items = objResturantDBEntities.Companies.ToList();
            ViewBag.StaffList = items;

            Staff emp = new Staff();
            var lastPCode = objResturantDBEntities.Staffs.OrderByDescending(c => c.StaffId).FirstOrDefault();
            if (id != 0)
            {
                emp = objResturantDBEntities.Staffs.Where(x => x.StaffId == id).FirstOrDefault<Staff>();
            }
            else if (lastPCode == null)
            {
                emp.PaymentCode = "ASDFGHT";
            }
            else
            {
                emp.PaymentCode = PaymentCode();
                //emp.CompanyId;
                emp.CompanyName = Session["UserName"].ToString();
            }
            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StaffId,FirstName,MiddleName,LastName,EnrollmentDate,PhoneNumber,Gender,EmailAddress,Address,PaymentCode,Balance,CompanyName,Password")] Staff staff
           )
        {
            if (ModelState.IsValid)
            {
                objResturantDBEntities.Staffs.Add(staff);
                objResturantDBEntities.SaveChanges();
                return RedirectToAction("UserDashBoard");
            }

            return View(staff);
        }

        public ActionResult Edit(int? id)
        {

            var items = objResturantDBEntities.Staffs.ToList();
            ViewBag.StaffList = items;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = objResturantDBEntities.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StaffId,FirstName,MiddleName,LastName,EnrollmentDate,PhoneNumber,Gender,EmailAddress,Address,PaymentCode,Balance,CompanyName, Password")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                objResturantDBEntities.Entry(staff).State = EntityState.Modified;
                objResturantDBEntities.SaveChanges();
                return RedirectToAction("Staff");
            }
            return View(staff);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = objResturantDBEntities.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Staff staff = objResturantDBEntities.Staffs.Find(id);
            objResturantDBEntities.Staffs.Remove(staff);
            objResturantDBEntities.SaveChanges();
            return RedirectToAction("Staff");
        }


        public string PaymentCode()
        {
            Random random = new Random();
            int length = 8;
            var rString = "";
            for (var i = 0; i < length; i++)
            {
                rString += ((char)(random.Next(1, 26) + 64)).ToString().ToUpper();
            }
            return rString;
        }

       
        public ActionResult Payment()
        {
            CustomerRepository objCustomerRepository = new CustomerRepository();
            ItemRepository objItemRepository = new ItemRepository();
            PaymentTypeRepository objPaymentTypeRepository = new PaymentTypeRepository();
            StaffRepository objStaffRepository = new StaffRepository();

            var objMutipleModels = new Tuple<IEnumerable<SelectListItem>, IEnumerable<SelectListItem>, IEnumerable<SelectListItem>, IEnumerable<SelectListItem>>
                    (objCustomerRepository.GetAllCustomers(), objItemRepository.GetAllItems(), objPaymentTypeRepository.GetAllPaymentType(), objStaffRepository.GetAllStaffs());

            return View(objMutipleModels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment([Bind(Include = "PaymentId,PaymentCode,CompanyName,Amount,PaymentDate")] Payment payment)
        { 
            if (ModelState.IsValid)
            {
                objResturantDBEntities.Entry(payment).State = EntityState.Modified;
                objResturantDBEntities.SaveChanges();
                return RedirectToAction("payment");
            }
            return View(payment);
        }

        [HttpPost]
        public JsonResult SearchPaymentCode(string paymentCode)
        {

            var staff = from c in objResturantDBEntities.Staffs
                        where c.PaymentCode.Contains(paymentCode)
                        select c;
            return Json(staff.ToList().Take(10));
        }

    }
}