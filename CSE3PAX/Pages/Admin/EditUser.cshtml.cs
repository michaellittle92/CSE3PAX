using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CSE3PAX.Pages.Admin
{
    //Checking for required Roles
    [RequireRoles("Admin")]
    public class EditUserModel : PageModel
    {

        // Alert message variable
        public string SuccessMessage { get; set; }

        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public EditUserModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        // User bind properties
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }
        [BindProperty]
        public string UserId { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        [BindProperty]
        public string LastName { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public bool IsLecturer { get; set; }
        [BindProperty]
        public string Expertise01 { get; set; }
        [BindProperty]
        public string Expertise02 { get; set; }
        [BindProperty]
        public string Expertise03 { get; set; }
        [BindProperty]
        public string Expertise04 { get; set; }
        [BindProperty]
        public string Expertise05 { get; set; }
        [BindProperty]
        public string Expertise06 { get; set; }
        [BindProperty]
        public double WorkHours { get; set; }
        [BindProperty]
        public string ResetPasswordEmail { get; set; }

        /*
        Handles the HTTP GET request for the HoursAndLoadConversion page.
        - Opens a connection to the database.
        - Executes a SQL query to retrieve user data based on the provided email.
        - If a result is found, populates the properties UserId, FirstName, LastName, and IsLecturer.
        - Sets the ResetPasswordEmail property to the provided email.
        - If the user is a lecturer:
            - Executes a SQL query to retrieve lecturer expertise and concurrent load capacity.
            - Populates properties Expertise01 to Expertise06 and calculates the work hours based on the concurrent load capacity.
        */
        public void OnGet(HoursAndLoadConversion hoursAndLoadConversion)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    /*
                    SQL query to retrieve user data based on the provided email address.
                    - Retrieves the UserId, FirstName, LastName, and IsLecturer columns from the Users table.
                    - Filters the data based on the provided email address parameter.
                    */
                    string getUserDataSQLQuery = "SELECT UserId, FirstName, LastName, IsLecturer FROM Users WHERE Email = @Email";

                    using (SqlCommand command = new SqlCommand(getUserDataSQLQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Email", Email);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // If there's a result, populate the properties
                            {

                                UserId = reader["UserId"].ToString();
                                FirstName = reader["FirstName"].ToString();
                                LastName = reader["LastName"].ToString();
                                IsLecturer = Convert.ToBoolean(reader["IsLecturer"]);

                                ResetPasswordEmail = Email;

                                reader.Close();
                                if (IsLecturer) {

                                    /*
                                    SQL query to retrieve expertise information for a lecturer based on the provided user ID.
                                    - Retrieves the ConcurrentLoadCapacity, Expertise01, Expertise02, Expertise03, Expertise04, Expertise05, and Expertise06 columns from the Lecturers table.
                                    - Filters the data based on the provided user ID parameter.
                                    */
                                    string getExpertiseSQLQuery = "SELECT ConcurrentLoadCapacity, Expertise01, Expertise02, Expertise03, Expertise04, Expertise05, Expertise06 FROM Lecturers WHERE UserID = @UserID";

                                    using (SqlCommand expertiseCommand = new SqlCommand(getExpertiseSQLQuery, connection))
                                    {
                                        expertiseCommand.Parameters.AddWithValue("@UserID", UserId);

                                        using (SqlDataReader expertiseReader = expertiseCommand.ExecuteReader())
                                        {
                                            if (expertiseReader.Read())
                                            {
                                                Expertise01 = expertiseReader["Expertise01"].ToString();
                                                Expertise02 = expertiseReader["Expertise02"].ToString();
                                                Expertise03 = expertiseReader["Expertise03"].ToString();
                                                Expertise04 = expertiseReader["Expertise04"].ToString();
                                                Expertise05 = expertiseReader["Expertise05"].ToString();
                                                Expertise06 = expertiseReader["Expertise06"].ToString();
                                                double concurrentLoadCapacity = Convert.ToDouble(expertiseReader["ConcurrentLoadCapacity"]);
                                                WorkHours = HoursAndLoadConversion.CalculateWorkHours(concurrentLoadCapacity);
                                                expertiseReader.Close();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {

                Debug.WriteLine(ex);
            }
        }

        /*
        Handles the HTTP POST request for editing user information.
        - Retrieves the load capacity based on the provided work hours using the HoursAndLoadConversion class.
        - Opens a connection to the database.
        - Executes a SQL query to update user information (FirstName, LastName, Email) in the Users table.
        - If the user is a lecturer, updates the lecturer's expertise and concurrent load capacity in the Lecturers table.
        - Sets the success message if the user is edited successfully.
        - Catches any exceptions and adds a model error if an error occurs during processing.
        */
        public IActionResult OnPost()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    double LoadCapacity = HoursAndLoadConversion.CalculateLoadCapacity(Convert.ToInt32(WorkHours));
                    Debug.WriteLine( "Load Capactiy " + LoadCapacity);

                    connection.Open();

                    /*
                    SQL query to update user data in the Users table.
                    - Sets the FirstName, LastName, and Email columns based on the provided parameters.
                    - Filters the update operation based on the UserId parameter.
                    */
                    string updateUserDataSQLQuery = @"
                        UPDATE Users 
                        SET FirstName = @FirstName, LastName = @LastName, Email = @Email 
                        WHERE UserId = @UserId";

                    using (SqlCommand command = new SqlCommand(updateUserDataSQLQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", FirstName);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@UserId", UserId);

                        int result = command.ExecuteNonQuery();

                        // Set the success message
                        SuccessMessage = "User edited successfully.";
                        Debug.WriteLine(FirstName);

                        if (result >= 1 && IsLecturer)
                        {

                            /*
                            SQL query to update expertise data in the Lecturers table.
                            - Sets the Expertise01, Expertise02, Expertise03, Expertise04, Expertise05, Expertise06, and ConcurrentLoadCapacity columns based on the provided parameters.
                            - Filters the update operation based on the UserId parameter.
                            */
                            string updateExpertiseSQLQuery = @"
                                UPDATE Lecturers
                                SET Expertise01 = @Expertise01, Expertise02 = @Expertise02, 
                                    Expertise03 = @Expertise03, Expertise04 = @Expertise04, 
                                    Expertise05 = @Expertise05, Expertise06 = @Expertise06,
                                    ConcurrentLoadCapacity = @ConcurrentLoadCapacity
                                WHERE UserId = @UserId";

                            using (SqlCommand expertiseCommand = new SqlCommand(updateExpertiseSQLQuery, connection))
                            {
                                expertiseCommand.Parameters.AddWithValue("@Expertise01", Expertise01 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise02", Expertise02 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise03", Expertise03 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise04", Expertise04 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise05", Expertise05 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise06", Expertise06 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@ConcurrentLoadCapacity", LoadCapacity);
                                expertiseCommand.Parameters.AddWithValue("@UserId", UserId);
                                expertiseCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ModelState.AddModelError("", "An error occurred while processing your request.");
            }
            return Page();
        }

        /*
        Handles the HTTP POST request for deleting a user asynchronously.
        - Retrieves the UserID and IsLecturer status based on the provided email from the Users table.
        - Deletes the user's entry from the Users table.
        - If the user is a lecturer, deletes the lecturer-specific entry from the Lecturers table first to maintain referential integrity.
        - Redirects to the staff management page after successful deletion.
        - Catches any exceptions and returns the current page if an error occurs during processing.
        */
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            System.Diagnostics.Debug.WriteLine($"Email: {Email}");
            try
            {
                /*
                SQL script to delete a user from the Users table based on the provided email parameter.
                - Retrieves the UserID and IsLecturer status based on the email.
                - If the user is a lecturer (IsLecturer = 1), deletes the lecturer-specific entry from the Lecturers table first to maintain referential integrity, then deletes the user from the Users table.
                - If the user is not a lecturer, deletes the user directly from the Users table.
                */
                var sql = @"
                    DECLARE @Email VARCHAR(255);
                    DECLARE @UserID INT;
                    DECLARE @IsLecturer BIT;

                    SET @Email = @EmailParam; 

                    -- Retrieve UserID and IsLecturer status based on the email
                    SELECT @UserID = UserID, @IsLecturer = IsLecturer FROM Users WHERE Email = @Email;

                    -- Check if the user is a lecturer
                    IF @IsLecturer = 1
                    BEGIN
                        -- Delete the lecturer-specific entry first to maintain referential integrity
                        DELETE FROM Lecturers WHERE UserID = @UserID;

                        -- Then delete the user from the Users table
                        DELETE FROM Users WHERE UserID = @UserID;
                    END
                    ELSE
                    BEGIN
                        -- If not a lecturer, delete the user directly
                        DELETE FROM Users WHERE UserID = @UserID;
                    END";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Parameterize the email address to avoid SQL injection
                        command.Parameters.AddWithValue("@EmailParam", Email);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return RedirectToPage("/Admin/StaffManagement");
            }
            catch (Exception ex)
            {
                // Log or handle the error as needed
                return Page();
            }
        }

        /*
        Handles the HTTP POST request for resetting a user's password asynchronously.
        - Validates the email address and password.
        - Checks password complexity requirements: Minimum 8 characters, at least one number, and one special character.
        - Generates a new unique user GUID.
        - Hashes the new password with the user GUID using SHA256 algorithm.
        - Updates the user's password in the database.
        - Redirects to the page showing the reset password message if successful.
        - Catches any exceptions and redirects to the page with an error message if an error occurs during processing.
        */
        public async Task<IActionResult> OnPostResetPasswordAsync()
        {
            if (string.IsNullOrEmpty(ResetPasswordEmail))
            {
                ModelState.AddModelError("", "Email address is required.");
                return Page();
            }

            if (string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("Password", "Password cannot be empty.");
                return RedirectToPage(new { email = ResetPasswordEmail });
            }

            // Password complexity requirements: Minimum 8 characters, at least one number, and one special character
            var passwordPattern = new Regex(@"^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{8,}$");
            if (!passwordPattern.IsMatch(Password))
            {
                ModelState.AddModelError("Password", "Password must be at least 8 characters long and include at least one number and one special character.");
                return RedirectToPage(new { email = ResetPasswordEmail });
            }

            try
            {
                string userGuid = Guid.NewGuid().ToString();
                string newPasswordHash = Security.HashSHA256(Password + userGuid);

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    /*
                    SQL script to update the password and user GUID for a user in the Users table based on the provided UserID.
                    - Begins a transaction to ensure atomicity.
                    - Checks if a user with the provided UserID exists in the Users table.
                    - If the user exists, updates the Password and UserGuid fields with the new values.
                    - Commits the transaction to save changes.
                    */
                    var sql = @"
                        BEGIN TRANSACTION;
                        IF EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId)
                        BEGIN
                            UPDATE Users 
                            SET Password = @NewPasswordHash, UserGuid = @UserGuid
                            WHERE UserId = @UserId;
                        END
                        COMMIT TRANSACTION;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);
                        command.Parameters.AddWithValue("@UserId", UserId);
                        command.Parameters.AddWithValue("@UserGuid", userGuid);

                        int affectedRows = await command.ExecuteNonQueryAsync();
                        if (affectedRows == 0)
                        {
                            ModelState.AddModelError("", "User not found or password could not be updated.");
                            return RedirectToPage(new { email = this.Email });
                        }
                    }
                }
                TempData["Message"] = "Password has been reset successfully.";
                return RedirectToPage(new { email = this.Email });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while resetting the password.");
                return RedirectToPage(new { email = this.Email });
            }
        }
    }
}