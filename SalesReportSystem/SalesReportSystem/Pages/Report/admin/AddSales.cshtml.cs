using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SalesReportSystem.Model;
using SalesReportSystem.Pages.Report.Model;
using SalesReportSystem.Security.Encryption;
using SalesReportSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesReportSystem.Pages.Report.admin
{
    public class AddSalesModel : PageModel
    {
        private AuthDbContext _context { get; }
        private readonly IAccessService _accessService;
        private readonly IConfiguration _configuration;

        public SalesModel Model { get; set; }
        [BindProperty]
        public List<Users> ddlUser { get; set; }
        public List<SaleJoinUserModel> SaleList { get; set; }
        [TempData]
        public string AddSalesMessageText { get; set; }

       
        public AddSalesModel(AuthDbContext _context, IAccessService accessService, IConfiguration configuration)
        {
            this._context = _context;
            this._accessService = accessService;
            _configuration = configuration;
        }
       

        public async Task<IActionResult> OnGetAsync()
        {
            this.AddSalesMessageText = null;
            var user = await _accessService.GetCurrentUser();
            if (user == null) Response.Redirect(await _accessService.RedirectPage(), false);
            getUserList();
            getSaleList();
            return Page();
        }

        public async void getSaleList()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>().UseSqlServer(_configuration.GetConnectionString("AuthConnectionString")).Options;
            using (var context = new AuthDbContext(options, _configuration))
            {
                var sList = await (from s in context.sales
                                   join u in context.users on s.UserId equals u.UserId
                                   orderby s.SaleId descending
                                   select new SaleJoinUserModel()
                                   {
                                       SaleId = s.SaleId,
                                       SaleDate = s.SaleDate,
                                       Amount = s.Amount,
                                       SalePersonName = u.Name
                                   }).Take(5).ToListAsync();

                SaleList = sList;
            }
            
        }

        public async void getUserList()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>().UseSqlServer(_configuration.GetConnectionString("AuthConnectionString")).Options;
            using (var context = new AuthDbContext(options, _configuration))
            {
                ddlUser = await context.users.Where(u => u.RoleCode == "R005").ToListAsync();
            }
               
        }

        public async Task<IActionResult> OnPost(SalesModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var sales = new SalesModel()
                    {
                        SaleDate = model.SaleDate,
                        Amount = model.Amount,
                        UserId = model.UserId,
                    };

                    this._context.sales.Add(sales);
                    int result = await this._context.SaveChangesAsync();
                    if(Convert.ToBoolean(result))
                    {
                        this.AddSalesMessageText = "Successful Add Sales";
                    }

                }
                else
                {
                    this.AddSalesMessageText = "Failed to Add Sales, Please Try again.";
                }
            }
            catch (Exception ex)
            {
                this.AddSalesMessageText = ex.Message.ToString();
            }

            getSaleList();
            getUserList();
            ModelState.Clear();
            return Page();
        }
        

    }
}
