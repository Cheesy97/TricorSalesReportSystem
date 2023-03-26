using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SalesReportSystem.ViewModel;
using SalesReportSystem.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SalesReportSystem.Security.Encryption;
using SalesReportSystem.Pages.Report.Model;

namespace SalesReportSystem.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AuthDbContext _context;
        private readonly IAccessService _accessService;
        [BindProperty]
        public Login loginVM { get; set; }

        public LoginModel(AuthDbContext _context, IAccessService accessService)
        {
            this._context = _context;
            _accessService = accessService;

        }

        public void OnGet()
        { 
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                // Verification.  
                if (ModelState.IsValid)
                {
                    string encryptPassword = Encryption.EncryptString(this.loginVM.Password);
                    // Initialization.  
                    var loginInfo = LoginMethodAsync(this.loginVM.UserName, encryptPassword);
                    string decryPassword = Encryption.Decrypt(encryptPassword);
                    // Verification.  
                    if (loginInfo != null && loginInfo.Count > 0)
                    {
                        var getId = loginInfo.First(item => item.Name == loginVM.UserName).UserId.ToString();
                        _accessService.SetUserSession(getId);
                        return RedirectToPage("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info  
                Console.Write(ex);
            }

            // Info.  
            return this.Page();
        }

        public List<Users> LoginMethodAsync(string usernameVal, string passwordVal)
        {
            // Initialization.  
            List<Users> list = new List<Users>();

            try
            {
                string sql = "EXEC SP_Login @username, @password";

                List<SqlParameter> parms = new List<SqlParameter>
                {
                    // Create parameter(s)    
                    new SqlParameter { ParameterName = "@username", Value = usernameVal },
                    new SqlParameter { ParameterName = "@password", Value = passwordVal }
                };

                list = _context.users.FromSqlRaw<Users>(sql, parms.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Info.  
            return list;
        }
    }
}
