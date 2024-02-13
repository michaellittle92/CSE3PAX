using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSE3PAX.Pages.Shared
{
    public class MyProfileModel : PageModel


    {
        public string Email { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; }
        public string UserImage { get; set; }


        public void OnGet()
        {
            Email = HttpContext.Session.GetString("Email");
            UserID = (int)HttpContext.Session.GetInt32("UserID");
            FirstName = HttpContext.Session.GetString("FirstName");
            LastName = HttpContext.Session.GetString("LastName");
            FullName = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");

            // Retrieve user type flags from session
            bool isAdministrator = HttpContext.Session.GetBoolean("isAdministrator");
            bool isManager = HttpContext.Session.GetBoolean("isManager");
            bool isLecturer = HttpContext.Session.GetBoolean("isLecturer");

            // Determine user type based on the flags
            if (isAdministrator)
            {
                UserType = "Administrator";
                UserImage = "https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava3.webp";
            }
            else if (isManager)
            {
                UserType = "Manager";
                UserImage = "https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava5.webp";
            }
            else if (isLecturer)
            {
                UserType = "Lecturer";
                UserImage = "https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava6.webp";
            }
            else
            {
                UserType = "Unknown"; // Default value if none of the flags are set
            }
        }
    }
}
