using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static CSE3PAX.Pages.Manager.StaffSchedulesModel;
using System.Data.SqlClient;
using static CSE3PAX.Pages.Manager.GenerateReportsModel;
using System.Data;

namespace CSE3PAX.Pages.Manager
{

    //Checking for required Roles
    [RequireRoles("Manager")]

    public class GenerateReportsModel : PageModel
    {

        // Object to access application settings
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        // List to store Lecturer information
        public List<Lecturer> Lecturers { get; set; } = new List<Lecturer>();

        // List to store Lecturer information
        public List<SubjectInstance> SubjectInstances { get; set; } = new List<SubjectInstance>();

        // Lecturer class to store Lecturer variable information
        public class Lecturer
        {
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Expertise01 { get; set; }
            public string Expertise02 { get; set; }
            public string Expertise03 { get; set; }
            public string Expertise04 { get; set; }
            public string Expertise05 { get; set; }
            public string Expertise06 { get; set; }
            public decimal? ConcurrentLoadCapacity { get; set; }
        }

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
                case "lecturers":
                    LoadLecturers();
                    SortLecturersByUserId();
                    break;
                case "subjectInstances":
                    LoadSubjectInstances();
                    break;
                case "schedules":
                    LoadSchedules();
                    break;
                default:
                    break;
            }
            return Page();
        }

        // Method to get lecturer information from db
        private void LoadLecturers()
        {

            // Retrieve the list of lecturers from the database
            try
            {
                // Clear the existing list of lecturers
                Lecturers.Clear();

                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL query to select all users who are lecturers
                    string sql = "SELECT u.UserId, u.FirstName, u.LastName, u.Email, " +
                                 "l.Expertise01, l.Expertise02, l.Expertise03, " +
                                 "l.Expertise04, l.Expertise05, l.Expertise06, " +
                                 "l.ConcurrentLoadCapacity " +
                                 "FROM [Users] u " +
                                 "INNER JOIN [Lecturers] l ON u.UserId = l.UserId " +
                                 "WHERE u.isLecturer = 1";

                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Execute SQL query and get results
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Iterate through the results and add lecturers to the list
                            while (reader.Read())
                            {
                                // Save lecturer information to Lecturer object
                                var lecturer = new Lecturer
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
                                // Add lecturers
                                Lecturers.Add(lecturer);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error retrieving lecturers: " + ex.Message);
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
                    string sql = "SELECT SubjectInstanceId, SubjectId, SubjectInstanceName, SubjectInstanceCode, StartDate, EndDate, LecturerId, SubjectInstanceYear FROM [SubjectInstance]";

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
                                };
                                // Add subject instance to the list
                                SubjectInstances.Add(subjectInstance);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error retrieving subject instances: " + ex.Message);
            }
        }

        // Method to get scheduling information from db
        private void LoadSchedules()
        {

            Console.WriteLine("Generate Schedules");


        }


        // Method to sort lecturers by UserID
        private void SortLecturersByUserId()
        {
            Lecturers = Lecturers.OrderBy(lecturer => lecturer.UserId).ToList();
        }
    }
}