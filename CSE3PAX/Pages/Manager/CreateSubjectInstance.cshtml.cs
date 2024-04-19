using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using CSE3PAX.HelpClasses;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // Bind properties
        [BindProperty(SupportsGet = true)]
        public string SelectedSubject { get; set; }
        public string SuccessMessage { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }
        public DateTime? CalculatedEndDate { get; set; }
        [BindProperty]
        public string SelectedFirstName { get; set; }
        [BindProperty]
        public string SelectedLastName { get; set; }
        [BindProperty]
        public string SelectedEmail { get; set; }
        [BindProperty]
        public int NumberOfStudents { get; set; }
        [BindProperty]
        public string CheckboxState { get; set; }

        [BindProperty]
        public string SelectedSubjectHidden { get; set; }

        public List<CSE3PAX.HelpClasses.LecturerInfo> Lecturers { get; set; } = new List<CSE3PAX.HelpClasses.LecturerInfo>();
        public List<ListSubjects> ListSubjects { get; set; } = new List<ListSubjects>();

        /*
        Handles the HTTP GET request for the page.
        - Checks if TempData contains a success message and sets SuccessMessage accordingly.
        - Calls LoadSubjects method to populate the Subjects list.
        - Calls LoadLecturers method if SelectedSubject, StartDate, and EndDate are not null or empty.
        - Loads subject instance details if selectedSubjectInstance is provided.
        */
        public void OnGet(int? selectedSubjectInstance)
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

            if (selectedSubjectInstance.HasValue)
            {
                Debug.WriteLine($"SelectedSubjectInstance: {selectedSubjectInstance}");
                LoadSubjectInstanceDetails(selectedSubjectInstance.Value);
            }
        }

        /*
        Loads details of a specific subject instance from the database.
        - Constructs a SQL query to retrieve subject instance details along with related lecturer and user information.
        - Populates page properties with the retrieved data, such as SelectedSubject, StartDate, EndDate, SelectedFirstName, SelectedLastName, SelectedEmail, and NumberOfStudents.
        */
        private void LoadSubjectInstanceDetails(int instanceId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                SELECT 
                    SubjectInstanceID, 
                    SubjectInstanceName, 
                    SubjectInstanceCode, 
                    si.StartDate, 
                    si.EndDate, 
                    si.LecturerID, 
                    si.Load,
                    Users.UserID, 
                    Users.FirstName, 
                    Users.LastName, 
                    Users.Email,
					Subjects.SubjectName
                FROM 
                    SubjectInstance AS si
                INNER JOIN 
                    Lecturers ON si.LecturerID = Lecturers.LecturerID
                INNER JOIN 
                    Users ON Lecturers.UserID = Users.UserID

				INNER JOIN 
					Subjects ON si.SubjectID = Subjects.SubjectID
                WHERE 
                    SubjectInstanceID = @instanceId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@instanceId", instanceId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SelectedSubject = reader["SubjectName"].ToString();
                            StartDate = Convert.ToDateTime(reader["StartDate"]);
                            EndDate = Convert.ToDateTime(reader["EndDate"]);
                            SelectedFirstName = reader["FirstName"].ToString();
                            SelectedLastName = reader["LastName"].ToString();
                            SelectedEmail = reader["Email"].ToString();

                            Debug.WriteLine($"SelectedSubject: {SelectedSubject}");

                            double instanceLoad = Convert.ToDouble(reader["Load"]);

                            NumberOfStudents = CalculateStudentCount(instanceLoad);
                        }
                    }
                }
            }
        }

        /*
        Loads subjects from the database and populates the ListSubjects property.
        - Constructs a SQL query to select all subjects from the Subjects table.
        - Iterates through the query results and creates ListSubjects objects.
        - Sets the ListSubjects property to the generated list of subjects.
        - Creates a SelectList object from the subjects list and sets it in the ViewData for use in the page.
        */
        private void LoadSubjects()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT SubjectCode, SubjectName, SubjectClassification, YearLevel FROM Subjects";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    var subjects = new List<ListSubjects>();
                    while (reader.Read())
                    {
                        var subject = new ListSubjects
                        {
                            SubjectCode = reader["SubjectCode"].ToString(),
                            SubjectName = reader["SubjectName"].ToString(),
                            SubjectClassification = reader["SubjectClassification"].ToString(),
                            YearLevel = reader["YearLevel"].ToString(),
                        };
                        subjects.Add(subject);
                    }
                    ListSubjects = subjects;
                    ViewData["Subjects"] = new SelectList(subjects, "SubjectCode", "SubjectName", SelectedSubject);
                }
            }
        }

        /*
        Loads lecturers related to the selected subject from the database.
        - Calculates EndDate as 12 days after StartDate.
        - Constructs a SQL query to retrieve lecturer information, adjust ratings, and calculate load capacity percentages.
        - Executes the query with parameters for the selected subject, StartDate, and EndDate.
        - Iterates through the query results and creates LecturerInfo objects to store lecturer details.
        */
        private void LoadLecturers()
        {

            // Calculate EndDate. 12 days after StartDate
            EndDate = StartDate.Value.AddDays(7 * 12);
            Console.WriteLine("End Date " + EndDate);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                /*
                 * SQL query to calculate adjusted rating and load capacity percentage for lecturers based on selected criteria.
                 * Calculates adjusted rating based on whether the lecturer has taught the selected subject before and expertise match.
                 * Adjusted rating is determined by adding 5 to the rating if the lecturer has taught the subject, otherwise subtracting 2.
                 * Expertise match adds 0 to the rating if the lecturer's expertise matches the subject's classification, otherwise subtracting 2.
                 * Load capacity percentage is calculated based on the lecturer's concurrent load capacity and the sum of load for their instances.
                 * Parameters:
                 *     - @selectedSubject: The subject code for which the rating is calculated.
                 *     - @endDate: End date for the selected period.
                 *     - @startDate: Start date for the selected period.
                 * Query joins Users, Lecturers, SubjectInstance, and Subjects tables to gather required data.
                 * Filters lecturers who are also users and lecturers with instances falling within the selected period.
                 * Groups the result by lecturer and orders by adjusted rating and load capacity percentage.
                 */
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

        /*
        Handles the HTTP POST request for the page.
        - Outputs the value of SelectedSubjectHidden to the console for debugging purposes.
        - Reloads subjects to ensure the dropdown is populated.
        - Reloads lecturers based on the selected criteria if SelectedSubject and StartDate are not null or empty.
        - Returns the current page with the bound property values.
        */
        public IActionResult OnPost()
        {
            Console.WriteLine($"SelectedSubjectHidden: {SelectedSubjectHidden}");
            LoadSubjects(); // Reload subjects to ensure dropdown is populated

            if (!string.IsNullOrWhiteSpace(SelectedSubject) && StartDate.HasValue)
            {
                LoadLecturers(); // Reload lecturers based on the selected criteria
            }
            return Page(); // Return the current page with the bound property values
        }

        /*
        Handles the asynchronous HTTP POST request for submitting subject instance data.
        - Outputs the value of SelectedSubjectHidden to the console for debugging purposes.
        - Checks if SelectedEmail is null or empty, and returns the current page if true.
        - Sets up variables and parameters for SQL queries to insert subject instance data into the database.
        - Executes SQL queries to insert subject instance data into the database.
        - Sets TempData["SuccessMessage"] to indicate successful creation of the instance.
        - Redirects the user to the page for creating a subject instance after successful creation.
        - Handles SQL exceptions and outputs error messages to the debug console.
        - Returns the current page if an exception occurs.
        */

        public async Task<IActionResult> OnPostSubmitDataAsync()
        {
            bool developmentRequired;
            double load = CalculateInstanceLoad(NumberOfStudents);

            if (string.IsNullOrEmpty(SelectedEmail))
            {
                Console.WriteLine("SelectedEmail is null or empty.");
                return Page();
            }
            if (CheckboxState == "checked")
            {
                developmentRequired = true;
            }
            else
            {
                developmentRequired = false;
            }
            

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;

                        // Common SQL for setting up IDs and names
                        string prepCmdText = @"
                    DECLARE @UserID INT;
                    DECLARE @LecturerID INT;
                    DECLARE @SubjectID INT;
                    DECLARE @SubjectName NVARCHAR(100);
                    DECLARE @Year NVARCHAR(100);
                    DECLARE @Month NVARCHAR(100);
                    DECLARE @RandomAlphaNumeric NVARCHAR(4);

                    SELECT @UserID = UserID FROM Users WHERE Email = @UserEmailInput;
                    SELECT @SubjectID = SubjectID FROM Subjects WHERE SubjectCode = @SubjectCodeInput;
                    SELECT @LecturerID = LecturerID FROM Lecturers WHERE UserID = @UserID;
                    SET @Year = CAST(YEAR(@StartDateInput) AS NVARCHAR(4));
                    SET @Month = DATENAME(MONTH, @EndDateInput);
                    SELECT @RandomAlphaNumeric = UPPER(SUBSTRING(CONVERT(NVARCHAR(36), NEWID()), 1, 4));
                ";
                        command.CommandText = prepCmdText + @"
                    DECLARE @SubjectInstanceName NVARCHAR(200) = @Year + '-' + @SubjectCodeInput + '-' + @Month + ' (' + @RandomAlphaNumeric + ')';
                    DECLARE @SubjectInstanceCode NVARCHAR(100) = @Year + '-' + @SubjectCodeInput;

                    INSERT INTO SubjectInstance (SubjectID, SubjectInstanceName, SubjectInstanceCode, LecturerID, StartDate, EndDate, SubjectInstanceYear, Load)
                    VALUES (@SubjectID, @SubjectInstanceName, @SubjectInstanceCode, @LecturerID, @StartDateInput, @EndDateInput, @Year, @Load);

                    IF @DevelopmentRequired = 1
                    BEGIN
                        SET @SubjectInstanceName += '-Development';
                        INSERT INTO SubjectInstance (SubjectID, SubjectInstanceName, SubjectInstanceCode, LecturerID, StartDate, EndDate, SubjectInstanceYear, Load)
                        VALUES (@SubjectID, @SubjectInstanceName, @SubjectInstanceCode, @LecturerID, @StartDateInput, @EndDateInput, @Year, @Load);
                    END
                ";

                        command.Parameters.AddWithValue("@UserEmailInput", SelectedEmail);
                        command.Parameters.AddWithValue("@SubjectCodeInput", SelectedSubjectHidden);
                        command.Parameters.AddWithValue("@StartDateInput", StartDate.HasValue ? StartDate.Value.ToString("yyyy-MM-dd") : null);
                        command.Parameters.AddWithValue("@EndDateInput", EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : null);
                        command.Parameters.AddWithValue("@Load", load);
                        command.Parameters.AddWithValue("@DevelopmentRequired", developmentRequired ? 1 : 0);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                TempData["SuccessMessage"] = "Instance Created";
                return RedirectToPage("/Manager/CreateSubjectInstance");
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                return Page();
            }
        }


        /*
        Calculates the load for a subject instance based on the number of students.
        - Initializes instanceLoad to 1, representing the base load for up to 100 students.
        - If the studentCount exceeds 100, calculates the extraStudents beyond 100.
        - Calculates the loadIncrease based on the extraStudents. Each set of 20 students over 100 increases the load by 0.1.
        - Updates the instanceLoad with the calculated loadIncrease.
        - Returns the calculated instanceLoad.
        */
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

        /*
        Calculates the number of students required to achieve a target load for a subject instance.
        - Initializes studentCount to 100, representing the base number of students.
        - Initializes instanceLoad to 1.0, representing the base load for up to 100 students.
        - Checks if the targetLoad exceeds the instanceLoad.
        - If targetLoad is greater than instanceLoad, iterates until instanceLoad matches or exceeds targetLoad.
        - Calculates the load for the next 20 students (0.1 increase per 20 students).
        - If instanceLoad exceeds targetLoad, exits the loop.
        - Increases studentCount by 20 for each iteration.
        - Returns the calculated studentCount.
        */
        public int CalculateStudentCount(double targetLoad)
        {
            int studentCount = 100; // Base number of students
            double instanceLoad = 1.0; // Base load for up to 100 students

            if (targetLoad > instanceLoad)
            {
                while (instanceLoad < targetLoad)
                {
                    // Calculate load for next 20 students
                    instanceLoad += 0.1;

                    if (instanceLoad > targetLoad)
                        break;

                    // Increase student count by 20
                    studentCount += 20;
                }
            }
            return studentCount;
        }
    }
}