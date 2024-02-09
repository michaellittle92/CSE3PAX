<<<<<<< Updated upstream
using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Admin
{
    //Checking for required Roles
    [RequireRoles("Admin")]
  
    public class AdminIndexModel : PageModel {
        public void OnGet() {
        }
    }
}
=======
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Manager
{
    public class AdminIndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }

}
>>>>>>> Stashed changes
