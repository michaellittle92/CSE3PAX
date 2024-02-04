using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Shared
{
    public class AccessDeniedModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Use JavaScript to redirect after 10 seconds
            ViewData["RedirectUrl"] = "/Index";
            return Page();
        }
    }
}
