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
      
        public Dictionary<int, string> SubjectNames { get; set; } = new Dictionary<int, string>();
        [BindProperty]
        public int SelectedSubjectId { get; set; }



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
            PopulateSubjectDropdown();
        }

        private void PopulateSubjectDropdown()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT SubjectID, SubjectName FROM [schedulingDB].[dbo].[Subjects]";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        SubjectNames.Clear();  // Clear previous entries to avoid duplications
                        while (reader.Read())
                        {
                            SubjectNames.Add(reader.GetInt32(0), reader.GetString(1));
                        }
                    }
                    SelectedSubjectId = SubjectId;

                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int lecturerId = -1;
            string newSubjectCode = "";
            string newSubjectName = "";
            int selectedYear = StartDate.Year;  
            string selectedMonth = StartDate.ToString("MMMM");  

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Fetch the LecturerID based on the Lecturer's email
                string fetchLecturerIdSql = @"
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
                    Debug.WriteLine("Lecturer not found with the provided email.");
                    return Page();
                }

                // Fetch the new Subject Code if SubjectId changed
                if (SubjectId != SelectedSubjectId)
                {
                    string fetchSubjectCodeSql = "SELECT SubjectCode FROM Subjects WHERE SubjectID = @SubjectID";
                    using (var codeCommand = new SqlCommand(fetchSubjectCodeSql, connection))
                    {
                        codeCommand.Parameters.AddWithValue("@SubjectID", SelectedSubjectId);
                        using (var codeReader = codeCommand.ExecuteReader())
                        {
                            if (codeReader.Read())
                            {
                                newSubjectCode = codeReader.GetString(0);
                                newSubjectName = $"{selectedYear}-{newSubjectCode}-{selectedMonth} ({Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()})";
                                SubjectInstanceCode = $"{selectedYear}-{newSubjectCode}";
                            }
                        }
                    }
                }

                // Update the SubjectInstance
                string updateSql = @"
UPDATE SubjectInstance 
SET 
    SubjectId = @SelectedSubjectId,
    SubjectInstanceName = @NewSubjectInstanceName,
    SubjectInstanceCode = @NewSubjectInstanceCode,
    LecturerId = @LecturerId, 
    StartDate = @StartDate, 
    EndDate = @EndDate, 
    SubjectInstanceYear = @SubjectInstanceYear,
    Load = @Load 
WHERE SubjectInstanceId = @SubjectInstanceId";
                using (var updateCommand = new SqlCommand(updateSql, connection))
                {
                    updateCommand.Parameters.AddWithValue("@SubjectInstanceId", SubjectInstanceId);
                    updateCommand.Parameters.AddWithValue("@SelectedSubjectId", SelectedSubjectId);
                    updateCommand.Parameters.AddWithValue("@NewSubjectInstanceName", newSubjectName);
                    updateCommand.Parameters.AddWithValue("@NewSubjectInstanceCode", SubjectInstanceCode);
                    updateCommand.Parameters.AddWithValue("@LecturerId", lecturerId);
                    updateCommand.Parameters.AddWithValue("@StartDate", StartDate);
                    updateCommand.Parameters.AddWithValue("@EndDate", EndDate);
                    updateCommand.Parameters.AddWithValue("@SubjectInstanceYear", selectedYear);
                    updateCommand.Parameters.AddWithValue("@Load", Load);

                    await updateCommand.ExecuteNonQueryAsync();
                }
            }
            // Set success message after subject instance edit
            TempData["SuccessMessage"] = "Subject Instance edited successfully.";

            // Redirect
            return RedirectToPage("/Manager/StaffSchedules");
        }





    }
}
