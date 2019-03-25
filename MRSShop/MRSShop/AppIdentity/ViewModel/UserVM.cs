using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MRSShop.AppIdentity.ViewModel
{
    public class UserVM
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "First Name is Required.")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last Name is Required.")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "User Name is Required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Street address is Required.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City Name is Required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "PostalCode is Required.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is Required.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm Password is Required.")]
        [Compare("Password", ErrorMessage = "Password not match.")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
