using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;


namespace CSE3PAX.Pages.Admin
{

    //Checking for required Roles
    [RequireRoles("Admin")]

    public class SubjectManagementModel : PageModel
    {
        //Object to access application settings
        private readonly IConfiguration _configuration;

        //String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        //List to store Subject information
        public List<Subject> Subjects { get; set; } = new List<Subject>();

        //Subject class to store Subject variable information
        public class Subject
        {

            // Subject variables
            public int SubjectId { get; set; }
            public string SubjectCode { get; set; }
            public string SubjectName { get; set; }
            public string SubjectClassification { get; set; }
            public int YearLevel { get; set; }
            
        }

        //List to store Subject Instance information
        public List<SubjectInstance> SubjectInstances { get; set; } = new List<SubjectInstance>();

        //SubjectInstance class to store SubjectInstance variable information
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
            public string LecturerFirstName { get; internal set; }
            public string LecturerLastName { get; internal set; }
        }


        /*
         Initialise SubjectManagementModel class
         Configuration object (ConnectionStrings) located in appsettings.json
         Exception thrown when DefaultConnect string is not found in file
         */
        public SubjectManagementModel(IConfiguration configuration)
        {
            //Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        // HTTP OnGet
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
                case "subjects":
                    LoadSubjects();
                    SortSubjectsById();
                    break;
                case "subjectInstances":
                    Console.WriteLine("Subject instances generating");
                    LoadSubjectInstances();
                    SortSubjectInstancesById();
                    break;
            }
            return Page();
        }

        /*
        Loads subjects from the database and populates the Subjects list.
        The SQL query selects all subjects from the Subjects table.
        Each subject's information is extracted from the query results and added to the Subjects list.
        */
        private void LoadSubjects()
        {
            // Console write for testing
            Console.WriteLine("Generating Subjects");

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
                    string sql = "SELECT SubjectId, SubjectCode, SubjectName, SubjectClassification, YearLevel FROM [Subjects]";

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
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error retrieving subjects: " + ex.Message);
            }
        }

        /*
        Sorts the Subjects list by SubjectId in ascending order.
        */
        private void SortSubjectsById()
        {
            Subjects = Subjects.OrderBy(subject => subject.SubjectId).ToList();
        }

        /*
        Loads subject instances from the database and populates the SubjectInstances list.
        The SQL query selects all subject instances, joining Subjects, Lecturers, and Users tables to retrieve related information.
        Each subject instance's information is extracted from the query results and added to the SubjectInstances list.
        */
        private void LoadSubjectInstances()
        {

            // Console write for testing
            Console.WriteLine("Generating Subject Instances");

            // Retrieve the list of subject instances from the database
            try
            {

                //Clear the existing list of subject instances
                SubjectInstances.Clear();

                //Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    //Open Connection
                    connection.Open();

                    /*
                    Selects subject instance details along with related information from the Subjects, Lecturers, and Users tables.
                    Joins the SubjectInstance table with Subjects, Lecturers, and Users tables based on their respective foreign keys.
                    Aliases are used to distinguish between columns with the same name in different tables.
                    */
                    string sql = @"SELECT si.SubjectInstanceId, si.SubjectId, s.SubjectName AS SubjectInstanceName, 
                                    si.SubjectInstanceCode, si.StartDate, si.EndDate, si.LecturerId, 
                                    si.SubjectInstanceYear, s.SubjectCode, u.FirstName AS LecturerFirstName, u.LastName AS LecturerLastName
                                    FROM [SubjectInstance] si 
                                    JOIN [Subjects] s ON si.SubjectId = s.SubjectId
                                    JOIN [Lecturers] l ON si.LecturerId = l.LecturerId
                                    JOIN [Users] u ON l.UserId = u.UserId";

                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        // Execute SQL query and get results
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            // Iterate through the results and add subject instances to the list
                            while (reader.Read())
                            {

                                // Save subject instance information to SubjectInstance object
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
                                    SubjectCode = reader.GetString(8),
                                    LecturerFirstName = reader.GetString(9),
                                    LecturerLastName = reader.GetString(10)
                                };

                                // Add subject instances
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

        /*
        Sorts the SubjectInstances list by SubjectInstanceId in ascending order.
        */
        private void SortSubjectInstancesById()
        {
            SubjectInstances = SubjectInstances.OrderBy(instance => instance.SubjectInstanceId).ToList();
        }
    }
}