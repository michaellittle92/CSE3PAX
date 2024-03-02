using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Admin
{
    //Checking for required Roles
    [RequireRoles("Admin")]
    public class EditUserModel : PageModel
    {
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

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string getUserDataSQLQuery = "SELECT UserId, FirstName, LastName FROM Users WHERE Email = @Email";
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

                                Console.WriteLine($"{UserId}, {Email}, {FirstName}, {LastName}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return to the page to display validation errors
                return Page();
            }

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
                        // Add SQL parameters for the query
                        command.Parameters.AddWithValue("@FirstName", FirstName);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@UserId", UserId);

                        int result = command.ExecuteNonQuery();

                        //Check to see if update is successful
                        if (result == 1)
                        {

                            return RedirectToPage("/Admin/ReadUser");
                        }
                        else
                        {

                            Console.WriteLine("", "User could not be updated.");
                            return Page();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
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
                return RedirectToPage("/Admin/ReadUser");
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
        
    

