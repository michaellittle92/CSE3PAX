using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CSE3PAX.HelpClasses; // Adjust this namespace based on your actual namespace
using System.Reflection.PortableExecutable;
using System.Diagnostics;

namespace CSE3PAX.Pages.Manager
{
    public class SITestingModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SITestingModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [BindProperty(SupportsGet = true)]
        public string SelectedSubject { get; set; }

        public string SuccessMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        [BindProperty]
        public string SelectedFirstName { get; set; }

        [BindProperty]
        public string SelectedLastName { get; set; }

        [BindProperty]
        public string SelectedEmail { get; set; }

        [BindProperty]
        public int NumberOfStudents { get; set; }

        [BindProperty]
        public bool IsDevelopmentRequired { get; set; }

        [BindProperty]
        public string SelectedSubjectHidden { get; set; }

        public List<CSE3PAX.HelpClasses.LecturerInfo> Lecturers { get; set; } = new List<CSE3PAX.HelpClasses.LecturerInfo>();
        public List<ListSubjects> ListSubjects { get; set; } = new List<ListSubjects>();


        public void OnGet()
        {
            // Check if TempData is not null, then set SuccessMessage
            if (TempData["SuccessMessage"] != null)
            {
                SuccessMessage = "Subject instance created successfully!";
            }

            LoadSubjects();

            if (!string.IsNullOrWhiteSpace(SelectedSubject) && StartDate.HasValue && EndDate.HasValue)
            {
                LoadLecturers();
            }
        }

