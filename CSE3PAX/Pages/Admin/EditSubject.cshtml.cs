using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace CSE3PAX.Pages.Admin
{
    //Checking for required Roles
    [RequireRoles("Admin")]
    public class EditSubjectModel : PageModel
    {

        // Alert message variable
        public string SuccessMessage { get; set; }

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


        public IActionResult OnPost()
           
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string updateSubjectDataSQLQuery = "UPDATE Subjects SET SubjectName = @SubjectName, SubjectClassification = @SubjectClassification, YearLevel = @YearLevel WHERE SubjectCode = @SubjectCode";
                    using (SqlCommand command = new SqlCommand(updateSubjectDataSQLQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                        command.Parameters.AddWithValue("@SubjectName", SubjectName);
                        command.Parameters.AddWithValue("@SubjectClassification", SubjectClassification);
                        command.Parameters.AddWithValue("@YearLevel", YearLevel);

                        command.ExecuteNonQuery();

                        // Set the success message
                        SuccessMessage = "Subject edited successfully.";
                        // Redirect
                        return RedirectToPage("/Admin/SubjectManagement");
                    }
                    return Page();
                }
            }
            catch (Exception ex) { 
                return null; 
            }
        }


        public IActionResult OnPostDelete()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Delete associated instances in the SubjectInstance table
                    string deleteInstancesQuery = "DELETE FROM SubjectInstance WHERE SubjectID IN (SELECT SubjectID FROM Subjects WHERE SubjectCode = @SubjectCode)";
                    using (SqlCommand deleteInstancesCommand = new SqlCommand(deleteInstancesQuery, connection))
                    {
                        deleteInstancesCommand.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                        deleteInstancesCommand.ExecuteNonQuery();
                    }

                    // Delete the subject
                    string deleteSubjectDataSQLQuery = "DELETE FROM Subjects WHERE SubjectCode = @SubjectCode";
                    using (SqlCommand deleteSubjectCommand = new SqlCommand(deleteSubjectDataSQLQuery, connection))
                    {
                        deleteSubjectCommand.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                        deleteSubjectCommand.ExecuteNonQuery();
                    }

                    return RedirectToPage("/Admin/SubjectManagement");
                }
            }
            catch (Exception ex)
            {
                // Handle exception appropriately
                return null;
            }
        }


    }
}
