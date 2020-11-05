using PWEB_QUIZ2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PWEB_QUIZ2.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        Entities con = new Entities();

        // This method is used to fetch data from database.
        public ActionResult GetEmp()
        {
            con.Configuration.ProxyCreationEnabled = false;

            var getInfo = (from q in con.Employees
                           select q).ToList();
            if (getInfo.Count > 0)
            {
                return Json(new { success = true, getInfo },
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

                        con.Employees.Add(obj);
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
                return Json(new { success = false, message = "Please fill all fields" },
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
            var checkInfo = (from q in con.Employees
                             where q.Emp_ID == empID
                             select q).FirstOrDefault();
            if (checkInfo != null)
            {
                // Delete here
                try
                {
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
                return Json(new { success = false, message = "Error!" },
                   JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult Index()
        {
            return View();
        }
    }
}