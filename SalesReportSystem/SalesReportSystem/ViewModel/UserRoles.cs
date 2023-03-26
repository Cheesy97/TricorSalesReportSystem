using System.ComponentModel.DataAnnotations;

namespace SalesReportSystem.ViewModel
{
    public class UserRoles
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
    }
}
