using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CSE3PAX.Pages.Manager
{
    public class EditSubjectInstanceModel : PageModel
    {

        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public EditSubjectInstanceModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        [BindProperty]
        public int SubjectInstanceId { get; set; }
        [BindProperty]
        public int SubjectId { get; set; }
        [BindProperty]
        public string SubjectInstanceName { get; set; }
        [BindProperty]
        public string SubjectInstanceCode { get; set; }
        [BindProperty]
        public String LecturerEmail { get; set; }
        [BindProperty]
        public DateTime StartDate { get; set; }
        [BindProperty]
        public DateTime EndDate { get; set; }
        [BindProperty]
        public decimal Load { get; set; }
        [BindProperty]
        public int SubjectInstanceYear { get; set; }



        [FromQuery(Name = "selectedSubjectInstance")]
        public int SelectedSubjectInstance { get; set; }

        public void OnGet()
        {
            try
            {
                Debug.WriteLine("Loaded instance with ID: " + SelectedSubjectInstance);
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    // Update the SQL command to include the new JOINs and SELECT fields
                    string sql = @"SELECT si.SubjectInstanceID, si.SubjectID, si.SubjectInstanceName, si.SubjectInstanceCode, 
                           u.Email, si.StartDate, si.EndDate, si.SubjectInstanceYear, si.Load
                           FROM [schedulingDB].[dbo].[SubjectInstance] AS si
                           JOIN [schedulingDB].[dbo].[Lecturers] AS l ON si.LecturerID = l.LecturerID
                           JOIN [schedulingDB].[dbo].[Users] AS u ON l.UserID = u.UserID
                           WHERE si.SubjectInstanceID = @SubjectInstanceID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@SubjectInstanceID", SelectedSubjectInstance);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                SubjectInstanceId = reader.GetInt32(0);
                                SubjectId = reader.GetInt32(1);
                                SubjectInstanceName = reader.GetString(2);
                                SubjectInstanceCode = reader.GetString(3);
                                LecturerEmail = reader.GetString(4);
                                StartDate = reader.GetDateTime(5);
                                EndDate = reader.GetDateTime(6);
                                SubjectInstanceYear = reader.GetInt32(7);
                                Load = reader.GetDecimal(8);

                                Debug.WriteLine($"Subject Instance ID: {SubjectInstanceId}, Subject ID: {SubjectId}, Name: {SubjectInstanceName}, Code: {SubjectInstanceCode}, Lecturer Email: {LecturerEmail}, Start Date: {StartDate}, End Date: {EndDate}, Year: {SubjectInstanceYear}, Load: {Load}");
                            }
                            else
                            {
                                Debug.WriteLine("No subject instance found with the provided ID.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error fetching subject instance data: " + ex.Message);
            }
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int lecturerId = -1; // Initialize with an invalid value to check later

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // First, fetch the LecturerID based on the Lecturer's email
                var fetchLecturerIdSql = @"
        SELECT l.LecturerID
        FROM [schedulingDB].[dbo].[Users] AS u
        JOIN [schedulingDB].[dbo].[Lecturers] AS l ON u.UserID = l.UserID
        WHERE u.Email = @Email";

                using (var fetchCommand = new SqlCommand(fetchLecturerIdSql, connection))
                {
                    fetchCommand.Parameters.AddWithValue("@Email", LecturerEmail); 

                    using (var reader = await fetchCommand.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            lecturerId = reader.GetInt32(0);
                        }
                    }
                }

                if (lecturerId == -1)
                {
                    // Handle the case where the lecturer is not found
                    Debug.WriteLine("Lecturer not found with the provided email.");
                    return Page(); 
                }

                // Then, update the SubjectInstance with the fetched LecturerID
                var updateSql = @"
        UPDATE SubjectInstance 
        SET 
            SubjectId = @SubjectId, 
            SubjectInstanceName = @SubjectInstanceName, 
            SubjectInstanceCode = @SubjectInstanceCode, 
            LecturerId = @LecturerId, 
            StartDate = @StartDate, 
            EndDate = @EndDate, 
            SubjectInstanceYear = @SubjectInstanceYear,
            Load = @Load 
        WHERE SubjectInstanceId = @SubjectInstanceId";

                using (var updateCommand = new SqlCommand(updateSql, connection))
                {
                    updateCommand.Parameters.AddWithValue("@SubjectInstanceId", SubjectInstanceId);
                    updateCommand.Parameters.AddWithValue("@SubjectId", SubjectId);
                    updateCommand.Parameters.AddWithValue("@SubjectInstanceName", SubjectInstanceName);
                    updateCommand.Parameters.AddWithValue("@SubjectInstanceCode", SubjectInstanceCode);
                    updateCommand.Parameters.AddWithValue("@LecturerId", lecturerId); // Use the fetched LecturerID
                    updateCommand.Parameters.AddWithValue("@StartDate", StartDate);
                    updateCommand.Parameters.AddWithValue("@EndDate", EndDate);
                    updateCommand.Parameters.AddWithValue("@SubjectInstanceYear", SubjectInstanceYear);
                    updateCommand.Parameters.Add("@Load", SqlDbType.Decimal).Value = Load;
                    updateCommand.Parameters["@Load"].Precision = 10;
                    updateCommand.Parameters["@Load"].Scale = 1;

                    await updateCommand.ExecuteNonQueryAsync();
                }
            }

            return RedirectToPage("/Manager/StaffSchedules"); // Adjust the redirection as needed
        }




    }
}
