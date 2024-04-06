using Microsoft.AspNetCore.Mvc.RazorPages;
using CSE3PAX; 
using Microsoft.AspNetCore.Authorization;
using CSE3PAX.HelpClasses;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace CSE3PAX.Pages.Lecturer
{
    //Checking for required Roles
    [RequireRoles("Lecturer")]

    public class LecturerIndexModel : PageModel
    {
        // String to store full name (session)
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

        // Object to access application settings
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public LecturerIndexModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        //Current Table row headers
        public List<string> Next12Months { get; set; } = new List<string>();

        //SubjectInstance class in HelpClasses -> SubjectInstance.cs
        public List<SubjectInstance> SubjectInstances { get; set; } = new List<SubjectInstance>();

        // Initialize a list to store SubjectNames
        public HashSet<string> subjectNames = new HashSet<string>();


        public void OnGet()
        {

            // Session data
            FullName = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");

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
                // Handle case where userID is not set, perhaps redirect to login
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Corrected SQL command to use parameter directly in the WHERE clause
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
                // Consider logging the exception
                // Handle any errors that might have occurred during database access
            }
        }

        private void GetLecturerDetails()
        {

            try
            {
                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL query to select the lecturer details for the given UserID
                    string sql = "SELECT u.UserId, u.FirstName, u.LastName, u.Email, l.LecturerID, " +
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

                    // Retrieve the SubjectNames associated with the lecturer
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

        // Convert workload to hours per week
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