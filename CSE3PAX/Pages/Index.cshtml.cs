using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace CSE3PAX.Pages
{
    // Allow anonymous users to view page
    [AllowAnonymous]

    public class IndexModel : PageModel
    {
        // Object to access application settings
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        /*
         Initialise IndexModel class
         Configuration object (ConnectionStrings) located in appsettings.json
         Exception thrown when DefaultConnect string is not found in file
         */
        public IndexModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        /*
         Login user error messages
         */

        [BindProperty]
        [Required(ErrorMessage = "Please enter your email address"), EmailAddress]
        public string Email { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; } = "";
        public string errorMessage { get; set; } = "";
        public string successMessage { get; set; } = "";

        public void OnGet()
        {
        }

        public void OnPost() { 
            // Check if ModelState is valid
            if (!ModelState.IsValid)
            {
              errorMessage = "Please enter valid user credentials";
                return;
            }

            // Try/Catch statement to connect to SQL database and check user credentials and access
            try {

                /*
                 Establish connection to the database
                 using statements automatically close the connection when it is out of scope
                 */
                using (SqlConnection connection = new SqlConnection(_connectionString)) {

                    // Open connection
                    connection.Open();

                    // Get user information based on provided email
                    string sql = "SELECT * FROM [Users] WHERE Email = @Email";

                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection)) { 

                        // Add email parameter to command
                        command.Parameters.AddWithValue("@Email", Email);

                        // Execute SQL query and get results
                        using(SqlDataReader reader = command.ExecuteReader())
                        {

                            // Check if there is a matching user in the database
                            if (reader.Read())
                            {

                                // If user exists, get user information from the database
                                int userID = reader.GetInt32(0);
                                string email = reader.GetString(1);
                                string hashedPassword = reader.GetString(2);
                                Guid userGuidObj = reader.GetGuid(3);
                                string userGuid = userGuidObj.ToString();
                                string firstName = reader.GetString(4);
                                string lastName = reader.GetString(5);
                                bool isAdministrator = reader.GetBoolean(6);
                                bool isManager = reader.GetBoolean(7);
                                bool isLecturer = reader.GetBoolean(8);
                                bool isPasswordResetRequired = reader.GetBoolean(9);
                                string createdOn = reader.GetDateTime(10).ToString("dd/MM/yyyy");

                                /*
                                 Password Verification
                                 */

                                // Hash the input password and the user guid
                                string CalculatedPassword = Security.HashSHA256(Password + userGuid);

                                // Compare calculated hash with hashed password from database
                                bool isPasswordCorrect = CalculatedPassword == hashedPassword;

                                // Debugging or logging
                                Console.WriteLine($"Input password: {Password}");
                                Console.WriteLine($"Database password hash: {hashedPassword}");
                                Console.WriteLine($"Calculated password hash: {CalculatedPassword}");
                                Console.WriteLine($"Password match: {isPasswordCorrect}");

                                // If the password is correct the result of the hash is the same as the one in the database 
                                if (isPasswordCorrect)
                                {
                                    // Successful password verifciation => initialize the session variables
                                    HttpContext.Session.SetInt32("UserID", userID);
                                    HttpContext.Session.SetString("Email", email);
                                    HttpContext.Session.SetString("FirstName", firstName);
                                    HttpContext.Session.SetString("LastName", lastName);
                                    HttpContext.Session.SetBoolean("isAdministrator", isAdministrator);
                                    HttpContext.Session.SetBoolean("isManager", isManager);
                                    HttpContext.Session.SetBoolean("isLecturer", isLecturer);
                                    HttpContext.Session.SetBoolean("isPasswordResetRequired", isPasswordResetRequired);
                                    HttpContext.Session.SetString("createdOn", createdOn);

                                    // If statement to check user type
                                    if (isAdministrator)
                                    {
                                        Response.Redirect("/Admin/AdminIndex");
                                    }
                                    else if (isManager)
                                    {
                                        Response.Redirect("/Manager/ManagerIndex");
                                    }
                                    else if (isLecturer)
                                    {
                                        Response.Redirect("/Lecturer/LecturerIndex");
                                    }
                                }
                            }
                            // Close reader
                            reader.Close();
                        }
                    }
                }
            }

            // SQLException
            catch (SqlException sqlEx){
                Console.WriteLine($"SQL Error accessing the 'Users' table: {sqlEx.Message}");
                errorMessage = "An error occurred while accessing the database.";
            }

            // InvalidOperationException
            catch (InvalidOperationException invalidOpEx){
                // Handle InvalidOperationException
                Console.WriteLine($"Invalid Operation accessing the 'Users' table: {invalidOpEx.Message}");
                errorMessage = "An error occurred during the operation. Please try again.";
            }

            // UnauthorisedAccessException
            catch (UnauthorizedAccessException){
                errorMessage = "Wrong email or password";
                return;
            }

            // Incorrect email and password
            catch (Exception ex){
                errorMessage = ex.Message;
                return;
            }
            errorMessage = "Wrong email or password";
        }
    }
}