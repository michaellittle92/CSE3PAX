using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Lecturer
{

    //Checking for required Roles
    [RequireRoles("Lecturer")]
    public class ScheduleModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}