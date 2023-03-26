using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesReportSystem.ViewModel
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
