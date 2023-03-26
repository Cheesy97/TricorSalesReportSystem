using System.ComponentModel.DataAnnotations;

namespace SalesReportSystem.ViewModel
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Password { get; set; }
        public int LoginId { get; set; }
        [Required]
        public string RoleCode { get; set; }
        public int? ReportManager { get; set; }
    }

    public class Manager
    {
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string RoleCode { get; set; }
    }

    public class UserAccess
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string RoleCode { get; set; }
        public int? ReportManager { get; set; }
        public string RoleName { get; set; }
    }
}
