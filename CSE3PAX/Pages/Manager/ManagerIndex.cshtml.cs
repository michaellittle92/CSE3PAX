using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CSE3PAX.HelpClasses;

namespace CSE3PAX.Pages.Manager
{
    //Checking for required Roles
    [RequireRoles("Manager")]

    public class ManagerIndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}