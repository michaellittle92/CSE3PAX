using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Admin
{
    //Checking for required Roles, this should just be admin, but wanted the multi role example.
    [RequireRoles("Admin", "Manager")]
  

    public class AdminIndexModel : PageModel {
        public void OnGet() {
        }
    }
}
