using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CSE3PAX.HelpClasses; // Adjust this namespace based on your actual namespace
using System.Reflection.PortableExecutable;

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

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public List<CSE3PAX.HelpClasses.LecturerInfo> Lecturers { get; set; } = new List<CSE3PAX.HelpClasses.LecturerInfo>();
        public List<ListSubjects> ListSubjects { get; set; } = new List<ListSubjects>();


        public void OnGet()
        {
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
                var query = "SELECT SubjectCode, SubjectName, SubjectClassification, YearLevel FROM Subjects"; // Adjust this query as needed

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
                DECLARE @selectedSubject NVARCHAR(255) = @selectedSubjectParam; -- Parameterized
                DECLARE @startDate DATETIME = @startDateParam; -- Parameterized
                DECLARE @endDate DATETIME = @endDateParam; -- Parameterized

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
                AND Subjects.SubjectName = @selectedSubject
        ) THEN 0 ELSE -2 END 
    +
    CASE 
        WHEN Subjects.SubjectClassification IN (Lecturers.Expertise01, Lecturers.Expertise02, Lecturers.Expertise03, Lecturers.Expertise04, Lecturers.Expertise05, Lecturers.Expertise06)
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
                    command.Parameters.AddWithValue("@selectedSubjectParam", SelectedSubject);
                    command.Parameters.AddWithValue("@startDateParam", StartDate.HasValue ? StartDate.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@endDateParam", EndDate.HasValue ? EndDate.Value : (object)DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var lecturerInfo = new CSE3PAX.HelpClasses.LecturerInfo
                            {
                                Email = reader["Email"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                AdjustedRating = Convert.ToInt32(reader["AdjustedRating"]), // Make sure the SQL alias matches
                                LoadCapacityPercentage = Convert.ToInt32(reader["LoadCapacityPercentage"]) // Ditto
                            };
                            Lecturers.Add(lecturerInfo);
                        }

                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            Console.WriteLine($"SelectedSubject: {SelectedSubject}");
            LoadSubjects(); // Reload subjects to ensure dropdown is populated

            if (!string.IsNullOrWhiteSpace(SelectedSubject) && StartDate.HasValue && EndDate.HasValue)
            {
                LoadLecturers(); // Reload lecturers based on the selected criteria
            }

            return Page(); // Return the current page with the bound property values
        }
    }
}