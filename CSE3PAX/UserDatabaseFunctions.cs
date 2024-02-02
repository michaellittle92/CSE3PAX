using System.Data.SqlClient;

namespace CSE3PAX
{
    public class UserDatabaseFunctions
    {

        private readonly string _connectionString;

        public UserDatabaseFunctions(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int VerifyCredentials(string email, string password)
        {
            //this is the value that is returned 
            int userId = 0;

            SqlConnection con = new SqlConnection(_connectionString);


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

        public void ResetPassword(int userID, string newUserPassword)
        {

            Guid newUserGuid = Guid.NewGuid();
            string hashedPassword = Security.HashSHA256(newUserPassword + newUserGuid.ToString());

            SqlConnection con = new SqlConnection(_connectionString);
            using (SqlCommand cmd = new SqlCommand("UPDATE[Users] SET Password = @newUserPassword, UserGuid = @userGuid, IsPasswordResetRequired = 0 WHERE UserID = @userid;", con))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@newUserPassword", hashedPassword);
                cmd.Parameters.AddWithValue("@userGuid", newUserGuid);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }


    }
}
