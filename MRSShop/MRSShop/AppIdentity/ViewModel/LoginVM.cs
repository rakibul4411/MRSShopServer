using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MRSShop.AppIdentity.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "User Name is Required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is Required.")]

        public string Password { get; set; }
    }
}
