using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesReportSystem.Pages.Report.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using SalesReportSystem.Model;
using System.Linq;

namespace SalesReportSystem.Pages.Report.admin
{
    public class AllSalesRecordModel : PageModel
    {
        private readonly IPageListService _pageService;
        private readonly IAccessService _accessService;
        private AuthDbContext _context { get; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public List<SaleJoinUserModel> Data { get; set; }

        public AllSalesRecordModel(AuthDbContext _context, IPageListService pageService, IAccessService accessService)
        {
            this._pageService = pageService;
            this._context = _context;
            _accessService = accessService;
        }


        public async Task OnGetAsync()
        {
            var getcurrent = await _accessService.GetCurrentUser();
            if (getcurrent == null) Response.Redirect(await _accessService.RedirectPage(), false);
            await getSalesList();
        }

        public async Task getSalesList()
        {
            Data = await _pageService.GetPaginatedResult(CurrentPage, PageSize);
            Count = await _pageService.GetCount();
        }
        public async Task<ActionResult> OnGetDelete(int? id)
        {
            if (id != null)
            {
                var data = (from s in _context.sales
                            where s.SaleId == id
                            select s).SingleOrDefault();

                _context.Remove(data);
                _context.SaveChanges();
            }
             await getSalesList();

            return Page();
        }

    }
}
