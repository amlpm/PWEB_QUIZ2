using PWEB_QUIZ2.common;
using PWEB_QUIZ2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PWEB_QUIZ2.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        Entities con = new Entities();
        Common obj = new Common();

        [HttpGet]
        public ActionResult Login(string ReturnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                string userName = User.Identity.Name;
                var info = (from e in con.Employees
                            join ur in con.UserRoles on e.Emp_ID equals ur.Emp_Id
                            join r in con.Roles on ur.RoleID equals r.Id
                            where e.Username == userName
                            select new { r.Name }).ToList();
                if (info.Count > 0)
                {
                    string RoleName = info[0].Name;
                    if (RoleName == "Admin")
                    {
                        return RedirectToAction("Index", "Employee");
                    }
                    else
                    {
                        Session["Username"] = User.Identity.Name;
                        return RedirectToAction("Login", "Account");
                    }
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(MemberShip model, string ReturnUrl)
        {
            con.Configuration.ProxyCreationEnabled = false;
            if (ModelState.IsValid == true)
            {
                using (con)
                {
                    if (obj.validEmployee(model.Username, model.Password) == true)
                    {
                        var info = (from e in con.Employees
                                    join ur in con.UserRoles on e.Emp_ID equals ur.Emp_Id
                                    join r in con.Roles on ur.RoleID equals r.Id
                                    where e.Username == model.Username && e.USER_PASSWORD
                                    == model.Password
                                    select new { r.Name }).ToList();
                        if (info.Count > 0)
                        {
                            string RoleName = info[0].Name;
                            if (RoleName == "Admin")
                            {
                                Session["Role"] = RoleName;
                                string Username = model.Username.ToString();

                                FormsAuthentication.SetAuthCookie(Username, false);
                                return RedirectToLocal(ReturnUrl);
                            }
                            else
                            {
                                Session["Role"] = RoleName;
                                Session["Username"] = model.Username;
                                string Username = model.Username.ToString();

                                FormsAuthentication.SetAuthCookie(Username, false);
                                return RedirectToAction("Home", "User");
                            }
                        }
                    }
                    else
                        ModelState.AddModelError("", "Username/Password is invalid");
                    return View();
                }

            }
            else

                return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register(string ReturnUrl)
        {
            UserRole user = new UserRole();
            return View(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(UserRole x, string ReturnUrl)
        {
            using (con)
            {
                Employee obj = new Employee();
                obj.Name = x.Employee.Name;
                obj.Position = x.Employee.Position;
                obj.Gender = x.Employee.Gender;
                obj.Age = x.Employee.Age;
                obj.Salary = x.Employee.Salary;
                obj.Username = x.Employee.Username;
                obj.USER_PASSWORD = x.Employee.USER_PASSWORD;
                obj.Status = "1";
                con.Employees.Add(obj);
                con.SaveChanges();
                int roleIdInt = Convert.ToInt32(x.Role.Id);
                UserRole usr = new UserRole();
                usr.Emp_Id = obj.Emp_ID;
                usr.RoleID = roleIdInt;
                con.UserRoles.Add(usr);
                con.SaveChanges();
            }
            ModelState.Clear();

            ViewBag.SuccessMessage = "Registration Successful";
            return RedirectToAction("Login", "Account");
            //return View();
        }
        public ActionResult RedirectToLocal(string ReturnUrl)
        {
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("Index", "Employee");
        }

        public ActionResult Logout()
        {
            Session["Username"] = null;
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}