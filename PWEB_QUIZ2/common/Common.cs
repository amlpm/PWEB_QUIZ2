using PWEB_QUIZ2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWEB_QUIZ2.common
{
    public class Common
    {
        public bool validEmployee(string username, string password)
        {
            bool check = false;
            using (Entities con = new Entities())
            {
                if (con.Employees.Any(x => x.Username == username && x.USER_PASSWORD == password && x.Status == "1") == true)
                {
                    check = true;
                }
                else
                    check = false;
            }
            return check;
        }
    }
}