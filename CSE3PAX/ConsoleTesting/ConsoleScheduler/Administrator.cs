using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleScheduler
{
    public class Administrator
    {

        private static string connectionString = "Data Source=DESKTOP-GOQKLFC\\SQLEXPRESS;Initial Catalog=ConsoleScheduler;Integrated Security=True";


        public static void AdministratorMenu()
        {
            bool coninuteAdminMenu = true;
            char adminMenuChoice;

            while (coninuteAdminMenu == true)
            {
                Console.WriteLine("\nAdministrator Menu");
                Console.WriteLine("-------------------------");

                Console.WriteLine("1. Add a new user");
                Console.WriteLine("2. Read users");
                Console.WriteLine("3. Update a user");
                Console.WriteLine("4. Delete a user");

                Console.WriteLine("Press Q to exit");

                adminMenuChoice = Console.ReadKey().KeyChar;
                Console.WriteLine($"You have pressed {adminMenuChoice}");

                if (adminMenuChoice == 'q' || adminMenuChoice == 'Q')
                {
                    coninuteAdminMenu = false;
                }
                else if (adminMenuChoice == '1')
                {
                    Console.WriteLine("Add a new user");
                    AddUser();


                }
                else if (adminMenuChoice == '2')
                {
                    int readMenuChoice;

                    Console.WriteLine("1. View All Users");
                    Console.WriteLine("2. View All Administrators");
                    Console.WriteLine("3. View All Managers");
                    Console.WriteLine("4. View All Lecturers");

                    readMenuChoice = Convert.ToInt32(Console.ReadLine());

                    switch(readMenuChoice)
                    {
                        case 1:
                            AdministratorDatabaseFunctions.DBReadAllUsers(connectionString);
                            break;
                        case 2:
                            AdministratorDatabaseFunctions.DBReadAllAdministrators(connectionString);
                            break;
                        case 3:
                            AdministratorDatabaseFunctions.DBReadAllManagers(connectionString);
                            break;
                        case 4:
                            AdministratorDatabaseFunctions.DBReadAllLecturers(connectionString);
                            break;
                        default:
                            Console.WriteLine("Invalid Choice");
                            break;
                    }
                }
                else if (adminMenuChoice == '3')
                {
                    Console.WriteLine("Update a user");
                    Console.WriteLine("Which user would you like to update?");
                    int userID = Convert.ToInt32(Console.ReadLine());
                    AdministratorDatabaseFunctions.DBUpdateUser(connectionString, userID);
                }
                else if (adminMenuChoice == '4')
                {
                    Console.WriteLine("Remove a user");
                    Console.WriteLine("Which user would you like to remove?");
                    int userID = Convert.ToInt32(Console.ReadLine());
                    AdministratorDatabaseFunctions.DBRemoveUser(connectionString, userID);
                }
                else
                {
                    Console.WriteLine("Invalid choice");
                }
            }

        }

        public static void AddUser()
        {
            string newUserEmail;
            string newUserPassword;
            string newUserFirstName;
            string newUserLastName;
            bool isNewUserAdmin;
            string isNewUserAdminChoice;
            bool IsNewUserManager;
            string isNewUserManagerChoice;
            bool isNewUserLecturer;
            string isNewUserLecturerChoice;
            List<String> expertise = new List<string>();

            Console.WriteLine("Add User");
            Console.WriteLine("--------------------");

            Console.WriteLine("Email Address: ");
            newUserEmail = Console.ReadLine();

            Console.WriteLine("Password: ");
            newUserPassword = Console.ReadLine();

            Console.WriteLine("First Name");
            newUserFirstName = Console.ReadLine();

            Console.WriteLine("Last Name");
            newUserLastName = Console.ReadLine();

            Console.WriteLine("Is this user an administrator(Y/N)");
            isNewUserAdminChoice = Console.ReadLine();

            if (isNewUserAdminChoice == "Y" || isNewUserAdminChoice == "y")
            {
                isNewUserAdmin = true;
            }
            else
            {
                isNewUserAdmin = false;
            }

            Console.WriteLine("Is this user a manager(Y/N)");
            isNewUserManagerChoice = Console.ReadLine();

            if (isNewUserManagerChoice == "Y" || isNewUserManagerChoice == "y")
            {
                IsNewUserManager = true;
            }
            else
            {
                IsNewUserManager = false;
            }

            Console.WriteLine("Is this user a lecturer(Y/N)");
            isNewUserLecturerChoice = Console.ReadLine();

            if (isNewUserLecturerChoice == "Y" || isNewUserLecturerChoice == "y")
            {
                isNewUserLecturer = true;
            }
            else
            {
                isNewUserLecturer = false;
            }

            if (isNewUserLecturer == true)
            {


                for (int i = 0; i < 6; i++)
                {
                    Console.WriteLine($"Expertise 0 {i + 1}:");
                    expertise.Add(Console.ReadLine());


                }
            }
            AdministratorDatabaseFunctions.DBAddUser(newUserEmail, newUserPassword, newUserFirstName, newUserLastName, isNewUserAdmin, IsNewUserManager, isNewUserLecturer, expertise);

        }

    }
}
