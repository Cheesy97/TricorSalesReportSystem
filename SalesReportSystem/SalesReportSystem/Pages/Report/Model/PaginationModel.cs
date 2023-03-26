using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace SalesReportSystem.Pages.Report.Model
{
    public class PaginationModel
    {
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 10;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public List<SalesModel> SaleList { get; set; }
    }
}
