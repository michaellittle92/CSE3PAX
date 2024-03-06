using Microsoft.AspNetCore.Mvc;
using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Admin
{
    public class ReadSubjectInstanceModel : PageModel
    {

        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public ReadSubjectInstanceModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        public List<ListSubjectInstances> ListSubjectInstances { get; set; } = new List<ListSubjectInstances>();


        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    string sql = "SELECT SubjectInstanceID, SubjectInstanceName, SubjectInstanceCode, Subjects.SubjectName, Users.FirstName, Users.LastName, Users.Email,  CONVERT(date, StartDate) as StartDate,\r\n    CONVERT(date, EndDate) as EndDate\r\nFROM SubjectInstance\r\nLEFT JOIN Lecturers ON Lecturers.LecturerID = SubjectInstance.LecturerID\r\nLEFT JOIN Users ON Users.UserID = Lecturers.UserID\r\nLEFT JOIN Subjects ON Subjects.SubjectID = SubjectInstance.SubjectID;\r\n";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var subjectInstance = new ListSubjectInstances
                                {
                                    SubjectInstanceID = (int)reader["SubjectInstanceID"],
                                    SubjectInstanceName = reader["SubjectInstanceName"].ToString(),
                                    SubjectInstanceCode = reader["SubjectInstanceCode"].ToString(),
                                    SubjectName = reader["SubjectName"].ToString(),
                                    LecturerFirstName = reader["FirstName"].ToString(),
                                    LecturerLastName = reader["LastName"].ToString(),
                                    LecturerEmail = reader["Email"].ToString(),
                                    StartDate = ((DateTime)reader["StartDate"]),
                                    EndDate = ((DateTime)reader["EndDate"]) // For date-only string

                                };
                                ListSubjectInstances.Add(subjectInstance);
                                //For testing
                                //Console.WriteLine($"SubjectCode: {subjectInstance.SubjectCode}, SubjectInstanceID: {subjectInstance.SubjectInstanceID}, SubjectInstanceName: {subjectInstance.SubjectInstanceName}, SubjectInstanceDescription: {subjectInstance.SubjectInstanceDescription}, SubjectInstanceYear: {subjectInstance.SubjectInstanceYear}");
                            }
                        }
                    }
                }
                                                     
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine(ex.Message);
            }
        }
    }
}
