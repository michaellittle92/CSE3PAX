using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace CSE3PAX.Pages.Shared
{
    public class DashboardModel : PageModel
    {
        // Variable to store user type
        public required string UserType { get; set; }

        /*
        Retrieves the user type from the session and redirects the user to their respective index page based on their role.
        Checks if the user is an administrator, manager, or lecturer by retrieving boolean values from the session.
        If the user is an administrator, redirects to the AdminIndex page.
        If the user is a manager, redirects to the ManagerIndex page.
        If the user is a lecturer, redirects to the LecturerIndex page.
        If the user's role is not recognized or if there is no active session, redirects to the default Index page.
        */
        public IActionResult OnGet()
        {
            // Variables to store user type based on user session information
            bool isAdministrator = HttpContext.Session.GetBoolean("isAdministrator");
            bool isManager = HttpContext.Session.GetBoolean("isManager");
            bool isLecturer = HttpContext.Session.GetBoolean("isLecturer");

            // Set index page based on user type
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
