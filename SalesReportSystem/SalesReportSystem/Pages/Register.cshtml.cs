using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesReportSystem.Model;
using SalesReportSystem.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SalesReportSystem.Security.Encryption;
using Microsoft.AspNetCore.DataProtection;
using System;
using SalesReportSystem.Pages.Report.Model;

namespace SalesReportSystem.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public IEnumerable<UserRoles> ddlRole { get; set; }
        [BindProperty]
        public List<Manager> ddlManager { get; set; }
        public Register Model { get; set; }


        private AuthDbContext _context { get; }
        public RegisterModel(AuthDbContext _context)
        {
            this._context = _context;
        }


        public async Task OnGet()
        {
            await getRoleList();
            await getManagerList();
        }

        public async Task getRoleList()
        {
            ddlRole = await _context.userRoles.ToListAsync();
        }

        public async Task getManagerList()
        {
            string manager = "Makerting Manager";

            var sList = await (from u in _context.users
                         join r in _context.userRoles on u.RoleCode equals r.RoleCode
                         where r.RoleName == manager
                         orderby u.Name ascending
                         select new Manager()
                         {
                             UserId = u.UserId,
                             Name = u.Name,
                             RoleCode = r.RoleCode
                         }).ToListAsync();
           
            ddlManager = sList;
            ddlManager.Insert(0, new Manager() { UserId = null, RoleCode = null, Name = "-- Please Select --" });
        }

        private List<SelectListItem> GetSelectListItems(List<UserRoles> elements)
        {
            var selectList = new List<SelectListItem>();
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.RoleCode.ToString(),
                    Text = element.RoleName
                });
            }
            return selectList;
        }


        public async Task<IActionResult> OnPostAsync(Register model)
        {
            string encryptPassword = Encryption.EncryptString(model.Password);

            if (ModelState.IsValid)
            {
                var user = new Users()
                {
                    Name = model.user.Name,
                    Password = encryptPassword,
                    RoleCode = model.user.RoleCode,
                    ReportManager = model.user.ReportManager
                };

                this._context.users.Add(user);
                var result = this._context.SaveChanges();

                if (Convert.ToBoolean(result))
                {
                    return RedirectToPage("Login");
                }

            }
            else
            {
                await getRoleList();
                await getManagerList();
            }
            return Page();
        }

    }
}
