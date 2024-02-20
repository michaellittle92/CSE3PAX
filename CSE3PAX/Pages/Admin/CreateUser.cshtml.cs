using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace CSE3PAX.Pages.Admin
{
    //Checking for required Roles
    [RequireRoles("Admin")]

    public class CreateUserModel : PageModel
    {

        // Object to access application settings
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;


        public CreateUserModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }


        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public bool IsAdmin { get; set; }

        [BindProperty]
        public bool IsManager { get; set; }

        [BindProperty]
        public bool IsLecturer { get; set; }

        [BindProperty]
        public bool TestCheck { get; set; }

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
        public int workHours { get; set; }

        public List<string> SubjectClassifications { get; set; } = new List<string>();


        public void OnGet()
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL query to get distinct Subject Classifications
                    string populateDDLs = "SELECT DISTINCT SubjectClassification FROM Subjects";

                    // Execute the query
                    using (SqlCommand command = new SqlCommand(populateDDLs, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Loop through the results
                            while (reader.Read())
                            {

                                string classification = reader.GetString(0); // Get the first column value in each row
                                SubjectClassifications.Add(classification);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void OnPost()
        {
            
            int userID = 0;
           string userGuid = GenerateGUID.CreateNewGuid();
           string CalculatedPassword = Security.HashSHA256(Password + userGuid);
           
            /*
            Console.WriteLine($"First Name: {FirstName}");
            Console.WriteLine($"Last Name: {LastName}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Password: {Password}");
            Console.WriteLine($"Is Administrator: {IsAdmin}");
            Console.WriteLine($"Is Manager: {IsManager}");
            Console.WriteLine($"Is Lecturer: {IsLecturer}");
            Console.WriteLine($"WorkHours: {CalculatedLoadCapacity}");
            Console.WriteLine($"Expertise01: {Expertise01}");
            Console.WriteLine($"Expertise02: {Expertise02}");
            */

            if (IsLecturer)
            {
                try {
                    double CalculatedLoadCapacity = CalculateLoadCapacity(workHours);

                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {

                        // Open connection
                        connection.Open();

                        // Insert User Information into Users table
                        string insertIntoUsersTable = "INSERT INTO [Users] (Email, Password, UserGuid, FirstName, LastName, IsAdmin, IsManager, IsLecturer, IsPasswordResetRequired) VALUES (@Email, @Password, @UserGuid, @FirstName, @LastName, @IsAdmin, @IsManager, @IsLecturer, @IsPasswordResetRequired)\r\n";
                        //
                        string getUserIDFromEmail = "Select userID FROM [Users] WHERE Email = @Email";

                        string insertIntoLecturersTable = "INSERT INTO [Lecturers](UserID, Expertise01, Expertise02, Expertise03, Expertise04, Expertise05, Expertise06, ConcurrentLoadCapacity) VALUES (@UserID, @Expertise01, @Expertise02, @Expertise03, @Expertise04, @Expertise05, @Expertise06, @ConcurrentLoadCapacity)";

                        // SQL command object with query and connection
                        using (SqlCommand command = new SqlCommand(insertIntoUsersTable, connection))
                        {

                            // Add parameters to command
                            command.Parameters.AddWithValue("@Email", Email);
                            command.Parameters.AddWithValue("Password", CalculatedPassword);
                            command.Parameters.AddWithValue("@UserGuid", userGuid);
                            command.Parameters.AddWithValue("@FirstName", FirstName);
                            command.Parameters.AddWithValue("@LastName", LastName);
                            command.Parameters.AddWithValue("@IsAdmin", IsAdmin);
                            command.Parameters.AddWithValue("@IsManager", IsManager);
                            command.Parameters.AddWithValue("@IsLecturer", IsLecturer);
                            command.Parameters.AddWithValue("@IsPasswordResetRequired", false);

                            // Execute SQL query 
                            command.ExecuteNonQuery();

                        }
                    using (SqlCommand command = new SqlCommand(getUserIDFromEmail, connection))
                        {
                            command.Parameters.AddWithValue("@Email", Email);

                            using (SqlDataReader reader = command.ExecuteReader())
                                if (reader.Read())
                                { //if there is at least one row
                                    userID = reader.GetInt32(0);
                                }

                        }

                        using (SqlCommand command = new SqlCommand(insertIntoLecturersTable, connection))
                        {
                            List<string> expertiseFields = new List<string>{Expertise01, Expertise02, Expertise03, Expertise04, Expertise05, Expertise06};
                            command.Parameters.AddWithValue("@UserID", userID);
                            

                            // Loop through each expertise field
                            for (int i = 0; i < expertiseFields.Count; i++)
                            {
                                string parameterName = $"@Expertise0{i + 1}"; // Construct parameter name dynamically
                                command.Parameters.Add(parameterName, SqlDbType.VarChar);

                                if (string.IsNullOrEmpty(expertiseFields[i]))
                                    command.Parameters[parameterName].Value = DBNull.Value;
                                else
                                    command.Parameters[parameterName].Value = expertiseFields[i];
                            }

                            command.Parameters.AddWithValue("@ConcurrentLoadCapacity", CalculatedLoadCapacity);

                            // Execute SQL query
                            command.ExecuteNonQuery();
                        }


                    }
                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message);
                    Console.WriteLine($"SQL Error: {ex.Message}");

                }
            }
            else
            {
                try {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {

                        // Open connection
                        connection.Open();

                        // Get user information based on provided email
                        string insertIntoUsersTable = "INSERT INTO [Users] (Email, Password, UserGuid, FirstName, LastName, IsAdmin, IsManager, IsLecturer, IsPasswordResetRequired) VALUES (@Email, @Password, @UserGuid, @FirstName, @LastName, @IsAdmin, @IsManager, @IsLecturer, @IsPasswordResetRequired)\r\n";

                        // SQL command object with query and connection
                        using (SqlCommand command = new SqlCommand(insertIntoUsersTable, connection))
                        {

                            // Add email parameter to command
                            command.Parameters.AddWithValue("@Email", Email);
                            command.Parameters.AddWithValue("Password", CalculatedPassword);
                            command.Parameters.AddWithValue("@UserGuid", userGuid);
                            command.Parameters.AddWithValue("@FirstName", FirstName);
                            command.Parameters.AddWithValue("@LastName", LastName);
                            command.Parameters.AddWithValue("@IsAdmin", IsAdmin);
                            command.Parameters.AddWithValue("@IsManager", IsManager);
                            command.Parameters.AddWithValue("@IsLecturer", IsLecturer);
                            command.Parameters.AddWithValue("@IsPasswordResetRequired", false);

                            // Execute SQL query 
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }

                }
                catch (Exception ex)
                { Console.WriteLine(ex.ToString()); }
            }
         

        }

        private double CalculateLoadCapacity(double hours)
        {
            double loadCapacity = (6.0 / 38.0) * hours;

            //rounds to the nearest 10th place.
            loadCapacity = Math.Round(loadCapacity, 1, MidpointRounding.AwayFromZero);

            //Returns the smaller value
            return Math.Min(loadCapacity, 6);
        }
    }
}
