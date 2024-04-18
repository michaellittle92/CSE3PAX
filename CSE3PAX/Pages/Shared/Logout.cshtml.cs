using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Shared
{

    // Logout model to handle logout functionality
    public class LogoutModel : PageModel
    {
        /*
        Clears the session data and signs out the user by removing their authentication cookie.
        Redirects the user to the login page ("/Index").
        */
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
