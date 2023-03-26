using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesReportSystem.ViewModel
{
    public class Register
    {
        public Users user { get; set; }
        public IEnumerable<UserRoles> roleList { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(nameof(Password), ErrorMessage = "Password and confirm Password is Not Match!")]
        public string comfirmPassword { get; set; }
    }
}
