using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesReportSystem.Model;
using SalesReportSystem.Pages.Report.Model;
using SalesReportSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SalesReportSystem.Pages.Report.admin
{
    public class EditSalesModel : PageModel
    {
        private AuthDbContext _context { get; }
        private readonly IAccessService _accessService;

        public EditSalesModel(AuthDbContext _context, IAccessService accessService)
        {
            this._context = _context;
            _accessService = accessService;
        }

        [BindProperty]
        public SalesModel Sale { get; set; }
        public List<Users> ddlUser { get; set; }

        public async Task<ActionResult> OnGet(int? id)
        {
            UserAccess access = await _accessService.GetCurrentUser();
            if (access == null) Response.Redirect(await _accessService.RedirectPage(), false);
            getUserList(access.UserId);
            if (id != null)
            {
                var data = (from s in _context.sales
                            where s.SaleId == id
                            select s).SingleOrDefault();
                Sale = data;
            }
            return Page();
        }

        public void getUserList(int id)
        {
            ddlUser = _context.users.Where(s => s.UserId == id).ToList();
        }

        public ActionResult OnPost()
        {
            var sales = Sale;
            sales.UpdateDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Entry(sales).Property(x => x.SaleDate).IsModified = true;
            _context.Entry(sales).Property(x => x.Amount).IsModified = true;
            _context.Entry(sales).Property(x => x.UserId).IsModified = true;
            _context.Entry(sales).Property(x => x.UpdateDate).IsModified = true;
            _context.SaveChanges();
            return RedirectToPage("/Report/admin/AllSalesRecord");
        }
    }
}
