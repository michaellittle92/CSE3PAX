using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Admin
{
    [RequireRoles("Admin", "Manager")]
   // [Authorize(Policy = "isAdministrator")]

    public class AdminIndexModel : PageModel {
        public void OnGet() {
        }
    }
}
