using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CSE3PAX.Pages.Manager
{
    public class StaffSchedulesModel : PageModel
    {
        // Object to access application settings
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        // Properties to store user information
        public List<Lecturer> Lecturers { get; set; }

        //Current Table row headers
        public List<string> Next12Months { get; set; } = new List<string>();

        //SubjectInstance class in HelpClasses -> SubjectInstance.cs
        public List<SubjectInstance> SubjectInstances { get; set; } = new List<SubjectInstance>();

        public class Lecturer
        {
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        // Properties to store selected lecturer's details
        public int SelectedUserId { get; set; }
        public int SelectedLecturerId { get; set; }
        public int SelectedSubjectInstanceId { get; set; }
        public string SelectedEmail { get; set; }
        public string SelectedFirstName { get; set; }
        public string SelectedLastName { get; set; }
        public decimal SelectedConcurrentLoadCapacity { get; set; }
        public string SelectedExpertise01 { get; set; }
        public string SelectedExpertise02 { get; set; }
        public string SelectedExpertise03 { get; set; }
        public string SelectedExpertise04 { get; set; }
        public string SelectedExpertise05 { get; set; }
        public string SelectedExpertise06 { get; set; }
        public int SelectedSubjectInstanceCount { get; set; } = 0;

        /*
         Initialise IndexModel class
         Configuration object (ConnectionStrings) located in appsettings.json
         Exception thrown when DefaultConnect string is not found in file
         */
        public StaffSchedulesModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        public void OnGet(int? selectedUserId)
        {
            LoadLecturers();
            if (selectedUserId.HasValue)
            {
                OnPost(selectedUserId.Value);
            }
        }

        public IActionResult OnPost(int selectedUserId)
        {
            // Receive the selected lecturer's UserID as a parameter and get their details

            try
            {
                // Get details of the selected lecturer using the selectedUserId
                FetchLecturerDetails(selectedUserId);

                // Testing
                Console.WriteLine($"UserID: {SelectedUserId}, Email: {SelectedEmail}, FirstName: {SelectedFirstName}, LastName: {SelectedLastName} Expertise01: {SelectedExpertise01}, ConcurrentLoadCapacity: {SelectedConcurrentLoadCapacity}");

                // Load lecturers to populated dropdown
                LoadLecturers();

                // Return same page
                return Page();
            }
            catch (Exception ex)
            {
                // Handle exception, such as logging or displaying an error message
                Console.WriteLine("An error occurred: " + ex.Message);

                // Return an error response
                return new BadRequestResult();
            }
        }

        // Method to load all lectures
        private void LoadLecturers()
        {
            Next12Months.Clear();
            // Retrieve the list of lecturers from the database

            try
            {
                Lecturers = new List<Lecturer>();

                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL query to select users who are lecturers
                    string sql = "SELECT UserId, FirstName, LastName FROM [Users] WHERE isLecturer = 1";

                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Execute SQL query and get results
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if there are matching users in the database
                            while (reader.Read())
                            {
                                // Create a new Lecturer object and add it to the list
                                Lecturers.Add(new Lecturer
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                });
                            }
                            DateTime now = DateTime.Now;
                            for (int i = 0; i < 12; i++)
                            {
                                DateTime nextMonth = now.AddMonths(i);
                                Next12Months.Add(nextMonth.ToString("MMMM-yyyy"));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception, such as logging or displaying an error message
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void FetchLecturerDetails(int userId)
        {
            // Get the details of the lecturer with the specified UserID
            try
            {

                SubjectInstances = new List<SubjectInstance>();

                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL select statement to get user details from the Users and Lecturers table
                    string sql = @"
                    SELECT 
                        u.Email, 
                        u.FirstName, 
                        u.LastName, 
                        u.UserID, -- Add UserID to fetch it
                        l.LecturerID, -- Add LecturerID to fetch it
                        l.Expertise01, 
                        l.Expertise02, 
                        l.Expertise03, 
                        l.Expertise04, 
                        l.Expertise05, 
                        l.Expertise06, 
                        l.ConcurrentLoadCapacity,
                        s.SubjectInstanceID, -- Corrected column name
                        s.SubjectInstanceName,
                        s.SubjectInstanceCode,
                        s.StartDate,
                        s.EndDate
                    FROM 
                        [Users] u
                    LEFT JOIN 
                        [Lecturers] l ON u.UserID = l.UserID
                    LEFT JOIN 
                        [SubjectInstance] s ON l.LecturerID = s.LecturerID
                    WHERE 
                        u.UserID = @UserId";


                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameter for UserID
                        command.Parameters.AddWithValue("@UserId", userId);

                        // Execute SQL query and get results
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if there are matching users in the database
                            while (reader.Read())
                            {
                                // Fetch lecturer details
                                SelectedUserId = userId;
                                SelectedLecturerId = reader.GetInt32(reader.GetOrdinal("LecturerID"));
                                SelectedEmail = reader.GetString(reader.GetOrdinal("Email"));
                                SelectedFirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                SelectedLastName = reader.GetString(reader.GetOrdinal("LastName"));
                                SelectedConcurrentLoadCapacity = reader.IsDBNull(reader.GetOrdinal("ConcurrentLoadCapacity")) ? 0 : reader.GetDecimal(reader.GetOrdinal("ConcurrentLoadCapacity"));
                                SelectedExpertise01 = !reader.IsDBNull(reader.GetOrdinal("Expertise01")) ? reader.GetString(reader.GetOrdinal("Expertise01")) : "";
                                SelectedExpertise02 = !reader.IsDBNull(reader.GetOrdinal("Expertise02")) ? reader.GetString(reader.GetOrdinal("Expertise02")) : "";
                                SelectedExpertise03 = !reader.IsDBNull(reader.GetOrdinal("Expertise03")) ? reader.GetString(reader.GetOrdinal("Expertise03")) : "";
                                SelectedExpertise04 = !reader.IsDBNull(reader.GetOrdinal("Expertise04")) ? reader.GetString(reader.GetOrdinal("Expertise04")) : "";
                                SelectedExpertise05 = !reader.IsDBNull(reader.GetOrdinal("Expertise05")) ? reader.GetString(reader.GetOrdinal("Expertise05")) : "";
                                SelectedExpertise06 = !reader.IsDBNull(reader.GetOrdinal("Expertise06")) ? reader.GetString(reader.GetOrdinal("Expertise06")) : "";

                                // Add subject instance to the list
                                SubjectInstances.Add(new SubjectInstance
                                {
                                    UserID = userId,
                                    LecturerID = reader.GetInt32(reader.GetOrdinal("LecturerID")),
                                    InstanceID = reader.GetInt32(reader.GetOrdinal("SubjectInstanceID")),
                                    InstanceName = reader["SubjectInstanceCode"].ToString(),
                                    SubjectName = reader["SubjectInstanceName"].ToString(),
                                    // Correctly handle DateTime data types
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")).ToString("MMMM-yyyy"),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")).ToString("MMMM-yyyy"),
                                    FullStartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")).ToString("MMMM dd, yyyy"),
                                    FullEndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")).ToString("MMMM dd, yyyy"),
                                });

                                SelectedSubjectInstanceCount++;
                            }
                            // Close reader
                            reader.Close();
                        }
                    }
                }
                // Print the full list of subject instances to the console
                Console.WriteLine("Full List of Subject Instances:");
                foreach (var subjectInstance in SubjectInstances)
                {
                    Console.WriteLine($"Subject Instance Name: {subjectInstance.SubjectName}, Subject Instance ID: {subjectInstance.InstanceName}");
                }
            }
            catch (Exception ex)
            {
                // Handle exception, such as logging or displaying an error message
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int instanceID, int lecturerID, int userID)
        {
            Debug.WriteLine("Deleting instance with ID: " + instanceID);
            Debug.WriteLine("Lecturer ID: " + lecturerID);
            Debug.WriteLine("User ID: " + userID);

            //  delete logic.
            try
            {
                // Establish connection to the database
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL delete statement to delete a subject instance
                    string sql = "DELETE FROM [SubjectInstance] WHERE SubjectInstanceID = @InstanceID";

                    // SQL command object with query and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameter for InstanceID
                        command.Parameters.AddWithValue("@InstanceID", instanceID);

                        // Execute SQL query and get results
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception, such as logging or displaying an error message
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            // Assuming deletion is successful, redirect back to the same page with the lecturerID

            return RedirectToPage(new { selectedUserId = userID });

        }
    }
}