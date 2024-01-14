using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleScheduler
{
    public class AdministratorDatabaseFunctions
    {
        private static string connectionString = "Data Source=DESKTOP-GOQKLFC\\SQLEXPRESS;Initial Catalog=ConsoleScheduler;Integrated Security=True";

        public static bool DBAddUser(string newUserEmail, string newUserPassword, string newUserFirstName, string newUserLastName, bool isNewUserAdmin, bool IsNewUserManager, bool isNewUserLecturer, List<string> expertise)
        {

            int newUserId = 0;

            Guid userGuid = Guid.NewGuid();
            string hashedPassword = Security.HashSHA256(newUserPassword + userGuid.ToString());

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string insertQuery = "INSERT INTO [Users] " +
                    "OUTPUT INSERTED.UserID " + // Output the inserted UserID
                    "VALUES (@email, @hashedPassword, @userGuid, @firstname, @lastname, @isAdmin, @isManager, @isLecturer, @isPasswordResetRequired);";

                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@email", newUserEmail);
                    cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                    cmd.Parameters.AddWithValue("@userGuid", userGuid);
                    cmd.Parameters.AddWithValue("@firstname", newUserFirstName);
                    cmd.Parameters.AddWithValue("@lastname", newUserLastName);
                    cmd.Parameters.AddWithValue("@isAdmin", isNewUserAdmin);
                    cmd.Parameters.AddWithValue("@isManager", IsNewUserManager);
                    cmd.Parameters.AddWithValue("@isLecturer", isNewUserLecturer);
                    cmd.Parameters.AddWithValue("@isPasswordResetRequired", true);

                    con.Open();
                    // Execute the insert query and retrieve the new UserID
                    newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();

                    Console.WriteLine($"New User ID = {newUserId}");
                    Console.ReadKey();

                    if (isNewUserLecturer == true)
                    {
                        DBAddLecturer(newUserId, expertise);
                    }

                }
            }

            return true;
        }

        public static bool DBAddLecturer(int newUseriD, List<string> expertise)
        {
            //Connect to DB 
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand(


                               "INSERT INTO Lecturers(UserID, Expertise01, Expertise02, Expertise03, Expertise04, Expertise05, Expertise06) " +
                                              "VALUES (@userID, @expertise0, @expertise1, @expertise2, @expertise3, @expertise4, @expertise5)", con))
            {
                cmd.Parameters.AddWithValue("@userID", newUseriD);
                cmd.Parameters.AddWithValue("@expertise0", expertise[0]);
                cmd.Parameters.AddWithValue("@expertise1", expertise[1]);
                cmd.Parameters.AddWithValue("@expertise2", expertise[2]);
                cmd.Parameters.AddWithValue("@expertise3", expertise[3]);
                cmd.Parameters.AddWithValue("@expertise4", expertise[4]);
                cmd.Parameters.AddWithValue("@expertise5", expertise[5]);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return true;
        }

        public static void DBReadAllUsers(string connectionString)
        {
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [Users]", con))
            {
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Console.WriteLine($"{dr["UserID"]}, {dr["Email"]}, {dr["FirstName"]}, {dr["LastName"]}, {dr["IsAdmin"]}, {dr["IsManager"]}, {dr["IsLecturer"]}");
                }
                dr.Close();
                con.Close();
            }
        }

        public static void DBReadAllAdministrators(string connectionString)
        {
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [Users] WHERE IsAdmin = 1", con))
            {
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Console.WriteLine($"{dr["UserID"]}, {dr["Email"]}, {dr["FirstName"]}, {dr["LastName"]}, {dr["IsAdmin"]}, {dr["IsManager"]}, {dr["IsLecturer"]}");
                }
                dr.Close();
                con.Close();
            }

        }
        public static void DBReadAllManagers(string connectionString)
        {
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [Users] WHERE IsManager = 1", con))
            {
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Console.WriteLine($"{dr["UserID"]}, {dr["Email"]}, {dr["FirstName"]}, {dr["LastName"]}, {dr["IsAdmin"]}, {dr["IsManager"]}, {dr["IsLecturer"]}");
                }
                dr.Close();
                con.Close();
            }
        }

        public static void DBReadAllLecturers(string connectionString)
        {
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [Users] WHERE IsLecturer = 1", con))
            {
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Console.WriteLine($"{dr["UserID"]}, {dr["Email"]}, {dr["FirstName"]}, {dr["LastName"]}, {dr["IsAdmin"]}, {dr["IsManager"]}, {dr["IsLecturer"]}");
                }
                dr.Close();
                con.Close();
            }
        }

        public static void DBUpdateUser(string connectionString, int userID) { 
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("UPDATE [Users] SET Email = @email,"
                               + "FirstName = @firstName, LastName = @lastName, IsAdmin = @isAdmin, IsManager = @isManager, IsLecturer = @isLecturer WHERE UserID = @userID", con))
            {
                Console.WriteLine("Enter new email: ");
                string newEmail = Console.ReadLine();
                Console.WriteLine("Enter new first name: ");
                string newFirstName = Console.ReadLine();
                Console.WriteLine("Enter new last name: ");
                string newLastName = Console.ReadLine();
                Console.WriteLine("Is the user an administrator? (y/n)");
                string newIsAdmin = Console.ReadLine();
                Console.WriteLine("Is the user a manager? (y/n)");
                string newIsManager = Console.ReadLine();
                Console.WriteLine("Is the user a lecturer? (y/n)");
                string newIsLecturer = Console.ReadLine();

                if (newIsAdmin == "y")
                {
                    newIsAdmin = "1";
                }
                else
                {
                    newIsAdmin = "0";
                }

                if (newIsManager == "y")
                {
                    newIsManager = "1";
                }
                else
                {
                    newIsManager = "0";
                }

                if (newIsLecturer == "y")
                {
                    newIsLecturer = "1";
                }
                else
                {
                    newIsLecturer = "0";
                }

                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@email", newEmail);
                cmd.Parameters.AddWithValue("@firstName", newFirstName);
                cmd.Parameters.AddWithValue("@lastName", newLastName);
                cmd.Parameters.AddWithValue("@isAdmin", newIsAdmin);
                cmd.Parameters.AddWithValue("@isManager", newIsManager);
                cmd.Parameters.AddWithValue("@isLecturer", newIsLecturer);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void DBRemoveUser(string connectionString, int userID)
            {
            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("DELETE FROM [Users] WHERE UserID = @userID", con))
                {
                cmd.Parameters.AddWithValue("@userID", userID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            }

        public static void ResetPassword(string connectionString, int userID, string newUserPassword)
        {

            Guid newUserGuid = Guid.NewGuid();
            string hashedPassword = Security.HashSHA256(newUserPassword + newUserGuid.ToString());

            SqlConnection con = new SqlConnection(connectionString);
            using (SqlCommand cmd = new SqlCommand("UPDATE[Users] SET Password = '@newUserPassword', UserGuid = @userGuid, IsPasswordResetRequired = 0 WHERE UserID = @userid;", con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@newUserPassword", hashedPassword);
                cmd.Parameters.AddWithValue("@userGuid", newUserGuid);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }


        //------------------------//
    }
}


