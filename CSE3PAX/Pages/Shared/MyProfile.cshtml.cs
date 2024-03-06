using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Shared
{
    public class MyProfileModel : PageModel {

        // Object to access application settings
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public string Email { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; }
        public string UserImage { get; set; }
        public string Expertise01 { get; set; }
        public string Expertise02 { get; set; }
        public string Expertise03 { get; set; }
        public string Expertise04 { get; set; }
        public string Expertise05 { get; set; }
        public string Expertise06 { get; set; }
        public decimal? ConcurrentLoadCapacity { get; set; }

        /*
         Initialise IndexModel class
         Configuration object (ConnectionStrings) located in appsettings.json
         Exception thrown when DefaultConnect string is not found in file
         */
        public MyProfileModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }


        public void OnGet()
        {
            Email = HttpContext.Session.GetString("Email");
            UserID = (int)HttpContext.Session.GetInt32("UserID");
            FirstName = HttpContext.Session.GetString("FirstName");
            LastName = HttpContext.Session.GetString("LastName");
            FullName = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");

            // Retrieve user type flags from session
            bool isAdministrator = HttpContext.Session.GetBoolean("isAdministrator");
            bool isManager = HttpContext.Session.GetBoolean("isManager");
            bool isLecturer = HttpContext.Session.GetBoolean("isLecturer");

            // Determine user type based on the flags
            if (isAdministrator)
            {
                UserType = "Administrator";
                UserImage = "https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava3.webp";
            }
            else if (isManager)
            {
                UserType = "Manager";
                UserImage = "https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava5.webp";
            }
            else if (isLecturer)
            {
                UserType = "Lecturer";
                UserImage = "https://mdbcdn.b-cdn.net/img/Photos/new-templates/bootstrap-chat/ava6.webp";
                GetLecturerDetails();
            }
            else
            {
                UserType = "Unknown"; // Default value if none of the flags are set
            }
        }

        private void GetLecturerDetails() {

            Console.WriteLine("Get lecturer details for userID " + UserID);

            // Retrieve the list of lecturers from the database
            try
            {
                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL query to select the lecturer details for the given UserID
                    string sql = "SELECT u.UserId, u.FirstName, u.LastName, u.Email, " +
                                 "l.Expertise01, l.Expertise02, l.Expertise03, " +
                                 "l.Expertise04, l.Expertise05, l.Expertise06, " +
                                 "l.ConcurrentLoadCapacity " +
                                 "FROM [Users] u " +
                                 "INNER JOIN [Lecturers] l ON u.UserId = l.UserId " +
                                 "WHERE u.UserId = @UserID";  // Filtering by UserID

                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameter for UserID
                        command.Parameters.AddWithValue("@UserID", UserID);

                        // Execute SQL query and get results
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if there is a result
                            if (reader.Read())
                            {
                                // Save lecturer information to Lecturer object
                                Expertise01 = reader.IsDBNull(4) ? null : reader.GetString(4);
                                Expertise02 = reader.IsDBNull(5) ? null : reader.GetString(5);
                                Expertise03 = reader.IsDBNull(6) ? null : reader.GetString(6);
                                Expertise04 = reader.IsDBNull(7) ? null : reader.GetString(7);
                                Expertise05 = reader.IsDBNull(8) ? null : reader.GetString(8);
                                Expertise06 = reader.IsDBNull(9) ? null : reader.GetString(9);
                                ConcurrentLoadCapacity = reader.IsDBNull(10) ? null : (decimal?)reader.GetDecimal(10);
                            }
                            else
                            {
                                // No lecturer found with the given UserID
                                Console.WriteLine("No lecturer found with UserID: " + UserID);
                            }

                            Console.WriteLine(Expertise01);
                            Console.WriteLine(Expertise02);
                            Console.WriteLine(Expertise03);
                            Console.WriteLine(Expertise04);
                            Console.WriteLine(Expertise05);
                            Console.WriteLine(Expertise06);
                            Console.WriteLine(ConcurrentLoadCapacity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error retrieving lecturer details: " + ex.Message);
            }
        }
    }
    }