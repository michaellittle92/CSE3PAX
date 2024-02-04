using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Manager
{

    [Authorize(Policy = "isManager")]

    public class ManagerIndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}