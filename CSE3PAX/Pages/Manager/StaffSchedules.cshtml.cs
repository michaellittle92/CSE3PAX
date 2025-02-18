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
        public string SuccessMessage { get; set; }

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        // Properties to store user information
        public List<Lecturer> Lecturers { get; set; }

        //Current Table row headers
        public List<string> Next12Months { get; set; } = new List<string>();

        //SubjectInstance class in HelpClasses -> SubjectInstance.cs
        public List<SubjectInstance> SubjectInstances { get; set; } = new List<SubjectInstance>();

        // Lecturer class to store lecturer userid, first and last name
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

        /*
        Handles the HTTP GET request for the page.
        Loads the list of lecturers.
        If a selected user ID is provided, triggers the HTTP POST method with the selected user ID.
        */
        public void OnGet(int? selectedUserId)
        {
            LoadLecturers();
            if (selectedUserId.HasValue)
            {
                OnPost(selectedUserId.Value);
            }
        }

        /*
        Handles the HTTP POST request for the page.
        Retrieves details of the selected lecturer using the provided selectedUserId.
        Loads lecturers to populate the dropdown.
        Returns the same page.
        If an error occurs, logs the error message and returns a BadRequestResult.
        */
        public IActionResult OnPost(int selectedUserId)
        {
            try
            {
                // Get details of the selected lecturer using the selectedUserId
                FetchLecturerDetails(selectedUserId);

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

        /*
        Clears the Next12Months list.
        Retrieves the list of lecturers from the database.
        Establishes a connection to the database and executes a SQL query to select users who are lecturers.
        Adds the retrieved lecturers to the Lecturers list.
        Generates the list of the next 12 months and adds them to the Next12Months list.
        If an error occurs during the process, logs the error message.
        */

        private void LoadLecturers()
        {
            Next12Months.Clear();

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

        /*
        Fetches the details of the lecturer with the specified UserID from the database.
        Establishes a connection to the database and executes a SQL query to retrieve the lecturer details.
        Adds the lecturer's details to the respective properties.
        Retrieves the subject instances associated with the lecturer and adds them to the SubjectInstances list.
        If an error occurs during the process, logs the error message.
        */
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

                    /*
                    Retrieves details of a lecturer and their associated subject instances from the database.
                    Selects the lecturer's email, first name, last name, user ID, lecturer ID, expertise fields, concurrent load capacity, and subject instance details.
                    Joins the Users table with the Lecturers table on the UserID column.
                    Joins the Lecturers table with the SubjectInstance table on the LecturerID column.
                    Filters the results based on the specified user ID.
                    */
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

        /*
        Handles the asynchronous POST request to delete a subject instance from the database.
        Retrieves the instanceID, lecturerID, and userID from the request parameters.
        Deletes the subject instance corresponding to the instanceID from the database.
        If an error occurs during the deletion process, logs the error message.
        Redirects back to the same page with the selectedUserID parameter after deletion.
        */
        public IActionResult OnPostDelete(int instanceID, int lecturerID, int userID)
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
                TempData["SuccessMessage"] = "Subject Instance deleted successfully!";
                Console.WriteLine(SuccessMessage);
                return RedirectToPage(new { selectedUserId = userID });
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return Page();
            }
        }

        /*
        Handles the asynchronous POST request to edit a subject instance.
        Retrieves the instanceID, lecturerID, and userID from the request parameters.
        Logs the instanceID, lecturerID, and userID for debugging purposes.
        Redirects to the "CreateSubjectInstance" page with the selectedSubjectInstance parameter set to instanceID.
        */
        public async Task<IActionResult> OnPostEditAsync(int instanceID, int lecturerID, int userID)
        {
            Debug.WriteLine("Editing instance with ID: " + instanceID);
            Debug.WriteLine("Lecturer ID: " + lecturerID);
            Debug.WriteLine("User ID: " + userID);
            return RedirectToPage("/Manager/EditSubjectInstance",new { selectedSubjectInstance = instanceID });
        }
    }
}