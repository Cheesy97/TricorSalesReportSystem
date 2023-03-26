using System;
using System.ComponentModel.DataAnnotations;

namespace SalesReportSystem.Pages.Report.Model
{
    public class SalesModel
    {
        [Key]
        public int SaleId { get; set; }
        [Required]
        public DateTime SaleDate { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        [RegularExpression("^\\d{0,8}(\\.\\d{1,2})?$", ErrorMessage = "Amount must be numeric")]
        public decimal Amount { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int UserId { get; set; }
    }
    public class SaleJoinUserModel
    {
        [Key]
        public int SaleId { get; set; }
        [Required]
        public DateTime SaleDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Amount must be numeric")]
        public decimal Amount { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string SalePersonName { get; set; }
    }

    public class FiscalYearReport
    {
        public string Month { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public string SalesAmount { get; set;}
    }

    public class ManagerMonthlyReport
    {
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        public string Amount { get; set; }
        public string SalePersonName { get; set; }
    }

}
