using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Admin
{
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
    }
}
        
    

