using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PWEB_QUIZ2.Models
{
    public class MemberShip
    {
        [Required(ErrorMessage = "Username required")]
        public String Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password required")]
        public String Password { get; set; }
    }

}