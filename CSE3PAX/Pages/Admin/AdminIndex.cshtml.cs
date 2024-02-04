using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Admin
{
    [Authorize(Policy = "isAdministrator")]

    public class AdminIndexModel : PageModel {
        public void OnGet() {
        }
    }
}
