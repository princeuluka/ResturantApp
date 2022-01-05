using ResturantApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ResturantApp.Controllers
{
    public class StaffController : Controller
    {
        private ResturantDBEntities objResturantDBEntities;

        public StaffController()
        {
            objResturantDBEntities = new ResturantDBEntities();
        }
        // GET: Staff
        public ActionResult Index()
        {
            List<Staff> model = objResturantDBEntities.Staffs.ToList();
            return View(model);
        }

        public ActionResult Staff()
        {
            // StaffRepository objStaffRepository = new StaffRepository();
            //var objStaffModel = new Tuple<IEnumerable<SelectListItem>>(objStaffRepository.GetAllStaffs());
            List<Staff> model = objResturantDBEntities.Staffs.ToList();
            return View(model);
            // return View(from Staff in objResturantDBEntities.Staffs.Take(10) select Staff);
        }

        [HttpGet]
        public JsonResult getStaffData(string PaymentCode)
        {
            //  string FirstName = objResturantDBEntities.Staffs.Single(model => model.PaymentCode == PaymentCode).FirstName;
            //JsonResult json = Json(objResturantDBEntities.Staffs.Where(e => e.PaymentCode == PaymentCode).FirstOrDefault(), JsonRequestBehavior.AllowGet);

            return Json(objResturantDBEntities.Staffs.Where(e => e.PaymentCode == PaymentCode).FirstOrDefault(), JsonRequestBehavior.AllowGet);

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
        public ActionResult Edit([Bind(Include = "StaffId,FirstName,MiddleName,LastName,EnrollmentDate,PhoneNumber,Gender,EmailAddress,Address,PaymentCode,Balance,CompanyId, Password")] Staff staff)
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

            }
            return View(emp);
        }


        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StaffId,FirstName,MiddleName,LastName,EnrollmentDate,PhoneNumber,Gender,EmailAddress,Address,PaymentCode,Balance,CompanyId,Password")] Staff staff
            )
        {
            if (ModelState.IsValid)
            {
                objResturantDBEntities.Staffs.Add(staff);
                objResturantDBEntities.SaveChanges();
                return RedirectToAction("Staff");
            }

            return View(staff);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                objResturantDBEntities.Dispose();
            }
            base.Dispose(disposing);
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
        public List<SelectListItem> GetCompanies()
        {
            var ResultCategory = objResturantDBEntities.Companies.ToList();
            List<SelectListItem> listCategories = new List<SelectListItem>();
            foreach (var items in ResultCategory)
            {
                listCategories.Add(new SelectListItem() { Text = items.CompanyName, Value = items.CompanyId.ToString() });
            }
            return listCategories;
        }
    }
}