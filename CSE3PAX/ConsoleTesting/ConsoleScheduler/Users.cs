using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleScheduler
{
    public class Users
    {
        private static string connectionString = "Data Source=DESKTOP-GOQKLFC\\SQLEXPRESS;Initial Catalog=ConsoleScheduler;Integrated Security=True";

        

        public static int VerifyCredentials(string email, string password)
        {
            //this is the value that is returned 
            int userId = 0;

            SqlConnection con = new SqlConnection(connectionString);

            using (SqlCommand cmd = new SqlCommand("SELECT UserId, Password, UserGUID FROM [Users] WHERE Email=@email", con))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //dr.read = we found user(s) with the matching username. 
                    int dbUserId = Convert.ToInt32(dr["UserId"]);
                    string dbPassword = Convert.ToString(dr["Password"]);
                    string dbUserGuid = Convert.ToString(dr["UserGuid"]);


                    // hash the UserGuid from the database with the password we want to check.
                    string hashedPassword = Security.HashSHA256(password + dbUserGuid);

                    //if the password is correct the result of the hash is the same as the one in the database 
                    if (dbPassword == hashedPassword)
                    {
                        userId = dbUserId;

                    }
                }
                con.Close();
            }
            return userId;
        }

        public static string PrivledgesCheck(int userID)
        {
            string privledges = string.Empty;
            // use userID to check if user is admin, manager or lecturer.
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("SELECT IsAdmin, IsManager, IsLecturer FROM [Users] Where UserID=@userID", con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                { //checks if there are any results, but there always should be
                    bool isAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    bool isManager = Convert.ToBoolean(dr["IsManager"]);
                    bool isLecturer = Convert.ToBoolean(dr["IsLecturer"]);

                    if (isAdmin)
                    {
                        privledges = "Administrator";
                    }
                    else if (isManager)
                    {
                        privledges = "Manager";
                    }
                    else if (isLecturer)
                    {
                        privledges = "Lecturer";
                    }
                    else
                    {
                        privledges = "No privledges found";
                    }
                }
                else
                {
                    Console.WriteLine("No user privledges found, please contact your system administrator");
                }
                dr.Close();
                con.Close();

                // return the privledges as a string
                return privledges;
            }
        }

        public static void IsPasswordResetRequired(int userID) {

            string newUserPassword;

            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("SELECT IsPasswordResetRequired FROM [USERS] WHERE UserID = @UserID", con))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    bool isPasswordResetRequired = Convert.ToBoolean(dr["IsPasswordResetRequired"]);

                    if (isPasswordResetRequired)
                    {
                        Console.WriteLine("Your password has expired, please reset your password");
                        Console.WriteLine("--------------------------");

                        Console.WriteLine("New Password: ");
                        newUserPassword = Console.ReadLine();
                        AdministratorDatabaseFunctions.ResetPassword(connectionString, userID, newUserPassword);
                    }
                }
            }
        }


    }
}