        private void LoadSubjects()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT SubjectCode, SubjectName, SubjectClassification, YearLevel FROM Subjects"; 

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var subject = new ListSubjects
                        {
                            SubjectCode = reader["SubjectCode"].ToString(),
                            SubjectName = reader["SubjectName"].ToString(),
                            SubjectClassification = reader["SubjectClassification"].ToString(),
                            YearLevel = reader["YearLevel"].ToString(),
                        };
                        ListSubjects.Add(subject);
                    }
                }
            }
        }

        private void LoadLecturers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                DECLARE @rating INT = 5;
               
				DECLARE @SelectedSubjectClassifcation NVARCHAR(100);
			

				
				SELECT @SelectedSubjectClassifcation = Subjects.SubjectClassification 
				FROM Subjects 
				WHERE Subjects.SubjectCode = @selectedSubject;

                  SELECT 
                    Users.Email, 
                    Users.FirstName, 
                    Users.LastName,
                    @rating + CASE 
                        WHEN EXISTS (
                            SELECT 1 
                            FROM SubjectInstance
                            INNER JOIN Subjects ON SubjectInstance.SubjectID = Subjects.SubjectID
                            WHERE 
                                SubjectInstance.LecturerID = Lecturers.LecturerID 
                                AND Subjects.SubjectCode = @selectedSubject
                        ) THEN 0 ELSE -2 END 
                    +
                    CASE 

                        WHEN @SelectedSubjectClassifcation IN (Lecturers.Expertise01, Lecturers.Expertise02, Lecturers.Expertise03, Lecturers.Expertise04, Lecturers.Expertise05, Lecturers.Expertise06)
                        THEN 0 ELSE -2 END AS AdjustedRating,
                    CAST(
                        ROUND(ISNULL(SUM(SubjectInstance.Load), 0) / NULLIF(Lecturers.ConcurrentLoadCapacity, 0) * 100, 0) 
                    AS INT) AS LoadCapacityPercentage
                        FROM 
                            Users 
                        LEFT JOIN Lecturers ON Users.UserID = Lecturers.UserID
                        LEFT JOIN SubjectInstance ON Lecturers.LecturerID = SubjectInstance.LecturerID
                        LEFT JOIN Subjects ON Subjects.SubjectName = @selectedSubject
                        WHERE 
                            Users.IsLecturer = 1
                            AND (
                                (SubjectInstance.StartDate <= @endDate AND SubjectInstance.EndDate >= @startDate)
                                OR SubjectInstance.LecturerID IS NULL
                            )
                        GROUP BY 
                            Users.Email, 
                            Users.FirstName, 
                            Users.LastName, 
                            Lecturers.ConcurrentLoadCapacity, 
                            Lecturers.LecturerID,
                            Subjects.SubjectClassification,
                            Lecturers.Expertise01, Lecturers.Expertise02, Lecturers.Expertise03, 
                            Lecturers.Expertise04, Lecturers.Expertise05, Lecturers.Expertise06
                        ORDER BY 
                            AdjustedRating DESC, 
                            LoadCapacityPercentage;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@selectedSubject", SelectedSubject);
                    command.Parameters.AddWithValue("@startDate", StartDate.HasValue ? StartDate.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@endDate", EndDate.HasValue ? EndDate.Value : (object)DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var lecturerInfo = new CSE3PAX.HelpClasses.LecturerInfo
                            {
                                Email = reader["Email"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                AdjustedRating = Convert.ToInt32(reader["AdjustedRating"]),
                                LoadCapacityPercentage = Convert.ToInt32(reader["LoadCapacityPercentage"]) 
                            };
                            Lecturers.Add(lecturerInfo);
                        }

                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            Console.WriteLine($"SelectedSubjectHidden: {SelectedSubjectHidden}");
            LoadSubjects(); // Reload subjects to ensure dropdown is populated

            if (!string.IsNullOrWhiteSpace(SelectedSubject) && StartDate.HasValue && EndDate.HasValue)
            {
                LoadLecturers(); // Reload lecturers based on the selected criteria
            }

            return Page(); // Return the current page with the bound property values
        }

        public async Task<IActionResult> OnPostSubmitDataAsync()
        {
            Console.WriteLine($"SelectedSubjectHidden: {SelectedSubjectHidden}");


            if (string.IsNullOrEmpty(SelectedEmail))
            {
                // Handle the case where SelectedEmail is null or empty.
                
                Console.WriteLine("SelectedEmail is null or empty.");
                return Page();
            }


            Console.WriteLine("Submitted");
            bool developmentRequired = IsDevelopmentRequired;
            double load = CalculateInstanceLoad(NumberOfStudents);

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;

                        command.CommandText = @"
                    DECLARE @UserID INT;
                    DECLARE @LecturerID INT;
                    DECLARE @SubjectID INT;
                    DECLARE @SubjectName NVARCHAR(100);
                    DECLARE @Year NVARCHAR(100);
                    DECLARE @Month NVARCHAR(100);
                    DECLARE @SubjectInstanceCode NVARCHAR(100);
                    DECLARE @RandomAlphaNumeric NVARCHAR(4);
                    DECLARE @SubjectInstanceName NVARCHAR(200);

                    SELECT @UserID = UserID FROM Users WHERE Email = @UserEmailInput; 
                    SELECT @SubjectID = SubjectID FROM Subjects WHERE SubjectCode = @SubjectCodeInput;
                    SELECT @LecturerID = LecturerID FROM Lecturers WHERE UserID = @UserID;
                    SELECT @SubjectName = SubjectName FROM Subjects WHERE SubjectCode = @SubjectCodeInput;
                    SET @Year = CAST(YEAR(@StartDateInput) AS NVARCHAR(4));
                    SET @Month = DATENAME(MONTH, @EndDateInput);

                    SELECT @RandomAlphaNumeric = UPPER(SUBSTRING(CONVERT(NVARCHAR(36), NEWID()), 1, 4));

                    SET @SubjectInstanceCode = @Year + '-' + @SubjectCodeInput;
                    SET @SubjectInstanceName = @Year + '-' + @SubjectCodeInput + '-' + @Month + ' (' + @RandomAlphaNumeric + ')';

                    INSERT INTO SubjectInstance (SubjectID, SubjectInstanceName, SubjectInstanceCode, LecturerID, StartDate, EndDate, SubjectInstanceYear, Load)
                    VALUES (@SubjectID, @SubjectInstanceName, @SubjectInstanceCode, @LecturerID, @StartDateInput, @EndDateInput, @Year, @Load);";

                        command.Parameters.AddWithValue("@UserEmailInput", SelectedEmail); 
                        command.Parameters.AddWithValue("@SubjectCodeInput", SelectedSubjectHidden);
                        command.Parameters.AddWithValue("@StartDateInput", StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : null);
                        command.Parameters.AddWithValue("@EndDateInput", EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : null);

                        // Add the Load value as a parameter
                        command.Parameters.AddWithValue("@Load", load);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                if (developmentRequired)
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandType = System.Data.CommandType.Text;

                            command.CommandText = @"
                            DECLARE @UserID INT;
                            DECLARE @LecturerID INT;
                            DECLARE @SubjectID INT;
                            DECLARE @SubjectName NVARCHAR(100);
                            DECLARE @Year NVARCHAR(100);
                            DECLARE @Month NVARCHAR(100);
                            DECLARE @SubjectInstanceCode NVARCHAR(100);
                            DECLARE @RandomAlphaNumeric NVARCHAR(4);
                            DECLARE @SubjectInstanceName NVARCHAR(200);

                            SELECT @UserID = UserID FROM Users WHERE Email = @UserEmailInput; 
                            SELECT @SubjectID = SubjectID FROM Subjects WHERE SubjectCode = @SubjectCodeInput;
                            SELECT @LecturerID = LecturerID FROM Lecturers WHERE UserID = @UserID;
                            SELECT @SubjectName = SubjectName FROM Subjects WHERE SubjectCode = @SubjectCodeInput;
                            SET @Year = CAST(YEAR(@StartDateInput) AS NVARCHAR(4));
                            SET @Month = DATENAME(MONTH, @EndDateInput);

                            SELECT @RandomAlphaNumeric = UPPER(SUBSTRING(CONVERT(NVARCHAR(36), NEWID()), 1, 4));

                            SET @SubjectInstanceCode = @Year + '-' + @SubjectCode;
                            SET @SubjectInstanceName = @Year + '-' + @SubjectCode + '-' + @Month + ' (' + @RandomAlphaNumeric + ')-Development';

                            INSERT INTO SubjectInstance (SubjectID, SubjectInstanceName, SubjectInstanceCode, LecturerID, StartDate, EndDate, SubjectInstanceYear, Load)
                            VALUES (@SubjectID, @SubjectInstanceName, @SubjectInstanceCode, @LecturerID, @StartDateInput, @EndDateInput, @Year, @Load);";

                            command.Parameters.AddWithValue("@UserEmailInput", SelectedEmail);
                            command.Parameters.AddWithValue("@SubjectCodeInput", SelectedSubjectHidden);
                            command.Parameters.AddWithValue("@StartDateInput", StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : null);
                            command.Parameters.AddWithValue("@EndDateInput", EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : null);

                            // Add the Load value as a parameter
                            command.Parameters.AddWithValue("@Load", load);

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }

                TempData["SuccessMessage"] = "Instance Created";

                // Page redirect
                return RedirectToPage("/Manager/CreateSubjectInstance");

            }
            catch (SqlException ex)
            {

                Debug.WriteLine($"SQL Error: {ex.Message}");
     
            }
            return Page();
}
        

        public double CalculateInstanceLoad(int studentCount)
        {
            double instanceLoad = 1; // Base load for up to 100 students

            if (studentCount > 100)
            {
                // Calculate the number of students over 100
                int extraStudents = studentCount - 100;

                // Calculate the increase in load. Each set of 20 students over 100 increases the load by 0.1
                double loadIncrease = (extraStudents / 20.0) * 0.1;

                // Update the instance load with the calculated increase
                instanceLoad += loadIncrease;
            }
            return instanceLoad;
        }
    }
}