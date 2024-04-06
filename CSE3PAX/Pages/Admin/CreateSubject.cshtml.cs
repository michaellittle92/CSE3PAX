using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Admin
{
    //Checking for required Roles
    [RequireRoles("Admin")]

    public class CreateSubjectModel : PageModel
    {

        // Alert message variable
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

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

        public void OnPost()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Check if subject code already exists
                    string checkDuplicateQuery = "SELECT COUNT(*) FROM Subjects WHERE SubjectCode = @SubjectCode";
                    using (SqlCommand checkCommand = new SqlCommand(checkDuplicateQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            ErrorMessage = "A subject with the provided Subject Code already exists!";
                            return; // Exit the method
                        }
                    }

                    // If subject code is unique, proceed with insertion
                    string insertSubjectSQLQuery = "INSERT INTO Subjects (SubjectCode, SubjectName, SubjectClassification, YearLevel) VALUES (@SubjectCode, @SubjectName, @SubjectClassification, @YearLevel)";
                    using (SqlCommand command = new SqlCommand(insertSubjectSQLQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                        command.Parameters.AddWithValue("@SubjectName", SubjectName);
                        command.Parameters.AddWithValue("@SubjectClassification", SubjectClassification);
                        command.Parameters.AddWithValue("@YearLevel", YearLevel);
                        command.ExecuteNonQuery();

                        // Set the success message
                        SuccessMessage = "Subject created successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
