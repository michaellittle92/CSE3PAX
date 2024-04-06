using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;

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

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
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

                                reader.Close();
                                if (IsLecturer) { 
                                
                                    string getExpertiseSQLQuery = "SELECT Expertise01, Expertise02, Expertise03, Expertise04, Expertise05, Expertise06 FROM Lecturers WHERE UserID = @UserID";
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

                                                Debug.WriteLine(Expertise06);
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

        public IActionResult OnPost()
        {
            string logMessage = $"UserId: {UserId}, " +
                        $"Email: {Email}, " +
                        $"FirstName: {FirstName}, " +
                        $"LastName: {LastName}, " +
                        $"IsLecturer: {IsLecturer}";

            if (IsLecturer)
            {
                logMessage += $", Expertise01: {Expertise01}, " +
                              $"Expertise02: {Expertise02}, " +
                              $"Expertise03: {Expertise03}, " +
                              $"Expertise04: {Expertise04}, " +
                              $"Expertise05: {Expertise05}, " +
                              $"Expertise06: {Expertise06}";
            }

            Debug.WriteLine(logMessage);

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
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
                            string updateExpertiseSQLQuery = @"
                    UPDATE Lecturers
                    SET Expertise01 = @Expertise01, Expertise02 = @Expertise02, 
                        Expertise03 = @Expertise03, Expertise04 = @Expertise04, 
                        Expertise05 = @Expertise05, Expertise06 = @Expertise06
                    WHERE UserId = @UserId";

                            using (SqlCommand expertiseCommand = new SqlCommand(updateExpertiseSQLQuery, connection))
                            {
                                expertiseCommand.Parameters.AddWithValue("@Expertise01", Expertise01 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise02", Expertise02 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise03", Expertise03 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise04", Expertise04 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise05", Expertise05 ?? string.Empty);
                                expertiseCommand.Parameters.AddWithValue("@Expertise06", Expertise06 ?? string.Empty);
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

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            System.Diagnostics.Debug.WriteLine($"Email: {Email}");
            try
            {
                // SQL script with parameterized email
                var sql = @"
            DECLARE @Email VARCHAR(255);
            DECLARE @UserID INT;
            DECLARE @IsLecturer BIT;

            SET @Email = @EmailParam; -- Use parameterized value here

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

        public async Task<IActionResult> OnPostResetPasswordAsync()
        {
            try
            {
                // Generate a new GUID
                string userGuid = Guid.NewGuid().ToString();

                string newPasswordHash = Security.HashSHA256(Password + userGuid);

                // Update the password and userGuid in the database for the specified user
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var sql = @"
                UPDATE Users 
                SET Password = @NewPasswordHash,
                    UserGuid = @UserGuid
                WHERE UserId = @UserId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);
                        command.Parameters.AddWithValue("@UserId", UserId);
                        command.Parameters.AddWithValue("@UserGuid", userGuid);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                // Optionally, add a success message or log the password reset event
                TempData["Message"] = "Password has been reset successfully.";

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Log the error or handle it as needed
                ModelState.AddModelError("", "An error occurred while resetting the password.");
                return Page();
            }
        }


    }
}
        
    

