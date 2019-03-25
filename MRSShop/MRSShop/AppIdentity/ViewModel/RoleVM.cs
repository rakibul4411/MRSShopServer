using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MRSShop.AppIdentity.ViewModel
{
    public class RoleVM
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Name is Required.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
