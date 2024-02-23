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

        // String to store full name (session)
        public string FullName { get; set; }



        public void OnGet()
        {

            // Session data
            FullName = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");

        }
    }
}