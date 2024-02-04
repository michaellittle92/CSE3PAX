using Microsoft.AspNetCore.Mvc.RazorPages;
using CSE3PAX; 
using Microsoft.AspNetCore.Authorization;

namespace CSE3PAX.Pages.Lecturer
{
    [Authorize(Policy = "isLecturer")]

    public class LecturerIndexModel : PageModel
    {
        //get current month and year 
        //Array SubjectInstance {instance name, subject name, start date, end date,}
        //Array SubjectInstances[]

        //SQL Query to pull all subject instances over the next 12 months where lecturer id = lecturer id
        // put each row into the subject instances array

        //For each subjectinstance in subjectinstances 
            //reorder by start date, if this cannot be done effectively by sql query

        //get subjectinstance[0] start month
        //create 12 column table based on start month adding +1 to month for each column. 
        
        //for each row subjectinstance in subject instances 
            // 

        
    }
}