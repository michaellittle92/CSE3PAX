using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages
{
    public class LecturerIndexModel : PageModel
    {

        //LecturerInfo list
        public List<LecturerInfo> listLecturers = new List<LecturerInfo>();

        public void OnGet() {
            try
            {

                //AWS SQL Database connection string 
                String connectionString = "Data Source=latrobeschedulesystem.cqdeypb3gbwr.us-east-1.rds.amazonaws.com,1433;Initial Catalog=LaTrobeScheduleSystemDB;Integrated Security=False;User ID=admin;Password=Bghas23fn74!!";

                //SqlConnection connect to SQL DB
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    //Open SQL connection
                    connection.Open();

                    //sql string variable to read all entries from lecturers sql table
                    String sql = "SELECT * FROM lecturers";

                    //SqlCommand execute command against SQL db
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        //SqlDataReader to read db/tables
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            //While loop to read each entry in lecturer table
                            while (reader.Read())
                            {

                                //Create LecturerInfo object and read attributes
                                LecturerInfo info = new LecturerInfo();
                                info.userID = "" + reader.GetInt32(0);
                                info.FirstName = reader.GetString(1);
                                info.LastName = reader.GetString(2);
                                info.Email = reader.GetString(3);
                               info.ConcurrentLoadCapacity = reader.GetString(5);
                               info.ExpertiseFeild01 = reader.GetString(6);
                                //info.ExpertiseFeild02 = reader.GetString(7);
                               // info.ExpertiseFeild03 = reader.GetString(8);
                              //  info.ExpertiseFeild04 = reader.GetString(9);
                               // info.ExpertiseFeild05 = reader.GetString(10);


                                listLecturers.Add(info);
                            }
                        }
                    }
                }
            }
            catch(Exception ex) { 
            }
        }
    }

    //Lecturer info attributes
    public class LecturerInfo {
        public String userID;
        public String LastName;
        public String FirstName;
        public String Email;
        public String ConcurrentLoadCapacity;
        public String ExpertiseFeild01;
        public String ExpertiseFeild02;
        public String ScheduleAssigned;
        public String Address;
        public String Phone;
        public String EmploymentStatus;
    }
}