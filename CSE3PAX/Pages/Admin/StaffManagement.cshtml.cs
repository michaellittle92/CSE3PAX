using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static CSE3PAX.Pages.Admin.GenerateReportsModel;

namespace CSE3PAX.Pages.Admin
{

    //Checking for required Roles
    [RequireRoles("Admin")]

    public class StaffManagementModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public StaffManagementModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }


        // List to store User information
        public List<User> Users { get; set; } = new List<User>();

        public class User
        {
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string UserType { get; set; }
            public string Expertise01 { get; set; }
            public string Expertise02 { get; set; }
            public string Expertise03 { get; set; }
            public string Expertise04 { get; set; }
            public string Expertise05 { get; set; }
            public string Expertise06 { get; set; }
            public decimal? ConcurrentLoadCapacity { get; set; }
        }


        /*
        Initialise IndexModel class
        Configuration object (ConnectionStrings) located in appsettings.json
        Exception thrown when DefaultConnect string is not found in file
        */

        public void OnGet()
        {
        }

        /*
        onPost checks each button pressed to generate report information
        Switch case is used to call the method required.
        */
        public IActionResult OnPost(string buttonType)
        {
            switch (buttonType)
            {
                case "users":
                    LoadUsers();
                    SortUsersByUserId();
                    break;
            }
            return Page();
        }

        // Load Users Methods
        private void LoadUsers()
        {
            //console write for testing
            Console.WriteLine("Generate Users");

            //Retrieve the list of users from the database
            try
            {
                // Clear the existing list of users
                Users.Clear();

                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    //Open connection
                    connection.Open();

                    // SQL query to select all users who are lecturers
                    string sql = "SELECT u.UserId, u.FirstName, u.LastName, u.Email, " +
                         "l.Expertise01, l.Expertise02, l.Expertise03, " +
                         "l.Expertise04, l.Expertise05, l.Expertise06, " +
                         "l.ConcurrentLoadCapacity, u.isAdmin, u.isManager, u.isLecturer " +
                         "FROM [Users] u " +
                         "LEFT JOIN [Lecturers] l ON u.UserId = l.UserId ";

                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Execute SQL query and get results
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //Iterate through the results and add users to the list
                            while (reader.Read())
                            {
                                // Save user information to User object
                                var user = new User
                                {
                                    UserId = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    Expertise01 = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    Expertise02 = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    Expertise03 = reader.IsDBNull(6) ? null : reader.GetString(6),
                                    Expertise04 = reader.IsDBNull(7) ? null : reader.GetString(7),
                                    Expertise05 = reader.IsDBNull(8) ? null : reader.GetString(8),
                                    Expertise06 = reader.IsDBNull(9) ? null : reader.GetString(9),
                                    ConcurrentLoadCapacity = reader.IsDBNull(10) ? null : (decimal?)reader.GetDecimal(10)
                                };

                                // Determine UserType based on isAdmin, isManager, and isLecturer 
                                if (reader.GetBoolean(11))
                                    user.UserType = "Administrator";
                                else if (reader.GetBoolean(12))
                                    user.UserType = "Manager";
                                else if (reader.GetBoolean(13))
                                    user.UserType = "Lecturer";

                                // Add users
                                Users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error retrieving users: " + ex.Message);
            }
        }

        //Method to sort lecturers by UserID
        private void SortUsersByUserId()
        {
            Users = Users.OrderBy(user => user.UserId).ToList();
        }
    }
}