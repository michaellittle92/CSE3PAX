using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages
{
    public class AddLecturerModel : PageModel
    {
        public void OnGet()
        {
        }

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConcurrentLoadCapcity { get; set; } = "";
        public string ExpertiseFeild01 { get; set; } = "";

        public void OnPost() {
            FirstName = Request.Form["firstname"];
            LastName = Request.Form["lastname"];
            Email = Request.Form["email"];
            Password = Request.Form["password"];
            ConcurrentLoadCapcity = Request.Form["concurrentloadcapacity"];
            ExpertiseFeild01 = Request.Form["expertisefeild01"];

         

            try {

                String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=schedulingDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = " INSERT INTO lecturers(LastName, FirstName, Email, Password, ConcurrentLoadCapacity, ExpertiseFeild01, isLecturer)\r\n    VALUES  ('Test', 'Test', 'j.smith@latrobe.com.au','password', 6, 'Networking', 1);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    /*
                    string sql = "INSERT INTO lecturers(LastName, FirstName, Email, Password, ConcurrentLoadCapacity, ExpertiseFeild01, isLecturer)" +
                        "(@lastname, @firstname, @email, @password, @concurrentloadcapacity, @expertisefeild01, @islecturer);";

                    using (SqlCommand command = new SqlCommand(sql, connection)) {
                        command.Parameters.AddWithValue("@lastname", LastName);
                        command.Parameters.AddWithValue("@firstname", FirstName);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@password", Password);
                        command.Parameters.AddWithValue("@concurrentloadcapacity", ConcurrentLoadCapcity);
                        command.Parameters.AddWithValue("@expertisefeild01", ExpertiseFeild01);
                        command.Parameters.AddWithValue("@islecturer", 1);

                    command.ExecuteNonQuery();
                    }
                    */
                
            }

            }
            catch (Exception ex) {
                return;
            }
        }
    }
}
