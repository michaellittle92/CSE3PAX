using Microsoft.AspNetCore.Mvc;
using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace CSE3PAX.Pages.Admin
{
    public class CreateSubjectInstanceModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public CreateSubjectInstanceModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }


        [BindProperty]
        public string SubjectInstanceName { get; set; }
        [BindProperty]
        public string SubjectInstanceCode { get; set; }
        [BindProperty]
        public string LecturerFirstName { get; set; }
        [BindProperty]
        public string LecturerLastName { get; set; }
        [BindProperty]
        public string LecturerID { get; set; }
        [BindProperty]
        public string StartDate { get; set; }
        [BindProperty]
        public string EndDate { get; set; }

        public List<SubjectsForCreateSubjectInstance> SelectedSubject { get; set; } = new List<SubjectsForCreateSubjectInstance>();

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT SubjectID, SubjectName FROM Subjects";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var subject = new SubjectsForCreateSubjectInstance
                                {
                                    SubjectID = Convert.ToInt32(reader["SubjectID"]),
                                    SubjectName = reader["SubjectName"].ToString()
                                };
                                SelectedSubject.Add(subject);
                            }
                        }
                    }

                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
