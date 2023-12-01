using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CSE3PAX.Pages
{
    public class IndexModel : PageModel
    {
        public List<LecturerInfo> listLecturers = new List<LecturerInfo>();

        public void OnGet() {
            try
            {
                String connectionString = "Data Source=.\\sqlexpress;Initial Catalog=schedulingDB;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM lecturers";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LecturerInfo info = new LecturerInfo();
                                info.userID = "" + reader.GetInt32(0);
                                info.LastName = reader.GetString(1);
                                info.FirstName = reader.GetString(2);
                                info.Email = reader.GetString(3);
                                info.ConcurrentLoadCapacity = "" + (reader.GetInt32(5));
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
    public class LecturerInfo {
        public String userID;
        public String LastName;
        public String FirstName;
        public String Email;
        public String ConcurrentLoadCapacity;
        public String ExpertiseFeild01;
        //public String ExpertiseFeild02;
       // public String ExpertiseFeild03;
        //public String ExpertiseFeild04;
       // public String ExpertiseFeild05;


    }
}