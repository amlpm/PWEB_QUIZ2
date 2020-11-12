using PWEB_QUIZ2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PWEB_QUIZ2.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        Entities con = new Entities();
        public ActionResult Home()
        {
            if (Session["Username"] != null)
            {
                var userName = Session["Username"].ToString(); 
                var getUserInfo = (from q in con.Employees
                                   where q.Username == userName
                                   select q).ToList();
                if (getUserInfo.Count > 0)
                {
                    ViewBag.Emp_ID = getUserInfo[0].Emp_ID;
                    ViewBag.Name = getUserInfo[0].Name;
                    ViewBag.Position = getUserInfo[0].Position;
                    ViewBag.Gender = getUserInfo[0].Gender;
                    ViewBag.Age = getUserInfo[0].Age;
                    ViewBag.Salary = getUserInfo[0].Salary;
                    ViewBag.Username = getUserInfo[0].Username;
                    ViewBag.Password = getUserInfo[0].USER_PASSWORD;
                    ViewBag.Status = getUserInfo[0].Status;

                }
                return View();
            }
            else return RedirectToAction("Login", "Account");
        }

        public ActionResult UpdateUser(Employee emp)
        {
            con.Configuration.ProxyCreationEnabled = false;
            var userInfo = (from q in con.Employees
                            where q.Emp_ID == emp.Emp_ID
                            select q).FirstOrDefault();
            if (userInfo != null)
            {
                userInfo.Name = emp.Name;
                userInfo.Gender = emp.Gender;
                userInfo.Position = emp.Position;
                userInfo.Age = emp.Age;
                userInfo.Salary = emp.Salary;
                userInfo.USER_PASSWORD = emp.USER_PASSWORD;

                con.Entry(userInfo).State = EntityState.Modified;
                con.SaveChanges();

                return Json(new { success = true, message = "Updated successfully" },
                JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Error" },
            JsonRequestBehavior.AllowGet);
        }
    }
}