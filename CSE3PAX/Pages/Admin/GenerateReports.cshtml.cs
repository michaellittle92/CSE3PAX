using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Admin
{
    //Checking for required Roles
    [RequireRoles("Admin")]

    public class GenerateReportsModel : PageModel
    {
        // Object to access application settings
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        // List to store User information
        public List<User> Users { get; set; } = new List<User>();

        // List to store Subject information
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        // List to store Lecturer information
        public List<SubjectInstance> SubjectInstances { get; set; } = new List<SubjectInstance>();

        // User class to store User variable information
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

        // Subject class to store Subject variable information
        public class Subject
        {
            public int SubjectId { get; set; }
            public string SubjectCode { get; set; }
            public string SubjectName { get; set; }
            public string SubjectClassification { get; set; }
            public int YearLevel { get; set; }

        }

        // Subject instance class to store instance information
        public class SubjectInstance
        {
            public int SubjectInstanceId { get; set; }
            public int SubjectId { get; set; }
            public string SubjectInstanceName { get; set; }
            public string SubjectInstanceCode { get; set; }
            public int LecturerId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int SubjectInstanceYear { get; set; }
            public string SubjectCode { get; set; }
        }

        /*
         Initialise IndexModel class
         Configuration object (ConnectionStrings) located in appsettings.json
         Exception thrown when DefaultConnect string is not found in file
         */
        public GenerateReportsModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

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
                case "userManagement":
                    return RedirectToPage("/Admin/StaffManagement");
                case "subjects":
                    LoadSubjects();
                    break;
                case "subjectManagement":
                    return RedirectToPage("/Admin/SubjectManagement");
                case "instanceManagement":
                    return RedirectToPage("/Admin/CreateSubjectInstance");
                case "subjectInstances":
                    LoadSubjectInstances();
                    break;
                case "hideUsers":
                    HideUsers();
                    break;
                case "hideSubjects":
                    HideSubjects();
                    break;
                case "hideSubjectInstances":
                    HideSubjectInstances();
                    break;
                default:
                    break;
            }
            return Page();
        }

        // Method to hide users
        private void HideUsers() {
            Users.Clear();
        }

        // Method to hide subjects
        private void HideSubjects()
        {
            Subjects.Clear();
        }

        // Method to hide subject instances
        private void HideSubjectInstances()
        {
            SubjectInstances.Clear();
        }

        // Method to get all user information from database
        private void LoadUsers()
        {
            // Retrieve the list of users from the database
            try
            {
                // Clear the existing list of users
                Users.Clear();

                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
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
                            // Iterate through the results and add users to the list
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

                // Sort users by UserType
                Users = Users.OrderBy(u => u.UserType).ToList();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error retrieving users: " + ex.Message);
            }
        }

        // Method to load all subjects
        private void LoadSubjects()
        {
            // Retrieve the list of subjects from the database
            try
            {
                // Clear the existing list of subjects
                Subjects.Clear();

                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL query to select all subjects
                    string sql = "SELECT SubjectID, SubjectCode, SubjectName, SubjectClassification, YearLevel FROM Subjects";

                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Execute SQL query and get results
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Iterate through the results and add subjects to the list
                            while (reader.Read())
                            {
                                // Save subject information to Subject object
                                var subject = new Subject
                                {
                                    SubjectId = reader.GetInt32(0),
                                    SubjectCode = reader.GetString(1),
                                    SubjectName = reader.GetString(2),
                                    SubjectClassification = reader.GetString(3),
                                    YearLevel = reader.GetInt32(4),
                                };
                                // Add subjects
                                Subjects.Add(subject);
                            }
                        }
                    }
                }

                // Sort subjects by YearLevel
                Subjects = Subjects.OrderBy(s => s.YearLevel).ToList();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error retrieving subjects: " + ex.Message);
            }
        }


        // Method to get subject instance information from the database
        private void LoadSubjectInstances()
        {

            try
            {
                // Clear the existing list of subject instances
                SubjectInstances.Clear();

                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL query to select all subject instances
                    string sql = "SELECT si.SubjectInstanceId, si.SubjectId, si.SubjectInstanceName, si.SubjectInstanceCode, si.StartDate, si.EndDate, si.LecturerId, si.SubjectInstanceYear, s.SubjectCode " +
                        "FROM [SubjectInstance] si " +
                        "INNER JOIN [Subjects] s ON si.SubjectId = s.SubjectId";

                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Execute SQL query and get results
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Iterate through the results and add subject instances to the list
                            while (reader.Read())
                            {
                                // Create a new SubjectInstance object and add it to the list
                                var subjectInstance = new SubjectInstance
                                {
                                    SubjectInstanceId = reader.GetInt32(0),
                                    SubjectId = reader.GetInt32(1),
                                    SubjectInstanceName = reader.GetString(2),
                                    SubjectInstanceCode = reader.GetString(3),
                                    StartDate = reader.GetDateTime(4),
                                    EndDate = reader.GetDateTime(5),
                                    LecturerId = reader.GetInt32(6),
                                    SubjectInstanceYear = reader.GetInt32(7),
                                    SubjectCode = reader.GetString(8)
                                };
                                // Add subject instance to the list
                                SubjectInstances.Add(subjectInstance);
                            }
                        }
                    }
                }
                // Sort subjects by YearLevel
                SubjectInstances = SubjectInstances.OrderBy(s => s.SubjectCode).ToList();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error retrieving subject instances: " + ex.Message);
            }
        }

        // Method to sort lecturers by UserID
        private void SortUsersByUserId()
        {
            Users = Users.OrderBy(user => user.UserId).ToList();
        }
    }
}