using Microsoft.AspNetCore.Mvc.RazorPages;
using CSE3PAX; 
using Microsoft.AspNetCore.Authorization;
using CSE3PAX.HelpClasses;

namespace CSE3PAX.Pages.Lecturer
{

    public class LecturerIndexModel : PageModel
    {

        //Current Table row headers
        public List<string> Next12Months { get; set; } = new List<string>();

        //SubjectInstance class in HelpClasses -> SubjectInstance.cs
        public List<SubjectInstance> SubjectInstances { get; set; } = new List<SubjectInstance>();


        public void OnGet()
        {
            //get current month and year 
            DateTime now = DateTime.Now;

            //add each month to table row headers.
            for (int i = 0; i < 12; i++)
            {
                DateTime nextMonth = now.AddMonths(i);
                string monthYearName = nextMonth.ToString("MMMM-yyyy");
                //Console.WriteLine(monthYearName);
                Next12Months.Add(monthYearName);
            }
            //Will need to connect to DB here.

            //Object SubjectInstance will need to have {instance name, subject name, start date, end date,}
           
            // adding a SubjectInstance
            SubjectInstances.Add(new SubjectInstance
            {
                InstanceName = "2023-MAT2DMX",
                SubjectName = "DISCRETE MATHEMATICS FOR COMP SCI",
                StartDate = "June-2024",
                EndDate = "September-2024"
            });

            SubjectInstances.Add(new SubjectInstance
            {
                InstanceName = "2024-CSE3PBX",
                SubjectName = "INDUSTRY PROJECT 3B",
                StartDate = "Febuary-2024",
                EndDate = "April-2024"
            });


            //SQL Query to pull all subject instances over the next 12 months where lecturer id = lecturer id
            // put each row into the subject instances list

            //For each subjectinstance in subjectinstances 
            //reorder by start date, if this cannot be done effectively by sql query


        }

        public void OnPost() { }
       

    
    }
}