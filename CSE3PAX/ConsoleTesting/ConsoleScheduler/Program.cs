
using ConsoleScheduler;

internal class Program
{

    private static void Main(string[] args)
    {

        Guid userGuid = System.Guid.NewGuid();
        Console.WriteLine(userGuid);

        Console.WriteLine("Subject Scheduler - Console Edition");
        Console.WriteLine("--------------------");

        //Administrator.AddUser();
        Menu();

        

    }
    public static void LogIn()
    {
        string email;
        string password;




        Console.WriteLine("Login");
        Console.WriteLine("--------------------");
        Console.WriteLine("Email: ");
        email = Console.ReadLine();

        Console.WriteLine("Password: ");
        password = Console.ReadLine();

       

        int userID = Users.VerifyCredentials(email, password);
        if (userID > 0)
        {
            Console.WriteLine($"Login Successful, user id = {userID}, user is an {Users.PrivledgesCheck(userID)}");
            Users.IsPasswordResetRequired(userID);
        }
        else {
            Console.WriteLine("Login Failed");
        }

        if (Users.PrivledgesCheck(userID) == "Administrator")
        {
            Administrator.AdministratorMenu();
        }
      
        else
        {
            Console.WriteLine("Invalid User");
        }

        
    }



    public static void Menu() {
       
        int selectedOption;

        Console.WriteLine("Menu");
        Console.WriteLine("--------------------");

        Console.WriteLine("1. Log In");
        Console.WriteLine("2. Exit");

        selectedOption = Convert.ToInt32(Console.ReadLine());

        switch (selectedOption) { 
          case 1:
                       LogIn();
                       break;
                   case 2:
                       Environment.Exit(0);
                       break;
                   default:
                       Console.WriteLine("Invalid Input Selected.");
                       break;
               }
    }

}

