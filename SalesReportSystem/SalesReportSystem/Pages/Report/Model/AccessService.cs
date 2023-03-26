using Microsoft.AspNetCore.Http;
using SalesReportSystem.Model;
using SalesReportSystem.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace SalesReportSystem.Pages.Report.Model
{
    public interface IAccessService
    {
        Task<string> RedirectPage();
        Task<UserAccess> GetCurrentUser();
        Task<string> LogoutSession();
        void SetUserSession(string getId);
    }

    public class AccessService : IAccessService
    {
        private const string _UserLogged = "UserLogged";
        private string getSession { get; set;}
        private AuthDbContext _context { get; }
        private readonly IHttpContextAccessor contextAccessor;
        public AccessService(AuthDbContext _context, IHttpContextAccessor contextAccessor)
        {
            this._context = _context;
            this.contextAccessor = contextAccessor;
        }
        private HttpContext Context
        {
            get
            {
                return contextAccessor.HttpContext;
            }

        }
        private async Task<string> GetUserSession()
        {
            try
            {
                var sessionId = Context.Session.GetString(_UserLogged);
                return sessionId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> LogoutSession()
        {
            try
            {
                Context.Session.Clear();
                Context.Session.Remove(_UserLogged);
                
            }
            catch (Exception ex)
            {

            }
            return await RedirectPage();
        }

        public void SetUserSession(string getId)
        {
            Context.Session.SetString("UserLogged", getId);
        }


        public async Task<UserAccess> GetCurrentUser()
        {
            try
            {
                getSession = await GetUserSession();
                var CurrentUser = await (from u in _context.users
                                         join r in _context.userRoles on u.RoleCode equals r.RoleCode
                                         where u.UserId == Convert.ToInt32(getSession)
                                         select new UserAccess()
                                         {
                                             UserId = u.UserId,
                                             Name = u.Name,
                                             ReportManager = u.ReportManager,
                                             RoleCode = u.RoleCode,
                                             RoleName = r.RoleName
                                         }
                                   ).FirstOrDefaultAsync();

                return CurrentUser;
            }
            catch {
                return null;
            }
            
        }

        public async Task<string> RedirectPage()
        {
            UserAccess User = await GetCurrentUser();
            string page = "/Index";
            if (User == null)
            {
                page = "/Login";
            }
            else
            {
                if (User.RoleName == "Makerting Manager")
                {
                    page = "Report/Admin/SalesSummaryReport";
                }
                else
                {
                    page = "Report/Admin/AddSales";
                }
            }

            return page;
        }
    }
}
