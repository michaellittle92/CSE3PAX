using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Admin
{
    public class ReadUserModel : PageModel
    {
        public void OnGet()
        {

            /*
             SELECT Email, 
       FirstName, 
       LastName, 
       CASE 
           WHEN IsAdmin = 1 THEN 'Administrator' 
           WHEN IsManager = 1 THEN 'Manager' 
           WHEN IsLecturer = 1 THEN 'Lecturer' 
           ELSE 'No role found' 
       END AS Role
FROM Users;

             */
        }
    }
}
