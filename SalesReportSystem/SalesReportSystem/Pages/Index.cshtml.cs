using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using SalesReportSystem.Pages.Report.Model;
using SalesReportSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authentication;
using static System.Collections.Specialized.BitVector32;

namespace SalesReportSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAccessService _accessService;
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public IList<string> userRole { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IAccessService accessService, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _accessService = accessService;
            _userManager = userManager;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _accessService.GetCurrentUser();
            Response.Redirect(await _accessService.RedirectPage(), false);

            return Page();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            return RedirectToPage(await _accessService.LogoutSession());
        }
    }
}
