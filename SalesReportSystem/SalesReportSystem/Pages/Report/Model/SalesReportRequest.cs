using System;

namespace SalesReportSystem.Pages.Report.Model
{
    public class SalesReportRequest
    {
        public DateTime SaleDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int UserId { get; set; }
    }
}
