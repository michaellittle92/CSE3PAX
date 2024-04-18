using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;


namespace CSE3PAX.Pages.Admin
{
    public class ReadSubjectModel : PageModel
    {
        // Readonly field to store IConfiguration
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        // Constructor with IConfiguration parameter
        public ReadSubjectModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        // Store a list of subjects
        public List<ListSubjects> ListSubjects { get; set; } = new List<ListSubjects>();

        // Method executed when the page is loaded via HTTP GET request
        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // SQL query to retrieve subjects
                    string sql = "SELECT SubjectCode, SubjectName, SubjectClassification, YearLevel FROM Subjects;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            // Loop through each row in the result set
                            while (reader.Read())
                            {
                                var subject = new ListSubjects
                                {
                                    SubjectCode = reader["SubjectCode"].ToString(),
                                    SubjectName = reader["SubjectName"].ToString(),
                                    SubjectClassification = reader["SubjectClassification"].ToString(),
                                    YearLevel = reader["YearLevel"].ToString(),
                                };
                                ListSubjects.Add(subject);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
    }
}
