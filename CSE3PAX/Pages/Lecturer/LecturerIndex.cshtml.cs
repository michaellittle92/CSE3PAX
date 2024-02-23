using Microsoft.AspNetCore.Mvc.RazorPages;
using CSE3PAX; 
using Microsoft.AspNetCore.Authorization;
using CSE3PAX.HelpClasses;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace CSE3PAX.Pages.Lecturer
{
    //Checking for required Roles
    [RequireRoles("Lecturer")]

    public class LecturerIndexModel : PageModel
    {
        // String to store full name (session)
        public string FullName { get; set; }

        // Object to access application settings
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public LecturerIndexModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        //Current Table row headers
        public List<string> Next12Months { get; set; } = new List<string>();

        //SubjectInstance class in HelpClasses -> SubjectInstance.cs
        public List<SubjectInstance> SubjectInstances { get; set; } = new List<SubjectInstance>();


        public void OnGet()
        {

            // Session data
            FullName = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");

            DateTime now = DateTime.Now;
            for (int i = 0; i < 12; i++)
            {
                DateTime nextMonth = now.AddMonths(i);
                Next12Months.Add(nextMonth.ToString("MMMM-yyyy"));
            }

            var session = HttpContext.Session;
            var userID = session.GetInt32("UserID");
           
            if (!userID.HasValue)
            {
                // Handle case where userID is not set, perhaps redirect to login
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Corrected SQL command to use parameter directly in the WHERE clause
                    string sql = "SELECT SubjectInstanceCode, SubjectName, StartDate,EndDate FROM SubjectInstance LEFT JOIN Subjects ON SubjectInstance.SubjectID = Subjects.SubjectID WHERE LecturerID = (SELECT LecturerID FROM Lecturers WHERE UserID = @UserID)\r\n";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Use the parameter directly in your SQL command
                        command.Parameters.AddWithValue("@UserID", userID.Value);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SubjectInstances.Add(new SubjectInstance
                                {
                                    // Assuming these are the column names in your SubjectInstance table
                                    InstanceName = reader["SubjectInstanceCode"].ToString(),
                                    SubjectName = reader["SubjectName"].ToString(),
                                    // Correctly handle DateTime data types
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")).ToString("MMMM-yyyy"),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")).ToString("MMMM-yyyy"),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Consider logging the exception
                // Handle any errors that might have occurred during database access
            }
        }
    }
}