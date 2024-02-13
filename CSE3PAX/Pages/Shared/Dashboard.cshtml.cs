using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CSE3PAX.Pages.Shared
{
    public class DashboardModel : PageModel
    {
        public required string UserType { get; set; }

        // Handler for the GET request
        public IActionResult OnGet()
        {
            // Retrieve user type
            bool isAdministrator = HttpContext.Session.GetBoolean("isAdministrator");
            bool isManager = HttpContext.Session.GetBoolean("isManager");
            bool isLecturer = HttpContext.Session.GetBoolean("isLecturer");

            // Determine user type
            if (isAdministrator)
            {
                return RedirectToPage("/Admin/AdminIndex");
            }
            else if (isManager)
            {
                return RedirectToPage("/Manager/ManagerIndex");
            }
            else if (isLecturer)
            {
                return RedirectToPage("/Lecturer/LecturerIndex");
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
