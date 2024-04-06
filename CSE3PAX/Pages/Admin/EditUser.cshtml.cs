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
        public string ResetPasswordEmail { get; set; }

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

                                ResetPasswordEmail = Email;

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
        
    

