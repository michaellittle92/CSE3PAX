using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Admin
{
    public class EditSubjectModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public EditSubjectModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }
        [BindProperty(SupportsGet = true)]
        public string SubjectCode { get; set; }

        [BindProperty]
        public string SubjectName { get; set; }

        [BindProperty]
        public string SubjectClassification { get; set; }

        [BindProperty]
        public string YearLevel { get; set; }

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string getSubjectDataSQLQuery = "SELECT SubjectName, SubjectClassification, YearLevel FROM Subjects WHERE SubjectCode = @SubjectCode";
                    using (SqlCommand command = new SqlCommand(getSubjectDataSQLQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SubjectCode", SubjectCode);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // If there's a result, populate the properties
                            {
                                SubjectName = reader["SubjectName"].ToString();
                                SubjectClassification = reader["SubjectClassification"].ToString();
                                YearLevel = reader["YearLevel"].ToString();

                                Console.WriteLine($"{SubjectCode}, {SubjectName}, {SubjectClassification}, {YearLevel}");
                            }
                        }
                    }
                }
            }

            catch (Exception ex) { }
        }
    }
}
