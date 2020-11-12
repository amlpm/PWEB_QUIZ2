using PWEB_QUIZ2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PWEB_QUIZ2.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        Entities con = new Entities();

        // This method is used to fetch data from database.
        public ActionResult GetEmp(string empName = "",string sort="", string empPosition="", string empUsr = "")
        {
            con.Configuration.ProxyCreationEnabled = false;

            var getInfo = from q in con.Employees
                           select q;
            if (empName != null && empName != "")
            {
                getInfo = getInfo.Where(q => q.Name.Contains(empName));
            }
            if (empPosition != null && empPosition != "")
            {
                getInfo = getInfo.Where(q => q.Position.Contains(empPosition));
            }
            if (empUsr != null && empUsr != "")
            {
                getInfo = getInfo.Where(q => q.Username.Contains(empUsr));
            }
            if (sort=="Name"){ getInfo = getInfo.OrderBy(q => q.Name);}
            else if (sort == "Position") { getInfo = getInfo.OrderBy(q => q.Position); }
            else if (sort == "Gender") { getInfo = getInfo.OrderBy(q => q.Gender); }
            else if(sort == "Age") { getInfo = getInfo.OrderBy(q => q.Age); }
            else if(sort == "Salary") { getInfo = getInfo.OrderBy(q => q.Salary); }
            var getInfoList = getInfo.ToList();
            if (getInfoList.Count > 0)
            {
                return Json(new { success = true, getInfoList },
                JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, message = "No Data Found" },
                JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult AddOrEdit(Employee model)
        {
            con.Configuration.ProxyCreationEnabled = false;
            string message = string.Empty;
            
            if (ModelState.IsValid == true)
            {
                var check = (from q in con.Employees
                             where q.Emp_ID == model.Emp_ID
                             select q).FirstOrDefault();
                if (check != null)
                {
                    //Update here
                    check.Name = model.Name;
                    check.Position = model.Position;
                    check.Gender = model.Gender;
                    check.Age = model.Age;
                    check.Gender = model.Gender;
                    check.Salary = model.Salary;
                    check.Status = "1";

                    con.Entry(check).State = EntityState.Modified;
                    con.SaveChanges();
                    return Json(new
                    {
                        Exceptions = "Update",
                        success = true,
                        message =
                             "Updated Successfully"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Add here
                    var checkUsername = (from q in con.Employees
                                         where q.Username == model.Username
                                         select q).ToList();
                    if (checkUsername.Count > 0)
                    {
                        return Json(new
                        {
                            Exceptions = "Exist",
                            success = true,
                            message =
                            "Username is already exist"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Employee obj = new Employee();
                        obj = model;
                        obj.Status = "1";
                        con.Employees.Add(obj);
                        con.SaveChanges();
                        UserRole usr = new UserRole();
                        usr.Id = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        usr.Emp_Id = obj.Emp_ID;
                        usr.RoleID = 2;
                        con.UserRoles.Add(usr);
                        con.SaveChanges();

                        return Json(new
                        {
                            Exceptions = "Add",
                            success = true,
                            message =
                                "Added Successfully"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
                return Json(new { success = false, message = "Please fill all fields"},
                JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetEmployeeById(int empID = 0)
        {
            con.Configuration.ProxyCreationEnabled = false;
            var checkExist = (from q in con.Employees
                              where q.Emp_ID == empID
                              select q).FirstOrDefault();
            if (checkExist != null)
            {
                return Json(checkExist, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("No Data Found", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteEmployee(int empID)
        {
            var checkInf = (from p in con.UserRoles
                            where p.Emp_Id == empID
                            select p).FirstOrDefault();
            var checkInfo = (from q in con.Employees
                             where q.Emp_ID == empID
                             select q).FirstOrDefault();
            if (checkInf!=null)
            {
                // Delete here
                try
                {
                    //con.Employees.Remove(checkInfo);
                    con.UserRoles.Remove(checkInf);
                    con.SaveChanges();
                   // con.Entry(checkInf).State = EntityState.Deleted;
                    //con.SaveChanges();
                    con.Entry(checkInfo).State = EntityState.Deleted;
                    con.SaveChanges();

                    return Json(new { success = true, message = "Deleted Successfully" },
                         JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        Exceptions = "Error",
                        success = true,
                        message = e +
                                         "Error!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { success = true, message = "Error!" },
                   JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult Index()
        {
            return View();
        }
    }
}