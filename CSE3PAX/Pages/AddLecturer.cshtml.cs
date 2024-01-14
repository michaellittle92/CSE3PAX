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

        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        //public int IsLecturer = 1;

        public void OnPost() {

            FirstName = Request.Form["firstname"];
            LastName = Request.Form["lastname"];
            Email = Request.Form["email"];
            Password = Request.Form["password"];
            ConcurrentLoadCapcity = Request.Form["concurrentloadcapacity"];
            ExpertiseFeild01 = Request.Form["expertisefeild01"];


            if (FirstName.Length == 0 || LastName.Length == 0 || Email.Length == 0 || Password.Length == 0 ||
                ConcurrentLoadCapcity.Length == 0 || ExpertiseFeild01.Length == 0) {

                ErrorMessage = "Please fill in all required fields";
                return;
            }

            //Add Message to Database 
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=schedulingDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO lecturers(FirstName ,LastName , Email, Pass, ConcurrentLoadCapacity, ExpertiseFeild01, isLecturer)" +
                        "VALUES (@firstname, @lastname, @email,@password, @concurrentloadcapacity, @expertisefeild01,)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@firstname", FirstName);
                        command.Parameters.AddWithValue("@lastname", LastName);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@password", Password);
                        command.Parameters.AddWithValue("@concurrentloadcapacity", ConcurrentLoadCapcity);
                        command.Parameters.AddWithValue("@expertisefeild01", ExpertiseFeild01);
                        //command.Parameters.AddWithValue(@"islecturer", IsLecturer);

                    }

                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }
            SuccessMessage = "Lecturer added to Database.";

            FirstName = "";
            LastName = "";
            Email = "";
            Password = "";
            ConcurrentLoadCapcity = "";
            ExpertiseFeild01 = "";


        }
    }
}
