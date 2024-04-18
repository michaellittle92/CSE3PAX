using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages
{
    public class AddLecturerModel : PageModel
    {

        // HTTP OnGet
        public void OnGet()
        {
        }

        // Variables to store user information
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConcurrentLoadCapcity { get; set; } = "";
        public string ExpertiseFeild01 { get; set; } = "";
        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        /*
        Handles the POST request to add a new lecturer to the database.
        Retrieves form data including FirstName, LastName, Email, Password, ConcurrentLoadCapacity, and ExpertiseField01.
        Validates if all required fields are filled.
        Inserts the lecturer information into the database table 'lecturers'.
        If successful, displays a success message and clears the form fields.
        If an error occurs during database insertion, displays an error message.
        */
        public void OnPost() {

            FirstName = Request.Form["firstname"];
            LastName = Request.Form["lastname"];
            Email = Request.Form["email"];
            Password = Request.Form["password"];
            ConcurrentLoadCapcity = Request.Form["concurrentloadcapacity"];
            ExpertiseFeild01 = Request.Form["expertisefeild01"];

            // Check if variables are empty
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

                    /*
                    SQL query to insert a new record into the 'lecturers' table in the database.
                    It inserts values for FirstName, LastName, Email, Pass (Password), ConcurrentLoadCapacity, ExpertiseFeild01,
                    and sets the isLecturer flag (assuming it's a boolean field).
                    Parameters are used for safe and parameterized query execution.
                    */
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
