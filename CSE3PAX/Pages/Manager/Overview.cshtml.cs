using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Manager
{
    //Checking for required Roles
    [RequireRoles("Manager")]

    public class OverviewModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
