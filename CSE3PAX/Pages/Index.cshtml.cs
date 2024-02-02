using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CSE3PAX; 
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace CSE3PAX.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        //-----------------------------------------------


        [BindProperty]
        [Required(ErrorMessage = "Please enter your email address"), EmailAddress]
        public string Email { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; } = "";


        public string errorMessage { get; set; } = "";
        public string successMessage { get; set; } = "";

   

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            // Check if user is already logged in
            if (HttpContext.Session.Get("isLecturer") != null)
            {
                // Redirect to home page
                context.Result = new RedirectToPageResult("/Lecturer/LecturerIndex");
            }
        }

        public void OnGet()
        {

        }

        public void OnPost() { 

            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Password: {Password}");
            if (!ModelState.IsValid)
            {
              errorMessage = "Data Validation Failed";
                return;
            }

            //Successfull data validation


            //connection to database and check user credentials 

            try {
                using (SqlConnection connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    string sql = "SELECT * FROM [Users] WHERE Email = @Email";

                    using (SqlCommand command = new SqlCommand(sql, connection)) { 
                        command.Parameters.AddWithValue("@Email", Email);
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //User Exists

                                int userID = reader.GetInt32(0);
                                string email = reader.GetString(1);
                                string hashedPassword = reader.GetString(2);
                                Guid userGuidObj = reader.GetGuid(3);
                                string userGuid = userGuidObj.ToString();
                                string firstName = reader.GetString(4);
                                string lastName = reader.GetString(5);
                                bool isAdministrator = reader.GetBoolean(6);
                                bool isManager = reader.GetBoolean(7);
                                bool isLecturer = reader.GetBoolean(8);
                                bool isPasswordResetRequired = reader.GetBoolean(9);
                                string createdOn = reader.GetDateTime(10).ToString("dd/MM/yyyy");

                                //verify password

                               string CalculatedPassword = Security.HashSHA256(Password + userGuid);

                                bool isPasswordCorrect = CalculatedPassword == hashedPassword;

                                // Debugging or logging
                                Console.WriteLine($"Input password: {Password}");
                                Console.WriteLine($"Database password hash: {hashedPassword}");
                                Console.WriteLine($"Calculated password hash: {CalculatedPassword}");
                                Console.WriteLine($"Password match: {isPasswordCorrect}");

                                //if the password is correct the result of the hash is the same as the one in the database 
                                if (CalculatedPassword == hashedPassword)
                                {
                                    //successful password verifciation => initialize the session variables
                                    HttpContext.Session.SetInt32("UserID", userID);
                                    HttpContext.Session.SetString("Email", email);
                                    HttpContext.Session.SetString("FirstName", firstName);
                                    HttpContext.Session.SetString("LastName", lastName);
                                    HttpContext.Session.SetBoolean("isAdministrator", isAdministrator);
                                    HttpContext.Session.SetBoolean("isManager", isManager);
                                    HttpContext.Session.SetBoolean("isLecturer", isLecturer);
                                    HttpContext.Session.SetBoolean("isPasswordResetRequired", isPasswordResetRequired);
                                    HttpContext.Session.SetString("createdOn", createdOn);


                                    if (isAdministrator == true)
                                    {
                                        Response.Redirect("/Admin/AdminIndex");
                                    }

                                    else if (isManager == true)
                                    {
                                        Response.Redirect("/Manager/ManagerIndex");
                                    }
                                    else {
                                        Response.Redirect("/Lecturer/LecturerIndex");
                                    }
                                }
                            }
                        }
                    }

                }



            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            //Wrong email or password

            errorMessage = "Wrong email or password";
        }



    }


}