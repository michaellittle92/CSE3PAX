using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;

namespace CSE3PAX.Pages.Admin
{
    public class CreateSubjectInstanceModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public CreateSubjectInstanceModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }
        public List<ListSubjects> ListSubjects { get; set; } = new List<ListSubjects>();
        public List<Lecturers> ListLecturers { get; set; } = new List<Lecturers>();

        [BindProperty]
        public string LecturerEmail { get; set; }

        [BindProperty]
        public string SubjectNameInput { get; set; }

        [BindProperty]
        public DateTime StartDateInput { get; set; }

        [BindProperty]
        public DateTime EndDateInput { get; set; }


        public void OnGet()
        {
            try { 
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT DISTINCT SubjectName From Subjects;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var subjectName = new ListSubjects
                                {
                                    SubjectName = reader["SubjectName"].ToString(),
                                };
                                ListSubjects.Add(subjectName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT Email, FirstName, LastName From Users WHERE IsLecturer = 1;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var lecturer = new Lecturers
                                {
                                    Email = reader["Email"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                };
                                ListLecturers.Add(lecturer);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        public async Task<IActionResult> OnPostAsync()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;


                        command.CommandText = @"
                --Declared Variables 

                DECLARE @UserID INT;
                DECLARE @LecturerID INT;
                DECLARE @SubjectID INT;
                DECLARE @SubjectCode NVARCHAR(100);
                DECLARE @Year NVARCHAR(100);
                DECLARE @Month NVARCHAR(100);
                DECLARE @SubjectInstanceCode NVARCHAR(100);
                DECLARE @SubjectInstanceName NVARCHAR(100);
                DECLARE @RandomAlphaNumeric NVARCHAR(4); -- Variable for the 4-digit alphanumeric string

                --Calculated values in the DB 
                SELECT @UserID = UserID FROM Users WHERE Email = @UserEmailInput; 
                SELECT @SubjectID = SubjectID FROM Subjects WHERE SubjectName = @SubjectNameInput;
                SELECT @LecturerID = LecturerID FROM Lecturers WHERE UserID = @UserID;
                SELECT @SubjectCode = SubjectCode FROM Subjects WHERE SubjectID = @SubjectID;
                SET @Year = CAST(YEAR(@StartDateInput) AS NVARCHAR(4));
                SET @Month = DATENAME(MONTH, @EndDateInput);

                -- Generate a 4-digit alphanumeric string from a NEWID()
                SELECT @RandomAlphaNumeric = UPPER(SUBSTRING(CONVERT(NVARCHAR(36), NEWID()), 1, 4));

                SET @SubjectInstanceCode = @Year + '-' + @SubjectCode; -- Concatenate year and SubjectCode
                SET @SubjectInstanceName = @Year + '-' + @SubjectCode + '-' + @Month + ' (' + @RandomAlphaNumeric + ')'; 

                --Insert data into SubjectInstances Table (assuming the table and columns as before)
                INSERT INTO SubjectInstance (SubjectID, SubjectInstanceName, SubjectInstanceCode, LecturerID, StartDate, EndDate, SubjectInstanceYear)
                VALUES (@SubjectID, @SubjectInstanceName, @SubjectInstanceCode, @LecturerID, @StartDateInput, @EndDateInput, @Year);

                SELECT * FROM SubjectInstance";


                        command.Parameters.AddWithValue("@UserEmailInput", LecturerEmail);
                        command.Parameters.AddWithValue("@SubjectNameInput", SubjectNameInput);
                        command.Parameters.AddWithValue("@StartDateInput", StartDateInput);
                        command.Parameters.AddWithValue("@EndDateInput", EndDateInput);

                    
                        await command.ExecuteNonQueryAsync();
                    }
                }

                // Redirect or return a success message/page
                return RedirectToPage("./AdminIndex"); 
            }
            catch (SqlException ex)
            {

                Debug.WriteLine($"SQL Error: {ex.Message}");
     
            }
            return Page(); 
            }
        }


    }
