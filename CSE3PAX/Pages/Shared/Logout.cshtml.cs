using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Shared
{

    // Logout model to handle logout functionality
    public class LogoutModel : PageModel
    {
        // Handler for the GET request
        public async Task<IActionResult> OnGetAsync()
        {

            // Clear session data
            HttpContext.Session.Clear();

            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to the login page
            return RedirectToPage("/Index");
        }
    }
}
