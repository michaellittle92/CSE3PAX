using CSE3PAX.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;


namespace CSE3PAX.Pages.Admin
{
    public class ReadSubjectModel : PageModel
    {

        private readonly IConfiguration _configuration;

        // String to store DefaultConnection from configuration file
        private readonly string _connectionString;

        public ReadSubjectModel(IConfiguration configuration)
        {
            // Check if a valid configuration is provided
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
        }

        public List<ListSubjects> ListSubjects { get; set; } = new List<ListSubjects>();

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "SELECT SubjectCode, SubjectName, SubjectClassification, YearLevel FROM Subjects;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
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
                                //For testing
                                //Console.WriteLine($"SubjectCode: {subject.SubjectCode}, SubjectName: {subject.SubjectName}, SubjectDescription: {subject.SubjectDescription}, SubjectYear: {subject.SubjectYear}");

                            }
                        }
                    }
                }
            }


            catch (Exception ex) { }
        }
    }
}
