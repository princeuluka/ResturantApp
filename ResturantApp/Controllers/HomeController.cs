using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ResturantApp.Models;
using ResturantApp.Repositories;
using ResturantApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ResturantApp.Controllers
{
    public class HomeController : Controller
    {
        private DB_Entities _db = new DB_Entities();
        private ResturantDBEntities objResturantDBEntities;
        public HomeController()
        {
            objResturantDBEntities = new ResturantDBEntities();
        }
        public ActionResult Main()
        {
            if (Session["idUser"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User _user)
        {
            
            if (ModelState.IsValid)
            {
                var check = _db.Users.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    _user.Password = GetMD5(_user.Password);
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    _db.Users.Add(_user);
                    _db.SaveChanges();
                    return RedirectToAction("Main");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }


            }
            return View();


        }

        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            SqlConnection cs = new SqlConnection("Server=(localdb)\\ProjectsV13;Database=ResturantApp;Trusted_Connection=True;");
            cs.Open();
            SqlCommand cmd = new SqlCommand();


            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var data = _db.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["idUser"] = data.FirstOrDefault().IdUser;
                    int userID = (int)Session["idUser"];

                    cmd.Connection = cs;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_TRACK_USERLOG";
                    cmd.Parameters.AddWithValue("@IdUser", userID);
                    cmd.Parameters.AddWithValue("@COMMAND", 0);
                    cmd.ExecuteNonQuery();
                    return RedirectToAction("Main");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }


        //Logout
        public ActionResult Logout()
        {
            SqlConnection cs = new SqlConnection("Server=(localdb)\\ProjectsV13;Database=ResturantApp;Trusted_Connection=True;");
            cs.Open();
            SqlCommand cmd = new SqlCommand();
            int userID = (int)Session["idUser"];
            cmd.Connection = cs;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_TRACK_USERLOG";
            cmd.Parameters.AddWithValue("@IdUser", userID);
            cmd.Parameters.AddWithValue("@COMMAND", 1);
            cmd.ExecuteNonQuery();
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }



        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }


        public ActionResult Index()
        {
            CustomerRepository objCustomerRepository = new CustomerRepository();
            ItemRepository objItemRepository = new ItemRepository();
            PaymentTypeRepository objPaymentTypeRepository = new PaymentTypeRepository();
            StaffRepository objStaffRepository = new StaffRepository();

            var  objMutipleModels = new Tuple<IEnumerable<SelectListItem>, IEnumerable<SelectListItem>, IEnumerable<SelectListItem>,IEnumerable<SelectListItem>>
                    (objCustomerRepository.GetAllCustomers(),objItemRepository.GetAllItems(),objPaymentTypeRepository.GetAllPaymentType(),objStaffRepository.GetAllStaffs());

            return View(objMutipleModels);
        }

        public ActionResult SearchStaffData(string PaymentCode)
           {
            var items = objResturantDBEntities.Staffs.ToList();
            ViewBag.StaffList = items;

            if (PaymentCode == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = objResturantDBEntities.Staffs.Find(PaymentCode);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

            [HttpGet]
        public JsonResult getItemUnitPrice(int itemId)
        {
            decimal UnitPrice =(decimal)objResturantDBEntities.Items.Single(model => model.ItemId == itemId).ItemPrice;

            return Json(UnitPrice,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Index (OrderViewModel objOrderViewModel)
        {
            OrderRepository objOrderRepository = new OrderRepository();
            objOrderRepository.AddOrder(objOrderViewModel);
            return Json("Your Order has been Successfully Placed", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PaymentDB(PaymentViewModel objPaymentViewModel)
        {
            PaymentRepository objPaymentRepository = new PaymentRepository();
            objPaymentRepository.AddPayment(objPaymentViewModel);
            return Json("Payment Updated", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateRecord(StaffViewModel objStaffViewModel)
        {
            StaffRepository objStaffRepository = new StaffRepository();
            string pcode =  objStaffViewModel.PaymentCode;
            decimal bal = objStaffViewModel.Balance;
            Staff objStaff = objResturantDBEntities.Staffs.FirstOrDefault(x => x.PaymentCode == pcode);
            objStaff.Balance = bal;          
            objResturantDBEntities.SaveChanges();

            return Json("Record Updated", JsonRequestBehavior.AllowGet);
        }
    }
}