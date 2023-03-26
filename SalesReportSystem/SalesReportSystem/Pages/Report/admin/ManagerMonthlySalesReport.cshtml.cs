using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesReportSystem.Model;
using SalesReportSystem.Pages.Report.Model;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Globalization;
using SalesReportSystem.ViewModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SalesReportSystem.Pages.Report.admin
{
    public class ManagerMonthlySalesReportModel : PageModel
    {
        private AuthDbContext _context { get; }
        private readonly IAccessService _accessService;
        [BindProperty]
        public IList<SelectListItem> ddlYear { get; set; }
        public IList<ManagerMonthlyReport> MonthlyReportList { get; set; }
        public IEnumerable<string> monthNames { get; set; }
        public List<SelectListItem> ddlManager { get; set; }
        [BindProperty]
        public string getYear { get; set; }
        [BindProperty]
        public decimal TotalMontlySalesAmount { get; set; }
        [BindProperty]
        public string getMonth { get; set; }
        [BindProperty]
        public int getManager { get; set; }
        [TempData]
        public string FilterSearchMessage { get; set; }

        public ManagerMonthlySalesReportModel(AuthDbContext _context, IAccessService accessService)
        {
            this._context = _context;
            this._accessService = accessService;
        }

        public async void OnGet()
        {
            if (_accessService.GetCurrentUser() == null) Response.Redirect(await _accessService.RedirectPage(), false);
            Reset();

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

        public void getManagerList()
        {
            string manager = "Makerting Manager";

            var sList =  (from u in _context.users
                         join r in _context.userRoles on u.RoleCode equals r.RoleCode
                         where r.RoleName == manager
                         orderby u.Name ascending
                         select new Manager()
                         {
                             UserId = u.UserId,
                             Name = u.Name,
                             RoleCode = r.RoleCode
                         }).ToList();

            var selectList = sList.Select(x => new SelectListItem
            {
                Value = x.UserId.ToString(),
                Text = x.Name.ToString()
            }).ToList();

            ddlManager = selectList;
            ddlManager.Insert(0, new SelectListItem() { Value = null, Text = "-- Please Select --" });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                this.FilterSearchMessage = "Sales Report for " + this.getMonth + " " + this.getYear  + " (" + getManagerName(this.getManager) + ")";
                SqlParameter[] parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@Year", SqlDbType.VarChar);
                parameters[0].Value = this.getYear; 
                parameters[1] = new SqlParameter("@Month", SqlDbType.VarChar);
                parameters[1].Value = this.getMonth;
                parameters[2] = new SqlParameter("@ManagerId", SqlDbType.Int);
                parameters[2].Value = this.getManager;

                var result = await _context.ExecuteStoredProcedure("SP_getMonthlyReport_byManager", parameters);
                List<ManagerMonthlyReport> reportlist = new List<ManagerMonthlyReport>();

                if (result != null)
                {
                    foreach (var col in result)
                    {
                        var model = new ManagerMonthlyReport();
                        model.SalePersonName = col[1].ToString();
                        model.Amount = col[0].ToString();

                        decimal tryparseamount;

                        if (decimal.TryParse(model.Amount, out tryparseamount))
                        {
                            TotalMontlySalesAmount += tryparseamount;
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

            Reset();
            if (MonthlyReportList.Count <= 0)
            { this.MonthlyReportList = null; }
            return Page();
        }

        public async void Reset()
        {
            getSaleYear();
            var cultureInfo = new CultureInfo("en-US"); // set the culture to use for the month names
            monthNames = cultureInfo.DateTimeFormat.MonthNames.Take(12); // get the first 12 month names
            getManagerList();
        }

        public string getManagerName(int UserId)
        {
            string name = _context.users.Where(u => u.UserId == UserId).Select(u => u.Name).SingleOrDefault();
            return name;
        }
    }
}
