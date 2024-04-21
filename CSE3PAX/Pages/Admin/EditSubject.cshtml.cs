using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Admin
{
    // Checking for required Roles
    [RequireRoles("Admin")]

    /*
     The EditSubjectModel class represents the backend logic for editing subjects 
     within the admin dashboard. It handles user authorization, data retrieval, and database 
     operations for updating and deleting subjects.
     */
    public class EditSubjectModel : PageModel
    {
        // Alert message variable
        public string SuccessMessage { get; set; }
        public string DeleteMessage { get; set; }

        // Object to access application settings
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        /*
        The constructor initializes a new instance of the EditSubjectModel class.
        It requires an IConfiguration object to access application settings.
        If a valid configuration is not provided, it throws an ArgumentNullException.
        It retrieves the connection string named "DefaultConnection" from the configuration.
        If the connection string is not found, it throws an InvalidOperationException.
        */
        public EditSubjectModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        // Binding properties
        [BindProperty(SupportsGet = true)]
        public string SubjectCode { get; set; }
        [BindProperty]
        public string SubjectName { get; set; }
        [BindProperty]
        public string SubjectClassification { get; set; }
        [BindProperty]
        public string YearLevel { get; set; }

        // Handles HTTP GET requests
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
                            // If there's a result, populate the properties
                            if (reader.Read()) 
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

        // Handles HTTP POST requests for updating subject data
        public IActionResult OnPost()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    /*
                     The SQL query string updates the Subjects table by setting the SubjectName, SubjectClassification, and YearLevel columns
                     based on the provided parameters. It updates the row where the SubjectCode matches the given value.
                     */
                    string updateSubjectDataSQLQuery = "UPDATE Subjects SET SubjectName = @SubjectName, SubjectClassification = @SubjectClassification, YearLevel = @YearLevel WHERE SubjectCode = @SubjectCode";

                    using (SqlCommand command = new SqlCommand(updateSubjectDataSQLQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                        command.Parameters.AddWithValue("@SubjectName", SubjectName);
                        command.Parameters.AddWithValue("@SubjectClassification", SubjectClassification);
                        command.Parameters.AddWithValue("@YearLevel", YearLevel);

                        command.ExecuteNonQuery();

                        // Set success message after user creation
                        TempData["SuccessMessage"] = "Subject edited successfully.";

                        // Redirect
                        return RedirectToPage("/Admin/SubjectManagement");
                    }
                }
            }
            catch (Exception ex) { 
                return null; 
            }
        }

        // Handles HTTP POST requests for deleting subjects
        public IActionResult OnPostDelete()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    /*
                     The SQL query string deletes records from the SubjectInstance table where the SubjectID matches any of the SubjectID values
                     obtained from the Subjects table based on the provided SubjectCode parameter.
                     */
                    string deleteInstancesQuery = "DELETE FROM SubjectInstance WHERE SubjectID IN (SELECT SubjectID FROM Subjects WHERE SubjectCode = @SubjectCode)";

                    using (SqlCommand deleteInstancesCommand = new SqlCommand(deleteInstancesQuery, connection))
                    {
                        deleteInstancesCommand.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                        deleteInstancesCommand.ExecuteNonQuery();
                    }

                    // The SQL query string deletes records from the Subjects table where the SubjectCode matches the provided SubjectCode parameter.
                    string deleteSubjectDataSQLQuery = "DELETE FROM Subjects WHERE SubjectCode = @SubjectCode";

                    using (SqlCommand deleteSubjectCommand = new SqlCommand(deleteSubjectDataSQLQuery, connection))
                    {
                        deleteSubjectCommand.Parameters.AddWithValue("@SubjectCode", SubjectCode);
                        deleteSubjectCommand.ExecuteNonQuery();
                    }

                    // Set success message in TempData
                    TempData["DeleteMessage"] = "Subject deleted successfully.";

                    Console.WriteLine("Subject deleted");

                    // Redirect to the desired page after successful deletion
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
