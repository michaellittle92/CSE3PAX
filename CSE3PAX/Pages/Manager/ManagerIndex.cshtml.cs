﻿using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages.Manager
{
    //Checking for required Roles
    [RequireRoles("Manager")]


    public class ManagerIndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public ManagerIndexModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }


        // List to store User information
        public List<User> Users { get; set; } = new List<User>();

        //User class to store User variable information
        public class User
        {

            // User variables
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
            public decimal? WorkHours { get; set; }
            public int LoadCapacityPercentage { get; set; }
        }

        // String to store full name (session)
        public string FullName { get; set; }

        // HTTP OnGet
        public void OnGet()
        {
            // Retrieve full name from the session data
            FullName = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
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
                    OnGet();
                    LoadUsers();
                    SortLecturerWorkloadByPercentage();
                    break;
                case "hideUsers":
                    OnGet();
                    break;
            }
            return Page();
        }

        /*
        Method to load users from the database, including lecturers' expertise, load capacity, and user type.
        */
        private void LoadUsers()
        {
            // Console write for testing
            Console.WriteLine("Generate Users");

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

                    /*
                    SQL query to retrieve user data, including lecturers' expertise, load capacity, and user type,
                    as well as total load and load capacity percentage.
                    */
                    string sql = "SELECT u.UserId, u.FirstName, u.LastName, u.Email, " +
                                 "l.Expertise01, l.Expertise02, l.Expertise03, " +
                                 "l.Expertise04, l.Expertise05, l.Expertise06, " +
                                 "l.ConcurrentLoadCapacity, u.isAdmin, u.isManager, u.isLecturer, " +
                                 "MAX(si.Load) AS TotalLoad, " +
                                 "CAST(ROUND(ISNULL(SUM(si.Load), 0) / NULLIF(l.ConcurrentLoadCapacity, 0) * 100, 0) AS INT) AS LoadCapacityPercentage " +
                                 "FROM [Users] u " +
                                 "LEFT JOIN [Lecturers] l ON u.UserId = l.UserId " +
                                 "LEFT JOIN [SubjectInstance] si ON l.LecturerId = si.LecturerId " +
                                 "WHERE u.isLecturer = 1 " +
                                 "GROUP BY u.UserId, u.FirstName, u.LastName, u.Email, " +
                                 "l.Expertise01, l.Expertise02, l.Expertise03, " +
                                 "l.Expertise04, l.Expertise05, l.Expertise06, " +
                                 "l.ConcurrentLoadCapacity, u.isAdmin, u.isManager, u.isLecturer";

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
                                    ConcurrentLoadCapacity = reader.IsDBNull(10) ? null : (decimal?)reader.GetDecimal(10),
                                    WorkHours = (decimal?)ConvertToHoursPerWeek((decimal)(reader.IsDBNull(10) ? 6 : (decimal?)reader.GetDecimal(10))),
                                    LoadCapacityPercentage = Convert.ToInt32(reader["LoadCapacityPercentage"])
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

        //Method to sort lecturers workloads
        private void SortLecturerWorkloadByPercentage()
        {
            Users = Users.OrderBy(user => user.LoadCapacityPercentage).ToList();
        }

        /*
        Method to convert a load capacity value to hours per week.
        Assumes a full-time load capacity of 6 corresponds to 38 hours per week.
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