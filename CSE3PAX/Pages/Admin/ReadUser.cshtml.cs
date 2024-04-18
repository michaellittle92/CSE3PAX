using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Admin
{
    public class ReadUserModel : PageModel
    {

        // Readonly field to store IConfiguration
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        // Constructor with IConfiguration parameter
        public ReadUserModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        // List of users
        public List<ListUsers> ListUsers { get; set; } = new List<ListUsers>();

        // Method executed when the page is loaded via HTTP GET request
        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // SQL query to retrieve user information
                    string sql = "SELECT Email, FirstName, LastName, CASE WHEN IsAdmin = 1 THEN 'Administrator' WHEN IsManager = 1 THEN 'Manager' WHEN IsLecturer = 1 THEN 'Lecturer' ELSE 'No role found' END AS Role FROM Users;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read())
                            {
                                var user = new ListUsers { 
                                    Email = reader["Email"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Role = reader["Role"].ToString(),
                                };
                                ListUsers.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
