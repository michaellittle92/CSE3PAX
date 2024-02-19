using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Admin
{

    //Checking for required Roles
    [RequireRoles("Admin")]

    public class SubjectManagementModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}