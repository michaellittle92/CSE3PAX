using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Admin
{
    public class CreateSubjectModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public CreateSubjectModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }
        [BindProperty]
        public string SubjectCode { get; set; }

        [BindProperty]
        public string SubjectName { get; set; }

        [BindProperty]
        public string SubjectClassification { get; set; }

        [BindProperty]
        public string YearLevel { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            try {
                using (SqlConnection connection = new SqlConnection(_connectionString)) { 
                    connection.Open();
                    string insertSubjectSQLQuery = "INSERT INTO Subjects (SubjectCode, SubjectName, SubjectClassification, YearLevel) VALUES (@SubjectCode, @SubjectName, @SubjectClassification, @YearLevel)";
                    using (SqlCommand command = new SqlCommand(insertSubjectSQLQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                        command.Parameters.AddWithValue("@SubjectName", SubjectName);
                        command.Parameters.AddWithValue("@SubjectClassification", SubjectClassification);
                        command.Parameters.AddWithValue("@YearLevel", YearLevel);
                        command.ExecuteNonQuery();
                    }
                    return RedirectToPage("/Admin/ReadSubject");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }
    }
}
