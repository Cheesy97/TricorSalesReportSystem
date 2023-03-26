using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesReportSystem.Model;
using SalesReportSystem.Pages.Report.Model;
using SalesReportSystem.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace SalesReportSystem.Pages.Report.Pages
{
    public class SalesSummaryReportModel : PageModel
    {

        private AuthDbContext _context { get; }
        private readonly IAccessService _accessService;

        [BindProperty]
        public IList<SelectListItem> ddlYear { get; set; }
        public IList<FiscalYearReport> MonthlyReportList { get; set; }
        [BindProperty]
        public string getFiscalYear { get; set; }
        public decimal TotalYearSalesAmount { get; set; }
        [TempData]
        public string YearTittle { get; set; }

        public SalesSummaryReportModel(AuthDbContext _context, IAccessService accessService)
        {
            this._context = _context;
            this._accessService = accessService;
        }


        public async void OnGet()
        {
            this.TotalYearSalesAmount = 0;
            this.YearTittle = null;
            if (_accessService.GetCurrentUser() == null) Response.Redirect(await _accessService.RedirectPage(), false);
            getSaleYear();
        }
        public async void getSaleYear()
        {
            try
            {
                var result = await _context.ExecuteStoredProcedure("SP_getDDLSaleList", null);
                var selectList = result.Select(x => new SelectListItem
                {
                    Value = x[0].ToString(),
                    Text = x[0].ToString()
                }).ToList();
                ddlYear = selectList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostSubmit()
        {
            try
            {
                this.YearTittle = "Fiscal Year on " + this.getFiscalYear;
                var result = await _context.ExecuteStoredProcedure("SP_getMonthlySales_FiscalYear", new SqlParameter("@Year", this.getFiscalYear));
                List<FiscalYearReport> reportlist = new List<FiscalYearReport>();

                if (result != null)
                {
                    foreach (var col in result)
                    {
                        var model = new FiscalYearReport();
                        model.SalesAmount = col[1].ToString();
                        model.Month = col[0].ToString();

                        decimal tryparseamount;

                        if (decimal.TryParse(model.SalesAmount, out tryparseamount))
                        {
                            TotalYearSalesAmount += tryparseamount;
                        }
                        
                        reportlist.Add(model);
                    }

                    MonthlyReportList = reportlist;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            getSaleYear();
            return Page();
        }
    }
}
