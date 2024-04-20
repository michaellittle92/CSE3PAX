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
        public int LecturerId { get; set; }
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
                    using (SqlCommand command = new SqlCommand("SELECT * FROM SubjectInstance WHERE SubjectInstanceId = @SubjectInstanceId", connection))
                    {
                        command.Parameters.AddWithValue("@SubjectInstanceId", SelectedSubjectInstance);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                SubjectInstanceId = reader.GetInt32(0);
                                SubjectId = reader.GetInt32(1);
                                SubjectInstanceName = reader.GetString(2);
                                SubjectInstanceCode = reader.GetString(3);
                                LecturerId = reader.GetInt32(4);
                                StartDate = reader.GetDateTime(5);
                                EndDate = reader.GetDateTime(6);
                                SubjectInstanceYear = reader.GetInt32(7);
                                Load = reader.GetDecimal(8);

                                Debug.WriteLine($"Subject Instance ID: {SubjectInstanceId}, Subject ID: {SubjectId}, Name: {SubjectInstanceName}, Code: {SubjectInstanceCode}, Lecturer ID: {LecturerId}, Start Date: {StartDate}, End Date: {EndDate}, Year: {SubjectInstanceYear}, Load: {Load}");
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

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
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

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@SubjectInstanceId", SubjectInstanceId);
                    command.Parameters.AddWithValue("@SubjectId", SubjectId);
                    command.Parameters.AddWithValue("@SubjectInstanceName", SubjectInstanceName);
                    command.Parameters.AddWithValue("@SubjectInstanceCode", SubjectInstanceCode);
                    command.Parameters.AddWithValue("@LecturerId", LecturerId);
                    command.Parameters.AddWithValue("@StartDate", StartDate);
                    command.Parameters.AddWithValue("@EndDate", EndDate);
                    command.Parameters.AddWithValue("@SubjectInstanceYear", SubjectInstanceYear);
                    command.Parameters.Add("@Load", SqlDbType.Decimal).Value = Load;
                    command.Parameters["@Load"].Precision = 10;
                    command.Parameters["@Load"].Scale = 1;

                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToPage("/Manager/StaffSchedules"); // Adjust the redirection as needed
        }



    }
}
