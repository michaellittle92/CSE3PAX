using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Admin
{
    //Checking for required Roles
    [RequireRoles("Admin")]
  
    public class AdminIndexModel : PageModel {

        // String to store full name (session)
        public string FullName { get; set; }

        public void OnGet() {

            // Session data
            FullName = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");


        }
    }
}