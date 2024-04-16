using Microsoft.AspNetCore.Mvc.RazorPages;
using CSE3PAX.HelpClasses;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Lecturer
{
    // Ensuring required roles
    [RequireRoles("Lecturer")]

    /*
    This class, `LecturerIndexModel`, is a Razor Page Model responsible for handling the logic and data
    retrieval for the lecturer's index page. It includes properties to store lecturer information, methods
    to fetch lecturer details from the database, and to convert workload to hours per week. The class
    also manages session data, retrieves data from the database, and populates lists for rendering on
    the Razor Page. It incorporates authorisation checks for lecturer roles and uses dependency injection
    to access application settings. Additionally, it defines methods to retrieve lecturer details and 
    subject names associated with the lecturer from the database, handling exceptions appropriately.
    */

    public class LecturerIndexModel : PageModel
    {
        // Storing session-based information
        public string FullName { get; set; }
        public string Email { get; set; }
        public int UserID { get; set; }
        public int LecturerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public string UserImage { get; set; }
        public string Expertise01 { get; set; }
        public string Expertise02 { get; set; }
        public string Expertise03 { get; set; }
        public string Expertise04 { get; set; }
        public string Expertise05 { get; set; }
        public string Expertise06 { get; set; }
        public decimal? ConcurrentLoadCapacity { get; set; }
        public int InstanceCount = 0;
        public decimal? WorkHours { get; set; }

        // Accessing application settings
        private readonly IConfiguration _configuration;

        // Connection string from configuration file
        private readonly string _connectionString;

        public LecturerIndexModel(IConfiguration configuration)
        {
            // Checking for valid configuration
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Getting connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        // Table row headers for the next 12 months
        public List<string> Next12Months { get; set; } = new List<string>();

        // List to store SubjectInstances
        public List<SubjectInstance> SubjectInstances { get; set; } = new List<SubjectInstance>();

        // Initialize a list to store SubjectNames
        public HashSet<string> subjectNames = new HashSet<string>();

        /*
        Method executed when the page is requested. It retrieves session data such as the user's full name 
        and UserID. It then populates the Next12Months list with the names of the next 12 months in the format
        "MMMM-yyyy". After retrieving the UserID from the session, it calls the GetLecturerDetails method 
        to fetch lecturer-specific details from the database. If the UserID is not set, it handles the case 
        where the user is not authenticated, possibly redirecting to a login page.
    
        The method attempts to execute a SQL query to retrieve subject instance details for subjects taught
        by the lecturer identified by the UserID. It joins the SubjectInstance table with the Subjects table
        based on the SubjectID, filtering by the LecturerID associated with the provided UserID.
        */
        public void OnGet()
        {

            // Session data
            FullName = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");

            /*
            This loop iterates over the next 12 months starting from the current month.
            It calculates the date of each subsequent month and formats it to "MMMM-yyyy" (e.g., "April-2024").
            The formatted dates are then added to the Next12Months list for further use.
            */
            DateTime now = DateTime.Now;
            for (int i = 0; i < 12; i++)
            {
                DateTime nextMonth = now.AddMonths(i);
                Next12Months.Add(nextMonth.ToString("MMMM-yyyy"));
            }

            var session = HttpContext.Session;
            var userID = session.GetInt32("UserID");
            UserID = (int)userID;

            GetLecturerDetails();

            if (!userID.HasValue)
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    /*
                     SQL query to retrieve subject instance details including code, name, start date, and end date
                     for subjects taught by the lecturer identified by the provided UserID. It joins the SubjectInstance
                     table with the Subjects table based on the SubjectID, filtering by the LecturerID associated with
                     the provided UserID.
                     */
                    string sql = "SELECT SubjectInstanceCode, SubjectName, StartDate,EndDate FROM SubjectInstance LEFT JOIN Subjects ON SubjectInstance.SubjectID = Subjects.SubjectID WHERE LecturerID = (SELECT LecturerID FROM Lecturers WHERE UserID = @UserID)\r\n";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Use the parameter directly in your SQL command
                        command.Parameters.AddWithValue("@UserID", userID.Value);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SubjectInstances.Add(new SubjectInstance
                                {
                                    // Assuming these are the column names in your SubjectInstance table
                                    InstanceName = reader["SubjectInstanceCode"].ToString(),
                                    SubjectName = reader["SubjectName"].ToString(),
                                    // Correctly handle DateTime data types
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")).ToString("MMMM-yyyy"),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")).ToString("MMMM-yyyy"),
                                    FullStartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")).ToString("MMMM dd, yyyy"),
                                    FullEndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")).ToString("MMMM dd, yyyy"),

                                });
                                InstanceCount++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /*
        Method to retrieve lecturer details from the database based on the provided UserID. It establishes
        a connection to the database using the provided connection string and executes a SQL query to 
        retrieve details such as UserID, first name, last name, email, lecturer ID, expertise fields 
        (Expertise01 to Expertise06), and concurrent load capacity from the Users and Lecturers tables.
        The query filters the results based on the provided UserID and performs an inner join on the Users
        and Lecturers tables based on the UserID.
    
        If the lecturer details are found, it saves them to the corresponding properties in the class.
        Additionally, it retrieves the SubjectNames associated with the lecturer from the database and 
        populates the subjectNames list.
         */
        private void GetLecturerDetails()
        {

            try
            {
                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    /*
                     SQL query to retrieve lecturer details including UserID, first name, last name, email, lecturer ID,
                     and expertise fields (Expertise01 to Expertise06) along with concurrent load capacity from the Users
                     and Lecturers tables. It performs an inner join on the Users and Lecturers tables based on the UserID.
                     The query filters the results based on the provided UserID.
                    */
                    string sql = "SELECT u.UserId, u.FirstName, u.LastName, u.Email, l.LecturerID, " +
                                 "l.Expertise01, l.Expertise02, l.Expertise03, " +
                                 "l.Expertise04, l.Expertise05, l.Expertise06, " +
                                 "l.ConcurrentLoadCapacity " +
                                 "FROM [Users] u " +
                                 "INNER JOIN [Lecturers] l ON u.UserId = l.UserId " +
                                 "WHERE u.UserId = @UserID";

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
                                Email = reader.IsDBNull(3) ? null : reader.GetString(3);
                                LecturerID = reader.GetInt32(4);
                                Expertise01 = reader.IsDBNull(5) ? null : reader.GetString(5);
                                Expertise02 = reader.IsDBNull(6) ? null : reader.GetString(6);
                                Expertise03 = reader.IsDBNull(7) ? null : reader.GetString(7);
                                Expertise04 = reader.IsDBNull(8) ? null : reader.GetString(8);
                                Expertise05 = reader.IsDBNull(9) ? null : reader.GetString(9);
                                Expertise06 = reader.IsDBNull(10) ? null : reader.GetString(10);
                                ConcurrentLoadCapacity = reader.IsDBNull(11) ? null : (decimal?)reader.GetDecimal(11);
                                WorkHours = (decimal?)ConvertToHoursPerWeek((decimal)(reader.IsDBNull(11) ? 6 : (decimal?)reader.GetDecimal(11)));
                            }
                            else
                            {
                                // No lecturer found with the given UserID
                                Console.WriteLine("No lecturer found with UserID: " + UserID);
                            }
                        }
                    }

                    /*
                    SQL query string used to retrieve the names of subjects associated with a lecturer identified by their UserID.
                    It performs a LEFT JOIN operation between the SubjectInstance and Subjects tables based on the SubjectID.
                    The query filters the results by the LecturerID associated with the provided UserID, obtained from the Lecturers table.
                    */
                    string subjectNamesQuery = "SELECT SubjectName FROM SubjectInstance " +
                                               "LEFT JOIN Subjects ON SubjectInstance.SubjectID = Subjects.SubjectID " +
                                               "WHERE LecturerID = (SELECT LecturerID FROM Lecturers WHERE UserID = @UserID)";
                    using (SqlCommand subjectNamesCommand = new SqlCommand(subjectNamesQuery, connection))
                    {
                        subjectNamesCommand.Parameters.AddWithValue("@UserID", UserID);

                        using (SqlDataReader subjectNamesReader = subjectNamesCommand.ExecuteReader())
                        {
                            while (subjectNamesReader.Read())
                            {
                                // Add SubjectName to the list
                                subjectNames.Add(subjectNamesReader["SubjectName"].ToString());
                            }
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

        /*
        Method to convert workload capacity to hours per week. It takes the loadCapacity as input, which
        represents the workload capacity of the lecturer. If the loadCapacity is null, it returns null.
    
        It assumes a full-time load capacity of 6, which corresponds to 38 hours per week. The method 
        calculates the fraction of the full-time workload represented by the provided loadCapacity. It then 
        converts this fraction to hours per week by multiplying it by the full-time hours per week and 
        rounding up to the nearest integer using Math.Ceiling.

        Returns the calculated hours per week.
        */
        private double? ConvertToHoursPerWeek(decimal? loadCapacity)
        {
            if (loadCapacity == null)
            {
                return null;
            }

            // full-time load capacity of 6 corresponds to 38 hours per week
            const double fullTimeLoadCapacity = 6;
            const double fullTimeHoursPerWeek = 38;

            // Convert load capacity to a fraction of a full-time workload
            double loadFractionOfFullTime = (double)loadCapacity / fullTimeLoadCapacity;

            // Convert fraction of a full-time workload to hours per week and round up
            double hoursPerWeek = Math.Ceiling(loadFractionOfFullTime * fullTimeHoursPerWeek);
            return hoursPerWeek;
        }
    }
}